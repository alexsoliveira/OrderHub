using OrderHub.Application.DTOs;

namespace OrderHub.Application.UseCases;

/// <summary>
/// Use Case (Input Port) interface para criar um novo pedido
/// Define o contrato para implementações do caso de uso de criação de pedido
/// Representa um port na arquitetura hexagonal - interface de entrada da aplicação
/// </summary>
public interface ICreateOrderUseCase
{
    /// <summary>
    /// Executa o caso de uso para criar um novo pedido
    /// Orquestra a validação, criação da entidade de domínio, persistência e notificação
    /// </summary>
    /// <param name="request">Dados de entrada para criação do pedido (CustomerId, Items)</param>
    /// <param name="cancellationToken">Token para cancelamento assíncrono da operação</param>
    /// <returns>Resposta com os dados do pedido criado (OrderId, Items, TotalPrice, CreatedAt)</returns>
    /// <exception cref="ArgumentNullException">Se request é nulo</exception>
    /// <exception cref="ArgumentException">Se request inválido (CustomerId vazio ou sem Items)</exception>
    Task<OrderResponse> ExecuteAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
}
