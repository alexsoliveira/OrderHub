using OrderHub.Application.DTOs;

namespace OrderHub.Application.UseCases;

/// <summary>
/// Use Case (Input Port) interface para recuperar pedidos
/// Define o contrato para implementações do caso de uso de consulta de pedidos
/// Representa um port na arquitetura hexagonal - interface de entrada da aplicação
/// Suporta duas operações: busca por ID e busca por cliente
/// </summary>
public interface IGetOrderUseCase
{
    /// <summary>
    /// Executa o caso de uso para recuperar um pedido pelo seu identificador único
    /// </summary>
    /// <param name="orderId">Identificador único do pedido a ser recuperado</param>
    /// <param name="cancellationToken">Token para cancelamento assíncrono da operação</param>
    /// <returns>Dados do pedido encontrado (OrderId, CustomerId, Items, TotalPrice, CreatedAt, Status)</returns>
    /// <exception cref="ArgumentException">Se orderId estiver vazio ou nulo</exception>
    /// <exception cref="InvalidOperationException">Se pedido com ID fornecido não existir</exception>
    Task<OrderResponse> ExecuteAsync(string orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executa o caso de uso para recuperar todos os pedidos de um cliente específico
    /// Útil para operações de histórico, relatórios e visualização de pedidos do cliente
    /// </summary>
    /// <param name="customerId">Identificador único do cliente cujos pedidos serão recuperados</param>
    /// <param name="cancellationToken">Token para cancelamento assíncrono da operação</param>
    /// <returns>Lista com todos os pedidos do cliente (pode ser vazia se cliente não tem pedidos)</returns>
    /// <exception cref="ArgumentException">Se customerId estiver vazio ou nulo</exception>
    Task<List<OrderResponse>> GetByCustomerAsync(string customerId, CancellationToken cancellationToken = default);
}
