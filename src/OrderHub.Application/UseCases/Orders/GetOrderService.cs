using OrderHub.Application.DTOs;
using OrderHub.Application.Ports;
using OrderHub.Application.UseCases;

namespace OrderHub.Application.UseCases.Orders;

/// <summary>
/// Application Service para recuperar um pedido existente
/// Implementa IGetOrderUseCase (Input Port) da arquitetura hexagonal
/// </summary>
public class GetOrderService : IGetOrderUseCase
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    /// <summary>
    /// Executa a recuperação de um pedido pelo seu ID
    /// </summary>
    public async Task<OrderResponse> ExecuteAsync(
        string orderId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentException("OrderId é obrigatório", nameof(orderId));

        var orderResponse = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

        if (orderResponse == null)
            throw new InvalidOperationException($"Pedido com ID '{orderId}' não encontrado");

        return orderResponse;
    }

    /// <summary>
    /// Executa a recuperação de todos os pedidos de um cliente
    /// </summary>
    public async Task<List<OrderResponse>> GetByCustomerAsync(
        string customerId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("CustomerId é obrigatório", nameof(customerId));

        var orders = await _orderRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        return orders;
    }
}
