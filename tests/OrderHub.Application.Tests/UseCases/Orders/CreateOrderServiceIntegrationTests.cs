using Moq;
using OrderHub.Application.DTOs;
using OrderHub.Application.Ports;
using OrderHub.Application.Tests.Fixtures;
using OrderHub.Application.UseCases.Orders;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Application.Tests.UseCases.Orders;

/// <summary>
/// Testes de integração usando InMemoryOrderRepository
/// Validam os UseCases sem depender de banco de dados externo
/// </summary>
public class CreateOrderServiceIntegrationTests
{
    private readonly InMemoryOrderRepository _repository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<INotificationPort> _mockNotification;
    private readonly CreateOrderService _service;

    public CreateOrderServiceIntegrationTests()
    {
        _repository = new InMemoryOrderRepository();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockNotification = new Mock<INotificationPort>();

        // Setup: A propriedade Orders do IUnitOfWork retorna o repositório em memória real
        _mockUnitOfWork.Setup(x => x.Orders).Returns(_repository);

        // Setup padrão para transações
        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Setup padrão para notificação
        _mockNotification
            .Setup(x => x.SendOrderConfirmationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _service = new CreateOrderService(_mockUnitOfWork.Object, _mockNotification.Object);
    }

    [Fact]
    public async Task CreateOrderService_ShouldPersistOrderInRepository()
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
                    UnitPrice = 50m
                }
            }
        };

        // Act
        var result = await _service.ExecuteAsync(request);

        // Assert - Verifica se o pedido foi persistido no repositório em memória
        Assert.NotNull(result);
        var savedOrder = await _repository.GetByIdAsync(result.OrderId);
        Assert.NotNull(savedOrder);
        Assert.Equal(result.OrderId, savedOrder.OrderId);
        Assert.Equal(customerId, savedOrder.CustomerId);
    }

    [Fact]
    public async Task CreateOrderService_MultipleOrders_ShouldRetrieveByCustomerId()
    {
        // Arrange
        var customerId = Guid.NewGuid().ToString();
        var request1 = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = Guid.NewGuid().ToString(), Quantity = 1, UnitPrice = 100m }
            }
        };

        var request2 = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = Guid.NewGuid().ToString(), Quantity = 2, UnitPrice = 50m }
            }
        };

        // Act
        var result1 = await _service.ExecuteAsync(request1);
        var result2 = await _service.ExecuteAsync(request2);

        // Assert - Verifica se ambos os pedidos foram salvos e podem ser recuperados
        var customerOrders = await _repository.GetByCustomerIdAsync(customerId);
        Assert.NotEmpty(customerOrders);
        Assert.Equal(2, customerOrders.Count);
    }

    [Fact]
    public async Task CreateOrderService_ShouldVerifyOrderExists()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = Guid.NewGuid().ToString(), Quantity = 1, UnitPrice = 100m }
            }
        };

        // Act
        var result = await _service.ExecuteAsync(request);

        // Assert - Verifica se a existe de ordem funciona
        var exists = await _repository.ExistsAsync(result.OrderId);
        Assert.True(exists);
    }

    [Fact]
    public async Task CreateOrderService_RepositoryCanBeCleared()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = Guid.NewGuid().ToString(), Quantity = 1, UnitPrice = 100m }
            }
        };

        // Act
        var result = await _service.ExecuteAsync(request);
        var countBefore = _repository.Count;
        _repository.Clear();
        var countAfter = _repository.Count;

        // Assert - Verifica se o repositório foi limpo corretamente
        Assert.Equal(1, countBefore);
        Assert.Equal(0, countAfter);
    }
}
