using OrderHub.Application.DTOs;
using OrderHub.Domain.Aggregates.Order;

namespace OrderHub.Application.Ports;

/// <summary>
/// Port (Interface) para persistência de pedidos
/// Define o contrato para implementações de repositório de orders
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Recupera um pedido pelo seu identificador
    /// </summary>
    Task<OrderResponse?> GetByIdAsync(string orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupera todos os pedidos de um cliente
    /// </summary>
    Task<List<OrderResponse>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persiste um novo pedido ou atualiza um existente
    /// </summary>
    Task SaveAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um pedido pelo seu identificador
    /// </summary>
    Task DeleteAsync(string orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um pedido existe
    /// </summary>
    Task<bool> ExistsAsync(string orderId, CancellationToken cancellationToken = default);
}
