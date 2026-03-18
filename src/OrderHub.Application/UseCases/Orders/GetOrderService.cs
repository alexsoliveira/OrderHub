using OrderHub.Application.DTOs;
using OrderHub.Application.Exceptions;
using OrderHub.Domain.Ports;
using OrderHub.Application.UseCases;
using OrderHub.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace OrderHub.Application.UseCases.Orders;

/// <summary>
/// Application Service para recuperar um pedido existente
/// Implementa IGetOrderUseCase (Input Port) da arquitetura hexagonal
/// </summary>
public class GetOrderService : IGetOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<GetOrderService> _logger;

    public GetOrderService(IOrderRepository orderRepository, ILogger<GetOrderService> logger)
    {
        if (orderRepository == null)
            throw InvalidRequestException.CreateForNullField(nameof(orderRepository), "dependency injection failed");
        if (logger == null)
            throw InvalidRequestException.CreateForNullField(nameof(logger), "dependency injection failed");
        _orderRepository = orderRepository;
        _logger = logger;
    }

    /// <summary>
    /// Executa a recuperação de um pedido pelo seu ID
    /// </summary>
    public async Task<OrderResponse> ExecuteAsync(
        string orderId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando GetOrderUseCase para OrderId: {OrderId}", orderId);

        if (string.IsNullOrWhiteSpace(orderId))
            throw InvalidRequestException.CreateForNullField(nameof(orderId));

        // Converter string para OrderId Value Object
        if (!Guid.TryParse(orderId, out var guidId))
            throw new InvalidRequestException(nameof(orderId), "deve ser um GUID válido");

        var orderIdValueObject = OrderId.Create(guidId);
        var order = await _orderRepository.GetByIdAsync(orderIdValueObject, cancellationToken);

        if (order == null)
        {
            _logger.LogWarning("Pedido não encontrado. OrderId: {OrderId}", orderId);
            throw new OrderNotFoundException(orderId);
        }

        _logger.LogInformation("Pedido recuperado com sucesso. OrderId: {OrderId}, CustomerId: {CustomerId}", 
            orderId, order.CustomerId.Value);

        // Mapear Order (domínio) para OrderResponse (API)
        return MapOrderToResponse(order);
    }

    /// <summary>
    /// Executa a recuperação de todos os pedidos de um cliente
    /// </summary>
    public async Task<List<OrderResponse>> GetByCustomerAsync(
        string customerId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando GetByCustomerUseCase para CustomerId: {CustomerId}", customerId);

        if (string.IsNullOrWhiteSpace(customerId))
            throw InvalidRequestException.CreateForNullField(nameof(customerId));

        var orders = await _orderRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        
        _logger.LogInformation("Pedidos recuperados com sucesso. CustomerId: {CustomerId}, ItemCount: {ItemCount}", 
            customerId, orders.Count);

        return orders.Select(MapOrderToResponse).ToList();
    }

    /// <summary>
    /// Mapeia agregado Order para DTO OrderResponse
    /// </summary>
    private static OrderResponse MapOrderToResponse(OrderHub.Domain.Aggregates.Order.Order order)
    {
        var items = order.Items.Select(i => new OrderItemResponse
        {
            ProductId = i.ProductId.Value.ToString(),
            Quantity = i.Quantity,
            UnitPrice = i.Amount.Value,
            SubTotal = i.Amount.Value * i.Quantity
        }).ToList();

        var totalAmount = items.Sum(i => i.SubTotal);

        return new OrderResponse
        {
            OrderId = order.OrderId.Value.ToString(),
            CustomerId = order.CustomerId.Value.ToString(),
            Status = order.Status.ToString(),
            OrderDate = order.OrderDate,
            Items = items,
            TotalAmount = totalAmount,
            Currency = "BRL"
        };
    }
}
