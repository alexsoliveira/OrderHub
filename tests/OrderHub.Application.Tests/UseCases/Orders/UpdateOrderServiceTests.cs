using Moq;
using OrderHub.Application.DTOs;
using OrderHub.Application.Ports;
using OrderHub.Application.UseCases.Orders;
using OrderHub.Domain.Aggregates.Order;

namespace OrderHub.Application.Tests.UseCases.Orders;

public class UpdateOrderServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly UpdateOrderService _service;

    public UpdateOrderServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOrderRepository = new Mock<IOrderRepository>();

        _mockUnitOfWork.Setup(x => x.Orders).Returns(_mockOrderRepository.Object);

        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockOrderRepository
            .Setup(x => x.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _service = new UpdateOrderService(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldUpdateOrderSuccessfully()
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();
        var customerId = Guid.NewGuid().ToString();

        var existingOrder = new OrderResponse
        {
            OrderId = orderId,
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            Items = new List<OrderItemResponse>(),
            TotalAmount = 0m,
            Currency = "BRL"
        };

        var request = new UpdateOrderRequest
        {
            OrderId = orderId,
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest
                {
                    ProductId = Guid.NewGuid().ToString(),
                    Quantity = 5,
                    UnitPrice = 25m
                },
                new OrderItemRequest
                {
                    ProductId = Guid.NewGuid().ToString(),
                    Quantity = 2,
                    UnitPrice = 75m
                }
            }
        };

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOrder);

        // Act
        var result = await _service.ExecuteAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result.OrderId);
        Assert.Equal(2, result.Items!.Count);
        Assert.Equal(275m, result.TotalAmount); // (5*25) + (2*75) = 125 + 150 = 275

        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockOrderRepository.Verify(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        _mockOrderRepository.Verify(x => x.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.ExecuteAsync(null!));
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyOrderId_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new UpdateOrderRequest
        {
            OrderId = "",
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
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentOrder_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();
        var request = new UpdateOrderRequest
        {
            OrderId = orderId,
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

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderResponse?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ExecuteAsync(request));

        _mockUnitOfWork.Verify(
            x => x.RollbackAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
