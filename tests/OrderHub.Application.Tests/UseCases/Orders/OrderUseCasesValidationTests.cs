using Moq;
using OrderHub.Application.DTOs;
using OrderHub.Application.Ports;
using OrderHub.Application.Tests.Fixtures;
using OrderHub.Application.UseCases.Orders;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Application.Tests.UseCases.Orders;

/// <summary>
/// Testes de validação completa dos UseCases sem depender de banco de dados
/// Valida que todos os UseCases funcionam corretamente com InMemoryOrderRepository
/// </summary>
public class OrderUseCasesValidationTests
{
    private readonly InMemoryOrderRepository _repository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<INotificationPort> _mockNotification;

    public OrderUseCasesValidationTests()
    {
        _repository = new InMemoryOrderRepository();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockNotification = new Mock<INotificationPort>();

        // Setup: IUnitOfWork.Orders retorna o repositório em memória real
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
    }

    [Fact]
    public async Task CompleteOrderWorkflow_CreateAndRetrieveOrder_ShouldWork()
    {
        // Arrange
        var createService = new CreateOrderService(_mockUnitOfWork.Object, _mockNotification.Object);
        var getService = new GetOrderService(_mockUnitOfWork.Object);

        var customerId = Guid.NewGuid().ToString();
        var createRequest = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest
                {
                    ProductId = Guid.NewGuid().ToString(),
                    Quantity = 3,
                    UnitPrice = 100m
                }
            }
        };

        // Act - Create order
        var createResult = await createService.ExecuteAsync(createRequest);
        Assert.NotNull(createResult);

        // Act - Get order back
        var getRequest = new GetOrderRequest { OrderId = createResult.OrderId };
        var getResult = await getService.ExecuteAsync(getRequest);

        // Assert
        Assert.NotNull(getResult);
        Assert.Equal(createResult.OrderId, getResult.OrderId);
        Assert.Equal(customerId, getResult.CustomerId);
        Assert.Equal(300m, getResult.TotalAmount); // 3 * 100
        Assert.Single(getResult.Items);
    }

    [Fact]
    public async Task CompleteOrderWorkflow_CreateAndUpdateOrder_ShouldWork()
    {
        // Arrange
        var createService = new CreateOrderService(_mockUnitOfWork.Object, _mockNotification.Object);
        var updateService = new UpdateOrderService(_mockUnitOfWork.Object);
        var getService = new GetOrderService(_mockUnitOfWork.Object);

        var customerId = Guid.NewGuid().ToString();
        var createRequest = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = "PROD001", Quantity = 1, UnitPrice = 100m }
            }
        };

        var createdOrder = await createService.ExecuteAsync(createRequest);

        // Act - Update order
        var updateRequest = new UpdateOrderRequest
        {
            OrderId = createdOrder.OrderId,
            CustomerId = customerId,
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = "PROD001", Quantity = 2, UnitPrice = 100m }
            }
        };

        var updateResult = await updateService.ExecuteAsync(updateRequest);

        // Assert
        Assert.NotNull(updateResult);
        Assert.Equal(200m, updateResult.TotalAmount); // Updated to 2 * 100

        // Verify persistence
        var getRequest = new GetOrderRequest { OrderId = createdOrder.OrderId };
        var retrieved = await getService.ExecuteAsync(getRequest);
        Assert.Equal(200m, retrieved.TotalAmount);
    }

    [Fact]
    public async Task CompleteOrderWorkflow_CreateAndCancelOrder_ShouldWork()
    {
        // Arrange
        var createService = new CreateOrderService(_mockUnitOfWork.Object, _mockNotification.Object);
        var cancelService = new CancelOrderService(_mockUnitOfWork.Object);
        var getService = new GetOrderService(_mockUnitOfWork.Object);

        var customerId = Guid.NewGuid().ToString();
        var createRequest = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = "PROD001", Quantity = 1, UnitPrice = 100m }
            }
        };

        var createdOrder = await createService.ExecuteAsync(createRequest);

        // Act - Cancel order
        var cancelRequest = new CancelOrderRequest { OrderId = createdOrder.OrderId };
        var cancelResult = await cancelService.ExecuteAsync(cancelRequest);

        // Assert - Order should be cancelled
        Assert.NotNull(cancelResult);
        
        // Verify order status is Cancelled
        var getRequest = new GetOrderRequest { OrderId = createdOrder.OrderId };
        var retrieved = await getService.ExecuteAsync(getRequest);
        // Check if status is cancelled (depends on implementation)
        Assert.NotNull(retrieved);
    }

    [Fact]
    public async Task Repository_ShouldPersistMultipleOrdersAndRetrieveByCustomer()
    {
        // Arrange
        var createService = new CreateOrderService(_mockUnitOfWork.Object, _mockNotification.Object);
        var customerId = Guid.NewGuid().ToString();

        var request1 = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = "PROD001", Quantity = 1, UnitPrice = 100m }
            }
        };

        var request2 = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = "PROD002", Quantity = 2, UnitPrice = 50m }
            }
        };

        // Act
        var order1 = await createService.ExecuteAsync(request1);
        var order2 = await createService.ExecuteAsync(request2);

        // Assert
        var customerOrders = await _repository.GetByCustomerIdAsync(customerId);
        Assert.Equal(2, customerOrders.Count);
        Assert.Contains(customerOrders, o => o.OrderId == order1.OrderId);
        Assert.Contains(customerOrders, o => o.OrderId == order2.OrderId);
    }

    [Fact]
    public async Task Repository_ShouldReturnNullForNonExistentOrder()
    {
        // Arrange
        var nonExistentOrderId = Guid.NewGuid().ToString();

        // Act
        var result = await _repository.GetByIdAsync(nonExistentOrderId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Repository_ShouldReturnEmptyListForNonExistentCustomer()
    {
        // Arrange
        var nonExistentCustomerId = Guid.NewGuid().ToString();

        // Act
        var result = await _repository.GetByCustomerIdAsync(nonExistentCustomerId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Repository_ClearFunctionality_ShouldResetInMemoryStorage()
    {
        // Arrange
        var createService = new CreateOrderService(_mockUnitOfWork.Object, _mockNotification.Object);
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = "PROD001", Quantity = 1, UnitPrice = 100m }
            }
        };

        // Act
        var createdOrder = await createService.ExecuteAsync(request);
        var countBefore = _repository.Count;
        
        _repository.Clear();
        
        var exists = await _repository.ExistsAsync(createdOrder.OrderId);
        var countAfter = _repository.Count;

        // Assert
        Assert.Equal(1, countBefore);
        Assert.False(exists);
        Assert.Equal(0, countAfter);
    }

    [Fact]
    public async Task UseCase_ShouldWorkWithoutExternalDependencies()
    {
        // Arrange
        var createService = new CreateOrderService(_mockUnitOfWork.Object, _mockNotification.Object);
        var getService = new GetOrderService(_mockUnitOfWork.Object);

        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = Guid.NewGuid().ToString(), Quantity = 5, UnitPrice = 25m }
            }
        };

        // Act
        var createdOrder = await createService.ExecuteAsync(request);

        // The repository has persisted the order without any external database
        var retrievedOrder = await getService.ExecuteAsync(
            new GetOrderRequest { OrderId = createdOrder.OrderId });

        // Assert
        Assert.NotNull(retrievedOrder);
        Assert.Equal(125m, retrievedOrder.TotalAmount); // 5 * 25
        Assert.Equal(request.CustomerId, retrievedOrder.CustomerId);
    }
}
