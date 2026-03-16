using OrderHub.Application.DTOs;
using OrderHub.Application.Mappers;
using OrderHub.Domain.Ports;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Application.Tests.Fixtures;

/// <summary>
/// Repositório em memória para testes unitários
/// Implementa IOrderRepository armazenando pedidos em um dicionário na memória
/// </summary>
public class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<string, Order> _orders = new();

    /// <summary>
    /// Recupera um pedido pelo seu identificador (Application - string version)
    /// </summary>
    public Task<OrderResponse?> GetByIdAsync(string orderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return Task.FromResult<OrderResponse?>(null);

        var found = _orders.TryGetValue(orderId, out var order);
        if (!found)
            return Task.FromResult<OrderResponse?>(null);

        var response = OrderMapper.ToResponse(order);
        return Task.FromResult<OrderResponse?>(response);
    }

    /// <summary>
    /// Recupera todos os pedidos de um cliente (Application - DTO version)
    /// </summary>
    public Task<List<OrderResponse>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return Task.FromResult(new List<OrderResponse>());

        var orders = _orders.Values
            .Where(o => o.CustomerId.Value.ToString() == customerId)
            .Select(o => OrderMapper.ToResponse(o))
            .ToList();

        return Task.FromResult(orders);
    }

    /// <summary>
    /// Recupera um pedido pelo identificador (Domain - OrderId version)
    /// </summary>
    Task<Order?> IOrderRepository.GetByIdAsync(OrderId orderId, CancellationToken cancellationToken = default)
    {
        if (orderId == null)
            return Task.FromResult<Order?>(null);

        var found = _orders.TryGetValue(orderId.Value.ToString(), out var order);
        return Task.FromResult<Order?>(found ? order : null);
    }

    /// <summary>
    /// Recupera todos os pedidos de um cliente (Domain - Aggregate version)
    /// </summary>
    async Task<List<Order>> IOrderRepository.GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return new List<Order>();

        var orders = _orders.Values
            .Where(o => o.CustomerId.Value.ToString() == customerId)
            .ToList();

        return await Task.FromResult(orders);
    }

    /// <summary>
    /// Persiste um novo pedido ou atualiza um existente
    /// </summary>
    public Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        _orders[order.OrderId.Value.ToString()] = order;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Remove um pedido pelo seu identificador
    /// </summary>
    public Task DeleteAsync(OrderId orderId, CancellationToken cancellationToken = default)
    {
        if (orderId == null)
            return Task.CompletedTask;

        _orders.Remove(orderId.Value.ToString());
        return Task.CompletedTask;
    }

    /// <summary>
    /// Verifica se um pedido existe
    /// </summary>
    public Task<bool> ExistsAsync(OrderId orderId, CancellationToken cancellationToken = default)
    {
        if (orderId == null)
            return Task.FromResult(false);

        var exists = _orders.ContainsKey(orderId.Value.ToString());
        return Task.FromResult(exists);
    }

    /// <summary>
    /// Limpa todos os pedidos armazenados (útil para resetar estado em testes)
    /// </summary>
    public void Clear()
    {
        _orders.Clear();
    }

    /// <summary>
    /// Obtém a contagem de pedidos armazenados
    /// </summary>
    public int Count => _orders.Count;
}
