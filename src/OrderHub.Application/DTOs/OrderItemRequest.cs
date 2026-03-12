namespace OrderHub.Application.DTOs;

/// <summary>
/// DTO para item de pedido em requisição
/// </summary>
public record OrderItemRequest
{
    /// <summary>
    /// Identificador do produto
    /// </summary>
    public required string ProductId { get; init; }

    /// <summary>
    /// Quantidade do produto
    /// </summary>
    public required int Quantity { get; init; }

    /// <summary>
    /// Preço unitário do produto
    /// </summary>
    public required decimal UnitPrice { get; init; }
}
