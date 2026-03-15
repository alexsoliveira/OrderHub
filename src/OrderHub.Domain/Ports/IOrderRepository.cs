using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Domain.Ports;

/// <summary>
/// Output Port (Interface) para persistência de pedidos
/// Define o contrato que adapters de persistência devem implementar
/// Arquitetura Hexagonal: Port que o Domain precisa para interagir com sistemas externos
/// O Domain não conhece as implementações (banco de dados, cache, etc)
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Recupera um agregado Order pelo seu identificador
    /// </summary>
    /// <param name="orderId">Identificador do pedido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Agregado Order se encontrado, null caso contrário</returns>
    Task<Order?> GetByIdAsync(OrderId orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupera todos os pedidos de um cliente
    /// </summary>
    /// <param name="customerId">Identificador do cliente</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de agregados Order do cliente</returns>
    Task<List<Order>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persiste um novo pedido ou atualiza um existente
    /// Trabalha com o agregado Order completo
    /// </summary>
    /// <param name="order">Agregado Order para persistir</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task SaveAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um pedido pelo seu identificador
    /// </summary>
    /// <param name="orderId">Identificador do pedido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task DeleteAsync(OrderId orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um pedido existe
    /// </summary>
    /// <param name="orderId">Identificador do pedido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se existe, false caso contrário</returns>
    Task<bool> ExistsAsync(OrderId orderId, CancellationToken cancellationToken = default);
}
