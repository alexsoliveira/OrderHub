namespace OrderHub.Application.DTOs;

/// <summary>
/// DTO para resposta de pedido
/// </summary>
public record OrderResponse
{
    /// <summary>
    /// Identificador único do pedido
    /// </summary>
    public required string OrderId { get; init; }

    /// <summary>
    /// Identificador do cliente
    /// </summary>
    public required string CustomerId { get; init; }

    /// <summary>
    /// Data de criação do pedido
    /// </summary>
    public required DateTime OrderDate { get; init; }

    /// <summary>
    /// Status atual do pedido
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// Itens do pedido
    /// </summary>
    public required List<OrderItemResponse> Items { get; init; }

    /// <summary>
    /// Valor total do pedido
    /// </summary>
    public required decimal TotalAmount { get; init; }

    /// <summary>
    /// Moeda do pedido
    /// </summary>
    public string Currency { get; init; } = "BRL";

    /// <summary>
    /// Descrição do pedido (opcional)
    /// </summary>
    public string? Description { get; init; }
}
