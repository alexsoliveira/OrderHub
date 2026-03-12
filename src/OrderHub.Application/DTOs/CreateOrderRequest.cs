namespace OrderHub.Application.DTOs;

/// <summary>
/// DTO para requisição de criação de pedido
/// </summary>
public record CreateOrderRequest
{
    /// <summary>
    /// Identificador do cliente
    /// </summary>
    public required string CustomerId { get; init; }

    /// <summary>
    /// Itens do pedido
    /// </summary>
    public required List<OrderItemRequest> Items { get; init; }

    /// <summary>
    /// Descrição do pedido (opcional)
    /// </summary>
    public string? Description { get; init; }
}
