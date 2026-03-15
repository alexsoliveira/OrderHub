using OrderHub.Application.DTOs;

namespace OrderHub.Application.UseCases;

/// <summary>
/// Use Case (Input Port) interface para listar todos os pedidos
/// Define o contrato para implementações do caso de uso de listagem de pedidos
/// Representa um port na arquitetura hexagonal - interface de entrada da aplicação
/// Permite recuperar todos os pedidos do sistema com suporte a paginação (futura)
/// </summary>
public interface IListOrdersUseCase
{
    /// <summary>
    /// Executa o caso de uso para recuperar todos os pedidos do sistema
    /// Pode ser expandido futuramente para suportar paginação, filtros e ordenação
    /// </summary>
    /// <param name="cancellationToken">Token para cancelamento assíncrono da operação</param>
    /// <returns>Lista com todos os pedidos do sistema</returns>
    Task<List<OrderResponse>> ExecuteAsync(CancellationToken cancellationToken = default);
}
