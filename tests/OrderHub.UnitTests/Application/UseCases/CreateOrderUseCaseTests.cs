using Moq;
using FluentAssertions;
using OrderHub.Application.DTOs;
using OrderHub.Application.Ports;
using OrderHub.Application.UseCases.Orders;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.ValueObjects;
using Xunit;

namespace OrderHub.UnitTests.Application.UseCases;

/// <summary>
/// Testes unitários para CreateOrderService (Use Case de criação de pedidos)
/// Valida a orquestração entre camada de aplicação e domínio
/// Usa Moq para simular dependências externas (Repositories, Notification)
/// </summary>
public class CreateOrderUseCaseTests
{
    // Setup das dependências mock
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<INotificationPort> _mockNotification;
    private readonly CreateOrderService _createOrderService;

    public CreateOrderUseCaseTests()
    {
        // Arrange: Configurar mocks
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockNotification = new Mock<INotificationPort>();

        // Configurar o mock de UnitOfWork para retornar o mock de repository
        _mockUnitOfWork.Setup(x => x.Orders).Returns(_mockOrderRepository.Object);

        // Instanciar o service com os mocks
        _createOrderService = new CreateOrderService(
            _mockUnitOfWork.Object,
            _mockNotification.Object);
    }

    /// <summary>
    /// Testa o cenário feliz: criar pedido com dados válidos
    /// Valida se o pedido foi criado corretamente e persistido
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreateOrderSuccessfully()
    {
        // Arrange
        var customerId = Guid.NewGuid().ToString();
        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest
                {
                    ProductId = Guid.NewGuid().ToString(),
                    Quantity = 2,
                    UnitPrice = 99.90m
                },
                new OrderItemRequest
                {
                    ProductId = Guid.NewGuid().ToString(),
                    Quantity = 1,
                    UnitPrice = 149.99m
                }
            },
            Description = "Pedido de teste"
        };

        // Configurar mocks para sucesso
        _mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockOrderRepository.Setup(x => x.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockNotification.Setup(x => x.SendOrderConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _createOrderService.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.OrderId.Should().NotBeNullOrEmpty();
        result.CustomerId.Should().Be(customerId);
        result.Status.Should().Be("Pending");
        result.Items.Should().HaveCount(2);
        result.TotalAmount.Should().Be(349.79m); // (2 * 99.90) + (1 * 149.99) = 349.79

        // Verificar que as dependências foram chamadas
        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockOrderRepository.Verify(x => x.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockNotification.Verify(x => x.SendOrderConfirmationAsync(customerId, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Testa validação: request nulo deve lançar exceção
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Arrange
        CreateOrderRequest? request = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _createOrderService.ExecuteAsync(request!, CancellationToken.None));
    }

    /// <summary>
    /// Testa validação: CustomerId vazio deve lançar exceção
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithEmptyCustomerId_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = string.Empty,
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest
                {
                    ProductId = Guid.NewGuid().ToString(),
                    Quantity = 1,
                    UnitPrice = 50m
                }
            }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _createOrderService.ExecuteAsync(request, CancellationToken.None));

        exception.ParamName.Should().Be("CustomerId");
    }

    /// <summary>
    /// Testa validação: Items vazio deve lançar exceção
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithEmptyItems_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
            Items = new List<OrderItemRequest>() // Lista vazia
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _createOrderService.ExecuteAsync(request, CancellationToken.None));

        exception.ParamName.Should().Be("Items");
    }

    /// <summary>
    /// Testa validação: Items nulo deve lançar exceção
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithNullItems_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
            Items = null! // Items nulo
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _createOrderService.ExecuteAsync(request, CancellationToken.None));

        exception.ParamName.Should().Be("Items");
    }

    /// <summary>
    /// Testa cálculo correto do total com múltiplos itens
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithMultipleItems_ShouldCalculateTotalCorrectly()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = "prod-001", Quantity = 3, UnitPrice = 50m },    // 150
                new OrderItemRequest { ProductId = "prod-002", Quantity = 2, UnitPrice = 75.50m }, // 151
                new OrderItemRequest { ProductId = "prod-003", Quantity = 4, UnitPrice = 25m }     // 100
            }
        };

        _mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockOrderRepository.Setup(x => x.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockNotification.Setup(x => x.SendOrderConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _createOrderService.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.TotalAmount.Should().Be(401m); // 150 + 151 + 100
        result.Items.Should().HaveCount(3);
    }

    /// <summary>
    /// Testa que cada pedido criado tem um OrderId único
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_MultipleRequests_ShouldGenerateUniqueOrderIds()
    {
        // Arrange
        var request1 = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = "prod-001", Quantity = 1, UnitPrice = 100m }
            }
        };

        var request2 = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = "prod-002", Quantity = 1, UnitPrice = 100m }
            }
        };

        _mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockOrderRepository.Setup(x => x.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockNotification.Setup(x => x.SendOrderConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result1 = await _createOrderService.ExecuteAsync(request1, CancellationToken.None);
        var result2 = await _createOrderService.ExecuteAsync(request2, CancellationToken.None);

        // Assert
        result1.OrderId.Should().NotBe(result2.OrderId);
    }

    /// <summary>
    /// Testa que o pedido retorna com status inicial "Pending"
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_NewOrder_ShouldHavePendingStatus()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = "prod-001", Quantity = 1, UnitPrice = 100m }
            }
        };

        _mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockOrderRepository.Setup(x => x.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockNotification.Setup(x => x.SendOrderConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _createOrderService.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Status.Should().Be("Pending");
    }

    /// <summary>
    /// Testa que a transação é iniciada antes da criação
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldBeginTransactionBeforeSaving()
    {
        // Arrange
        var callOrder = new List<string>();

        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = "prod-001", Quantity = 1, UnitPrice = 100m }
            }
        };

        _mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("BeginTransaction"))
            .Returns(Task.CompletedTask);

        _mockOrderRepository.Setup(x => x.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("SaveAsync"))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Callback(() => callOrder.Add("Commit"))
            .Returns(Task.CompletedTask);

        _mockNotification.Setup(x => x.SendOrderConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _createOrderService.ExecuteAsync(request, CancellationToken.None);

        // Assert
        callOrder.Should().BeInAscendingOrder("BeginTransaction", "SaveAsync", "Commit");
        callOrder[0].Should().Be("BeginTransaction");
    }

    /// <summary>
    /// Testa que em caso de erro, o rollback é chamado
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WhenRepositoryThrowsException_ShouldRollback()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = "prod-001", Quantity = 1, UnitPrice = 100m }
            }
        };

        _mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockOrderRepository.Setup(x => x.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        _mockUnitOfWork.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _createOrderService.ExecuteAsync(request, CancellationToken.None));

        // Verificar que rollback foi chamado
        _mockUnitOfWork.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Testa CustomerId com whitespace deve lançar exceção
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithWhitespaceCustomerId_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = "   ", // Apenas espaços em branco
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = "prod-001", Quantity = 1, UnitPrice = 50m }
            }
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _createOrderService.ExecuteAsync(request, CancellationToken.None));

        exception.ParamName.Should().Be("CustomerId");
    }

    /// <summary>
    /// Testa que a notificação é enviada após sucesso
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_OnSuccess_ShouldSendNotification()
    {
        // Arrange
        var customerId = Guid.NewGuid().ToString();
        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = "prod-001", Quantity = 1, UnitPrice = 100m }
            }
        };

        _mockUnitOfWork.Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockOrderRepository.Setup(x => x.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockNotification.Setup(x => x.SendOrderConfirmationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _createOrderService.ExecuteAsync(request, CancellationToken.None);

        // Assert
        _mockNotification.Verify(x => x.SendOrderConfirmationAsync(customerId, result.OrderId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
