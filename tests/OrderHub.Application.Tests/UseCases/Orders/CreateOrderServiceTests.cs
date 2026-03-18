using Microsoft.Extensions.Logging;
using Moq;
using OrderHub.Application.DTOs;
using OrderHub.Domain.Ports;
using OrderHub.Application.UseCases.Orders;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Application.Tests.UseCases.Orders;

public class CreateOrderServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<INotificationPort> _mockNotification;
    private readonly Mock<ILogger<CreateOrderService>> _mockLogger;
    private readonly CreateOrderService _service;

    public CreateOrderServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockNotification = new Mock<INotificationPort>();
        _mockLogger = new Mock<ILogger<CreateOrderService>>();

        // Setup: A propriedade Orders do IUnitOfWork retorna o mock do repositório
        _mockUnitOfWork.Setup(x => x.Orders).Returns(_mockOrderRepository.Object);

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

        // Setup padrão para save
        _mockOrderRepository
            .Setup(x => x.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Setup padrão para notificação
        _mockNotification
            .Setup(x => x.SendOrderConfirmationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _service = new CreateOrderService(_mockUnitOfWork.Object, _mockNotification.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ShouldCreateOrderSuccessfully()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
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

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.OrderId);
        Assert.Equal(request.CustomerId, result.CustomerId);
        Assert.Single(result.Items);
        Assert.Equal(100m, result.TotalAmount); // 2 * 50

        // Verify que as dependências foram chamadas
        _mockUnitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockOrderRepository.Verify(x => x.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockNotification.Verify(
            x => x.SendOrderConfirmationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.ExecuteAsync(null!));
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyCustomerId_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = "",
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
    public async Task ExecuteAsync_WithEmptyItems_ShouldThrowArgumentException()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
            Items = new List<OrderItemRequest>()
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.ExecuteAsync(request));
    }

    [Fact]
    public async Task ExecuteAsync_WithMultipleItems_ShouldCalculateTotalCorrectly()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductId = Guid.NewGuid().ToString(), Quantity = 2, UnitPrice = 50m },
                new OrderItemRequest { ProductId = Guid.NewGuid().ToString(), Quantity = 3, UnitPrice = 30m }
            }
        };

        // Act
        var result = await _service.ExecuteAsync(request);

        // Assert
        Assert.Equal(2, result.Items!.Count);
        Assert.Equal(190m, result.TotalAmount); // (2*50) + (3*30) = 100 + 90 = 190
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepositorySaveFails_ShouldRollbackTransaction()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid().ToString(),
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
            .Setup(x => x.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ExecuteAsync(request));

        // Verify rollback foi chamado
        _mockUnitOfWork.Verify(
            x => x.RollbackAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
