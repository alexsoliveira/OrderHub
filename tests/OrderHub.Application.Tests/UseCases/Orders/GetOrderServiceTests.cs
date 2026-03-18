using Microsoft.Extensions.Logging;
using Moq;
using OrderHub.Application.DTOs;
using OrderHub.Domain.Ports;
using OrderHub.Application.UseCases.Orders;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Application.Tests.UseCases.Orders;

/// <summary>
/// Testes do GetOrderService
/// Nota: Os testes estão em refatoração para se alinhar com as interfaces Domain.Ports
/// Por enquanto, apenas testes básicos de instanciação estão funcionando
/// </summary>
public class GetOrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<ILogger<GetOrderService>> _mockLogger;
    private readonly GetOrderService _service;

    public GetOrderServiceTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockLogger = new Mock<ILogger<GetOrderService>>();
        _service = new GetOrderService(_mockOrderRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithValidRepository_ShouldInitialize()
    {
        // Assert
        Assert.NotNull(_service);
    }

    // TODO: Adicionar mais testes considerando a interface Domain.Ports.IOrderRepository
    // Que trabalha com Order agregado, não com DTOs
}
