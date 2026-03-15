namespace OrderHub.Domain.Ports;

/// <summary>
/// Output Port (Interface) para notificações
/// Define o contrato para envio de notificações (emails, SMS, push, etc)
/// Arquitetura Hexagonal: Port que o Domain precisa para notificações externas
/// </summary>
public interface INotificationPort
{
    /// <summary>
    /// Envia confirmação de pedido criado para o cliente
    /// </summary>
    /// <param name="customerId">Identificador do cliente</param>
    /// <param name="orderId">Identificador do pedido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task SendOrderConfirmationAsync(string customerId, string orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia notificação de pedido aprovado
    /// </summary>
    /// <param name="customerId">Identificador do cliente</param>
    /// <param name="orderId">Identificador do pedido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task SendOrderApprovedAsync(string customerId, string orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia notificação de pedido enviado/despachado
    /// </summary>
    /// <param name="customerId">Identificador do cliente</param>
    /// <param name="orderId">Identificador do pedido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task SendOrderShippedAsync(string customerId, string orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia notificação de pedido entregue
    /// </summary>
    /// <param name="customerId">Identificador do cliente</param>
    /// <param name="orderId">Identificador do pedido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task SendOrderDeliveredAsync(string customerId, string orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia notificação de pedido cancelado
    /// </summary>
    /// <param name="customerId">Identificador do cliente</param>
    /// <param name="orderId">Identificador do pedido</param>
    /// <param name="reason">Motivo do cancelamento</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task SendOrderCancelledAsync(string customerId, string orderId, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Envia notificação customizada
    /// </summary>
    /// <param name="customerId">Identificador do cliente</param>
    /// <param name="subject">Assunto da notificação</param>
    /// <param name="message">Corpo da notificação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task SendCustomNotificationAsync(string customerId, string subject, string message, CancellationToken cancellationToken = default);
}
