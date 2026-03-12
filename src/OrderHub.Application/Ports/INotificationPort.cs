namespace OrderHub.Application.Ports;

/// <summary>
/// Port (Interface) para notificações
/// Define o contrato para envio de notificações (emails, SMS, push, etc)
/// </summary>
public interface INotificationPort
{
    /// <summary>
    /// Envia confirmação de pedido criado para o cliente
    /// </summary>
    Task SendOrderConfirmationAsync(string customerId, string orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia notificação de pedido aprovado
    /// </summary>
    Task SendOrderApprovedAsync(string customerId, string orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia notificação de pedido enviado/despachado
    /// </summary>
    Task SendOrderShippedAsync(string customerId, string orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia notificação de pedido entregue
    /// </summary>
    Task SendOrderDeliveredAsync(string customerId, string orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia notificação de pedido cancelado
    /// </summary>
    Task SendOrderCancelledAsync(string customerId, string orderId, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia notificação customizada
    /// </summary>
    Task SendCustomNotificationAsync(string customerId, string subject, string message, CancellationToken cancellationToken = default);
}
