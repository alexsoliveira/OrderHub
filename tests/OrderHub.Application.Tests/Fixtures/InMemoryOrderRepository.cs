using OrderHub.Application.DTOs;
using OrderHub.Application.Mappers;
using OrderHub.Application.Ports;
using OrderHub.Domain.Aggregates.Order;

namespace OrderHub.Application.Tests.Fixtures;

/// <summary>
/// Repositório em memória para testes unitários
/// Implementa IOrderRepository armazenando pedidos em um dicionário na memória
/// </summary>
public class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<string, Order> _orders = new();
    private readonly OrderMapper _mapper = new();

    /// <summary>
    /// Recupera um pedido pelo seu identificador
    /// </summary>
    public Task<OrderResponse?> GetByIdAsync(string orderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return Task.FromResult<OrderResponse?>(null);

        var found = _orders.TryGetValue(orderId, out var order);
        if (!found)
            return Task.FromResult<OrderResponse?>(null);

        var response = _mapper.MapOrderToResponse(order);
        return Task.FromResult<OrderResponse?>(response);
    }

    /// <summary>
    /// Recupera todos os pedidos de um cliente
    /// </summary>
    public Task<List<OrderResponse>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return Task.FromResult(new List<OrderResponse>());

        var orders = _orders.Values
            .Where(o => o.CustomerId.Value.ToString() == customerId)
            .Select(o => _mapper.MapOrderToResponse(o))
            .ToList();

        return Task.FromResult(orders);
    }

    /// <summary>
    /// Persiste um novo pedido ou atualiza um existente
    /// </summary>
    public Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        _orders[order.Id.Value.ToString()] = order;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Remove um pedido pelo seu identificador
    /// </summary>
    public Task DeleteAsync(string orderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return Task.CompletedTask;

        _orders.Remove(orderId);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Verifica se um pedido existe
    /// </summary>
    public Task<bool> ExistsAsync(string orderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return Task.FromResult(false);

        var exists = _orders.ContainsKey(orderId);
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
