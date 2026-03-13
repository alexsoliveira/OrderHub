namespace OrderHub.Adapters.Inbound.Api.Models;

/// <summary>
/// DTO para item de pedido via API REST
/// </summary>
public record CreateOrderItemRequest
{
    /// <summary>
    /// Identificador do produto
    /// </summary>
    public required string ProductId { get; init; }

    /// <summary>
    /// Quantidade
    /// </summary>
    public required int Quantity { get; init; }

    /// <summary>
    /// Preço unitário
    /// </summary>
    public required decimal UnitPrice { get; init; }
}
