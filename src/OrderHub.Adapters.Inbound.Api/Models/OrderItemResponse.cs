namespace OrderHub.Adapters.Inbound.Api.Models;

/// <summary>
/// Resposta de um item de pedido
/// </summary>
public class OrderItemResponse
{
    /// <summary>
    /// ID do produto
    /// </summary>
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// Quantidade do item
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Preço unitário
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Preço total (Quantity * UnitPrice)
    /// </summary>
    public decimal TotalPrice { get; set; }
}
