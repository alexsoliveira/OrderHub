namespace OrderHub.Application.Exceptions;

/// <summary>
/// Exceção lançada quando um pedido (Order) é solicitado mas não é encontrado no repositório.
/// 
/// Status HTTP esperado: 404 Not Found
/// Exemplo: Chamar GET /api/v1/orders/{orderId} com um ID inexistente
/// </summary>
public sealed class OrderNotFoundException : ApplicationException
{
    /// <summary>
    /// ID do pedido que não foi encontrado.
    /// </summary>
    public string OrderId { get; }

    /// <summary>
    /// Inicializa uma nova instância de OrderNotFoundException.
    /// </summary>
    /// <param name="orderId">ID do pedido não encontrado</param>
    public OrderNotFoundException(string orderId)
        : base(
            $"Pedido com ID '{orderId}' não encontrado",
            "ORDER_NOT_FOUND")
    {
        OrderId = orderId;
    }
}
