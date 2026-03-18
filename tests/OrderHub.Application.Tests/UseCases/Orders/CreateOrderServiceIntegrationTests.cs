using Microsoft.Extensions.Logging;
using Moq;
using OrderHub.Domain.Ports;
using OrderHub.Application.Tests.Fixtures;
using OrderHub.Application.UseCases.Orders;

namespace OrderHub.Application.Tests.UseCases.Orders;

/// <summary>
/// Testes de integração usando InMemoryOrderRepository
/// Validam os UseCases sem depender de banco de dados externo
/// Nota: Em refatoração para alinhamento com Domain.Ports
/// </summary>
public class CreateOrderServiceIntegrationTests
{
    private readonly InMemoryOrderRepository _repository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<INotificationPort> _mockNotification;
    private readonly Mock<ILogger<CreateOrderService>> _mockLogger;
    private readonly CreateOrderService _service;

    public CreateOrderServiceIntegrationTests()
    {
        _repository = new InMemoryOrderRepository();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockNotification = new Mock<INotificationPort>();
        _mockLogger = new Mock<ILogger<CreateOrderService>>();

        _mockUnitOfWork.Setup(x => x.Orders).Returns(_repository);
        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockNotification
            .Setup(x => x.SendOrderConfirmationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _service = new CreateOrderService(_mockUnitOfWork.Object, _mockNotification.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidDependencies_ShouldInitialize()
    {
        // Assert
        Assert.NotNull(_service);
    }

    // TODO: Implementar testes de integração com Domain.Ports
}
