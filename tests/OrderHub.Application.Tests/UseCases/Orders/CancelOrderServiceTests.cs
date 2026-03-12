using Moq;
using OrderHub.Application.DTOs;
using OrderHub.Application.Ports;
using OrderHub.Application.UseCases.Orders;

namespace OrderHub.Application.Tests.UseCases.Orders;

public class CancelOrderServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<INotificationPort> _mockNotification;
    private readonly CancelOrderService _service;

    public CancelOrderServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockNotification = new Mock<INotificationPort>();

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
            .Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockNotification
            .Setup(x => x.SendOrderCancelledAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _service = new CancelOrderService(_mockUnitOfWork.Object, _mockNotification.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidOrderId_ShouldCancelOrderSuccessfully()
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
            TotalAmount = 100m,
            Currency = "BRL"
        };

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOrder);

        // Act
        await _service.ExecuteAsync(orderId);

        // Assert
        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockOrderRepository.Verify(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        _mockOrderRepository.Verify(x => x.DeleteAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockNotification.Verify(
            x => x.SendOrderCancelledAsync(
                customerId,
                orderId,
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidOrderIdAndReason_ShouldCancelWithReason()
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();
        var customerId = Guid.NewGuid().ToString();
        var reason = "Customer requested cancellation";

        var existingOrder = new OrderResponse
        {
            OrderId = orderId,
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            Items = new List<OrderItemResponse>(),
            TotalAmount = 100m,
            Currency = "BRL"
        };

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOrder);

        // Act
        await _service.ExecuteAsync(orderId, reason);

        // Assert
        _mockNotification.Verify(
            x => x.SendOrderCancelledAsync(
                customerId,
                orderId,
                reason,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyOrderId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.ExecuteAsync(""));
    }

    [Fact]
    public async Task ExecuteAsync_WithNullOrderId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.ExecuteAsync(null!));
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentOrder_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();
        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderResponse?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ExecuteAsync(orderId));

        _mockUnitOfWork.Verify(
            x => x.RollbackAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenDeleteFails_ShouldRollbackTransaction()
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
            TotalAmount = 100m,
            Currency = "BRL"
        };

        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOrder);

        _mockOrderRepository
            .Setup(x => x.DeleteAsync(orderId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ExecuteAsync(orderId));

        _mockUnitOfWork.Verify(
            x => x.RollbackAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
