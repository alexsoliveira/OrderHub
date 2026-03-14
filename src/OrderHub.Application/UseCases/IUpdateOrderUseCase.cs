using OrderHub.Application.DTOs;

namespace OrderHub.Application.UseCases;

/// <summary>
/// Use Case (Input Port) interface para atualizar um pedido
/// Define o contrato para implementações do caso de uso de atualização de pedidos
/// Representa um port na arquitetura hexagonal - interface de entrada da aplicação
/// Permite modificar itens, descrição e outros dados de um pedido existente
/// </summary>
public interface IUpdateOrderUseCase
{
    /// <summary>
    /// Executa o caso de uso para atualizar um pedido existente
    /// Valida regras de negócio (status, quantidade de itens, etc) antes de atualizar
    /// </summary>
    /// <param name="request">Dados de atualização do pedido (OrderId, Items, Description)</param>
    /// <param name="cancellationToken">Token para cancelamento assíncrono da operação</param>
    /// <returns>Response com os dados atualizados do pedido</returns>
    /// <exception cref="ArgumentNullException">Se request é nulo</exception>
    /// <exception cref="ArgumentException">Se request inválido (OrderId vazio, Items vazios)</exception>
    /// <exception cref="InvalidOperationException">Se pedido não existe ou está em status que não permite atualização</exception>
    Task<OrderResponse> ExecuteAsync(UpdateOrderRequest request, CancellationToken cancellationToken = default);
}
