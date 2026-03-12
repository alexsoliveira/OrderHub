using Moq;
using OrderHub.Application.DTOs;
using OrderHub.Application.Ports;
using OrderHub.Application.UseCases.Orders;

namespace OrderHub.Application.Tests.UseCases.Orders;

public class GetOrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly GetOrderService _service;

    public GetOrderServiceTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _service = new GetOrderService(_mockOrderRepository.Object);
    }

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
        var result = await _service.ExecuteAsync(orderId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId, result.OrderId);
        Assert.Equal(expectedOrder.CustomerId, result.CustomerId);
        Assert.Single(result.Items!);
        Assert.Equal(100m, result.TotalAmount);

        _mockOrderRepository.Verify(
            x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()),
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
    public async Task ExecuteAsync_WithNonExistentOrderId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var orderId = Guid.NewGuid().ToString();
        _mockOrderRepository
            .Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderResponse?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ExecuteAsync(orderId));
    }

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
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                Items = new List<OrderItemResponse>(),
                TotalAmount = 100m,
                Currency = "BRL"
            },
            new OrderResponse
            {
                OrderId = Guid.NewGuid().ToString(),
                CustomerId = customerId,
                OrderDate = DateTime.UtcNow,
                Status = "Delivered",
                Items = new List<OrderItemResponse>(),
                TotalAmount = 200m,
                Currency = "BRL"
            }
        };

        _mockOrderRepository
            .Setup(x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedOrders);

        // Act
        var result = await _service.GetByCustomerAsync(customerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, order => Assert.Equal(customerId, order.CustomerId));

        _mockOrderRepository.Verify(
            x => x.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByCustomerAsync_WithEmptyCustomerId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.GetByCustomerAsync(""));
    }
}
