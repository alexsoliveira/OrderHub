using OrderHub.Application.DTOs;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.Ports;

namespace OrderHub.Application.Ports;

/// <summary>
/// Application-level Port (Interface) para persistência de pedidos
/// Estende Domain.Ports.IOrderRepository para adicionar operações que retornam DTOs
/// </summary>
public interface IOrderRepository : Domain.Ports.IOrderRepository
{
    /// <summary>
    /// Recupera um pedido pelo seu identificador com retorno em DTO
    /// Versão Application-layer que retorna OrderResponse para a camada de aplicação
    /// </summary>
    /// <param name="orderId">ID do pedido em formato string (GUID)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>OrderResponse DTO ou null se não encontrado</returns>
    Task<OrderResponse?> GetByIdAsync(string orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupera todos os pedidos de um cliente com retorno em DTOs
    /// Versão Application-layer que retorna  OrderResponse DTOs para a camada de aplicação
    /// </summary>
    /// <param name="customerId">ID do cliente em formato string (GUID)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de OrderResponse DTOs do cliente</returns>
    Task<List<OrderResponse>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);
}
