namespace OrderHub.Adapters.Inbound.Api.Models;

/// <summary>
/// DTO para requisição de atualização de pedido via API REST
/// </summary>
public record UpdateOrderRequest
{
    /// <summary>
    /// Identificador do cliente (opcional)
    /// </summary>
    public string? CustomerId { get; init; }

    /// <summary>
    /// Itens do pedido (opcional)
    /// </summary>
    public List<CreateOrderItemRequest>? Items { get; init; }

    /// <summary>
    /// Descrição do pedido (opcional)
    /// </summary>
    public string? Description { get; init; }
}
