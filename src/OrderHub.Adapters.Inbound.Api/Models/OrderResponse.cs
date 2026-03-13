namespace OrderHub.Adapters.Inbound.Api.Models;

/// <summary>
/// Resposta padrão para pedidos
/// </summary>
public class OrderResponse
{
    /// <summary>
    /// ID único do pedido
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// ID do cliente
    /// </summary>
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Status atual do pedido
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Valor total do pedido
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Data de criação do pedido
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Itens do pedido
    /// </summary>
    public List<OrderItemResponse> Items { get; set; } = new();
}
