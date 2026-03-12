namespace OrderHub.Application.DTOs;

/// <summary>
/// DTO para item de pedido em resposta
/// </summary>
public record OrderItemResponse
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

    /// <summary>
    /// Subtotal do item (Quantity × UnitPrice)
    /// </summary>
    public required decimal SubTotal { get; init; }
}
