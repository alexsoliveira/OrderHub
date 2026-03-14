using Moq;
using FluentAssertions;
using OrderHub.Application.DTOs;
using OrderHub.Application.Ports;
using OrderHub.Application.UseCases.Orders;
using Xunit;

namespace OrderHub.UnitTests.Application.UseCases;

/// <summary>
/// Testes unitários para GetOrderService (Use Case de recuperação de pedidos)
/// Valida consultas por ID e por cliente
/// Usa Moq para simular dependências externas (Repository)
/// </summary>
public class GetOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly GetOrderService _getOrderService;

    public GetOrderUseCaseTests()
    {
        // Arrange: Configurar mocks
        _mockOrderRepository = new Mock<IOrderRepository>();

        // Instanciar o service com o mock
        _getOrderService = new GetOrderService(_mockOrderRepository.Object);
    }

    // =========================
    // ExecuteAsync(orderId) Tests
    // =========================

    /// <summary>
    /// Testa o cenário feliz: recuperar pedido por ID quando existe
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithValidOrderId_ShouldReturnOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();
        var expectedOrder = new OrderResponse
        {
            OrderId = orderId,
            CustomerId = Guid.NewGuid().ToString(),
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            Items = new List<OrderItemResponse>
            {
                new OrderItemResponse
                {
                    ProductId = Guid.NewGuid().ToString(),
                    Quantity = 2,
                    UnitPrice = 50m,
                    SubTotal = 100m
                }
            },
            TotalAmount = 100m,
            Currency = "BRL"
        };

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _getOrderService.ExecuteAsync(orderId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.OrderId.Should().Be(orderId);
        result.CustomerId.Should().Be(expectedOrder.CustomerId);
        result.Status.Should().Be("Pending");
        result.Items.Should().HaveCount(1);
        result.TotalAmount.Should().Be(100m);

        // Verificar que o repository foi chamado corretamente
        _mockOrderRepository.Verify(
            x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Testa validação: OrderId vazio deve lançar exceção
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithEmptyOrderId_ShouldThrowArgumentException()
    {
        // Arrange
        var orderId = string.Empty;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _getOrderService.ExecuteAsync(orderId, CancellationToken.None));

        exception.ParamName.Should().Be("orderId");
    }

    /// <summary>
    /// Testa validação: OrderId nulo deve lançar exceção
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithNullOrderId_ShouldThrowArgumentException()
    {
        // Arrange
        string? orderId = null;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _getOrderService.ExecuteAsync(orderId!, CancellationToken.None));

        exception.ParamName.Should().Be("orderId");
    }

    /// <summary>
    /// Testa validação: OrderId com apenas whitespace deve lançar exceção
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithWhitespaceOrderId_ShouldThrowArgumentException()
    {
        // Arrange
        var orderId = "   ";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _getOrderService.ExecuteAsync(orderId, CancellationToken.None));

        exception.ParamName.Should().Be("orderId");
    }

    /// <summary>
    /// Testa erro: OrderId não existente deve lançar InvalidOperationException
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithNonExistentOrderId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderResponse?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _getOrderService.ExecuteAsync(orderId, CancellationToken.None));

        exception.Message.Should().Contain(orderId);
        exception.Message.Should().Contain("não encontrado");
    }

    /// <summary>
    /// Testa recuperação de pedido com múltiplos itens
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithMultipleItems_ShouldReturnOrderWithAllItems()
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();
        var expectedOrder = new OrderResponse
        {
            OrderId = orderId,
            CustomerId = Guid.NewGuid().ToString(),
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            Items = new List<OrderItemResponse>
            {
                new OrderItemResponse { ProductId = "prod-001", Quantity = 2, UnitPrice = 50m, SubTotal = 100m },
                new OrderItemResponse { ProductId = "prod-002", Quantity = 1, UnitPrice = 75m, SubTotal = 75m },
                new OrderItemResponse { ProductId = "prod-003", Quantity = 3, UnitPrice = 25m, SubTotal = 75m }
            },
            TotalAmount = 250m,
            Currency = "BRL"
        };

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _getOrderService.ExecuteAsync(orderId, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(3);
        result.TotalAmount.Should().Be(250m);
    }

    /// <summary>
    /// Testa recuperação de pedido com status diferente
    /// </summary>
    [Theory]
    [InlineData("Pending")]
    [InlineData("Shipped")]
    [InlineData("Delivered")]
    [InlineData("Cancelled")]
    public async Task ExecuteAsync_WithDifferentStatuses_ShouldReturnCorrectStatus(string status)
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();
        var expectedOrder = new OrderResponse
        {
            OrderId = orderId,
            CustomerId = Guid.NewGuid().ToString(),
            OrderDate = DateTime.UtcNow,
            Status = status,
            Items = new List<OrderItemResponse>(),
            TotalAmount = 100m,
            Currency = "BRL"
        };

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _getOrderService.ExecuteAsync(orderId, CancellationToken.None);

        // Assert
        result.Status.Should().Be(status);
    }

    // =========================
    // GetByCustomerAsync() Tests
    // =========================

    /// <summary>
    /// Testa recuperação de pedidos de um cliente com pedidos
    /// </summary>
    [Fact]
    public async Task GetByCustomerAsync_WithValidCustomerId_ShouldReturnOrderList()
    {
        // Arrange
        var customerId = Guid.NewGuid().ToString();
        var expectedOrders = new List<OrderResponse>
        {
            new OrderResponse
            {
                OrderId = Guid.NewGuid().ToString(),
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow.AddDays(-5),
                Status = "Delivered",
                Items = new List<OrderItemResponse>(),
                TotalAmount = 100m,
                Currency = "BRL"
            },
            new OrderResponse
            {
                OrderId = Guid.NewGuid().ToString(),
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow.AddDays(-2),
                Status = "Pending",
                Items = new List<OrderItemResponse>(),
                TotalAmount = 200m,
                Currency = "BRL"
            }
        };

        _mockOrderRepository
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOrders);

        // Act
        var result = await _getOrderService.GetByCustomerAsync(customerId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(o => o.CustomerId.Should().Be(customerId));
        result[0].TotalAmount.Should().Be(100m);
        result[1].TotalAmount.Should().Be(200m);

        // Verificar que o repository foi chamado
        _mockOrderRepository.Verify(
            x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Testa recuperação para cliente sem pedidos (lista vazia, não erro)
    /// </summary>
    [Fact]
    public async Task GetByCustomerAsync_WithCustomerHavingNoOrders_ShouldReturnEmptyList()
    {
        // Arrange
        var customerId = Guid.NewGuid().ToString();
        var emptyOrderList = new List<OrderResponse>();

        _mockOrderRepository
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyOrderList);

        // Act
        var result = await _getOrderService.GetByCustomerAsync(customerId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Testa validação: CustomerId vazio deve lançar exceção
    /// </summary>
    [Fact]
    public async Task GetByCustomerAsync_WithEmptyCustomerId_ShouldThrowArgumentException()
    {
        // Arrange
        var customerId = string.Empty;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _getOrderService.GetByCustomerAsync(customerId, CancellationToken.None));

        exception.ParamName.Should().Be("customerId");
    }

    /// <summary>
    /// Testa validação: CustomerId nulo deve lançar exceção
    /// </summary>
    [Fact]
    public async Task GetByCustomerAsync_WithNullCustomerId_ShouldThrowArgumentException()
    {
        // Arrange
        string? customerId = null;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _getOrderService.GetByCustomerAsync(customerId!, CancellationToken.None));

        exception.ParamName.Should().Be("customerId");
    }

    /// <summary>
    /// Testa validação: CustomerId com apenas whitespace deve lançar exceção
    /// </summary>
    [Fact]
    public async Task GetByCustomerAsync_WithWhitespaceCustomerId_ShouldThrowArgumentException()
    {
        // Arrange
        var customerId = "   ";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _getOrderService.GetByCustomerAsync(customerId, CancellationToken.None));

        exception.ParamName.Should().Be("customerId");
    }

    /// <summary>
    /// Testa recuperação de múltiplos pedidos ordena por data
    /// </summary>
    [Fact]
    public async Task GetByCustomerAsync_WithMultipleOrders_ShouldReturnAllOrders()
    {
        // Arrange
        var customerId = Guid.NewGuid().ToString();
        var expectedOrders = new List<OrderResponse>();

        // Gerar 5 pedidos
        for (int i = 1; i <= 5; i++)
        {
            expectedOrders.Add(new OrderResponse
            {
                OrderId = Guid.NewGuid().ToString(),
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow.AddDays(-i),
                Status = "Pending",
                Items = new List<OrderItemResponse>(),
                TotalAmount = i * 100m,
                Currency = "BRL"
            });
        }

        _mockOrderRepository
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOrders);

        // Act
        var result = await _getOrderService.GetByCustomerAsync(customerId, CancellationToken.None);

        // Assert
        result.Should().HaveCount(5);
        result.Should().AllSatisfy(o => o.CustomerId.Should().Be(customerId));
    }

    /// <summary>
    /// Testa que o repository é chamado apenas uma vez
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_ShouldCallRepositoryExactlyOnce()
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();
        var expectedOrder = new OrderResponse
        {
            OrderId = orderId,
            CustomerId = Guid.NewGuid().ToString(),
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            Items = new List<OrderItemResponse>(),
            TotalAmount = 100m,
            Currency = "BRL"
        };

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOrder);

        // Act
        await _getOrderService.ExecuteAsync(orderId, CancellationToken.None);

        // Assert
        _mockOrderRepository.Verify(
            x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    /// Testa que GetByCustomerAsync chama repository corretamente
    /// </summary>
    [Fact]
    public async Task GetByCustomerAsync_ShouldCallRepositoryWithCorrectParameters()
    {
        // Arrange
        var customerId = Guid.NewGuid().ToString();
        _mockOrderRepository
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<OrderResponse>());

        // Act
        await _getOrderService.GetByCustomerAsync(customerId, CancellationToken.None);

        // Assert
        _mockOrderRepository.Verify(
            x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
