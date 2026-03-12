namespace OrderHub.Application.DTOs;

/// <summary>
/// DTO para requisição de atualização de pedido
/// </summary>
public record UpdateOrderRequest
{
    /// <summary>
    /// Identificador do pedido
    /// </summary>
    public required string OrderId { get; init; }

    /// <summary>
    /// Itens atualizados do pedido
    /// </summary>
    public required List<OrderItemRequest> Items { get; init; }

    /// <summary>
    /// Nova descrição (opcional)
    /// </summary>
    public string? Description { get; init; }
}
