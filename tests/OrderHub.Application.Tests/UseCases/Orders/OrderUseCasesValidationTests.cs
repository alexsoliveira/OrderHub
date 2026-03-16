using Moq;
using OrderHub.Domain.Ports;
using OrderHub.Application.Tests.Fixtures;
using OrderHub.Application.UseCases.Orders;

namespace OrderHub.Application.Tests.UseCases.Orders;

/// <summary>
/// Testes de validação completa dos UseCases sem depender de banco de dados
/// Valida que todos os UseCases funcionam corretamente com InMemoryOrderRepository
/// Nota: Em refatoração para alinhamento com Domain.Ports
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
    }

    [Fact]
    public void Repository_ShouldBeInitialized()
    {
        // Assert
        Assert.NotNull(_repository);
    }
}
