namespace OrderHub.Domain.ValueObjects;

/// <summary>
/// Enum que representa os possíveis estados de um pedido
/// </summary>
public enum OrderStatus
{
    /// <summary>Estado novo/inicial do pedido</summary>
    New = 0,

    /// <summary>Pedido pendente de processamento</summary>
    Pending = 1,

    /// <summary>Pedido em processamento</summary>
    Processing = 2,

    /// <summary>Pedido foi enviado</summary>
    Shipped = 3,

    /// <summary>Pedido foi entregue</summary>
    Delivered = 4,

    /// <summary>Pedido foi cancelado</summary>
    Cancelled = 5
}
