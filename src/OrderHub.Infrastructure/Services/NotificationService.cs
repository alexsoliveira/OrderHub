using OrderHub.Application.Ports;
using Microsoft.Extensions.Logging;

namespace OrderHub.Infrastructure.Services;

/// <summary>
/// Implementação de notificações (Stub implementation)
/// Em produção, seria integrada com serviços de email, SMS, push notifications, etc.
/// Por enquanto, apenas loga as notificações
/// </summary>
public class NotificationService : INotificationPort
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Envia confirmação de pedido criado
    /// </summary>
    public async Task SendOrderConfirmationAsync(
        string customerId,
        string orderId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(orderId);

        // Simular envio de notificação
        _logger.LogInformation(
            "Enviando confirmação de pedido: CustomerId={CustomerId}, OrderId={OrderId}",
            customerId, orderId);

        await Task.Delay(100, cancellationToken); // Simular latência
    }

    /// <summary>
    /// Envia notificação de pedido aprovado
    /// </summary>
    public async Task SendOrderApprovedAsync(
        string customerId,
        string orderId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(orderId);

        _logger.LogInformation(
            "Enviando notificação de pedido aprovado: CustomerId={CustomerId}, OrderId={OrderId}",
            customerId, orderId);

        await Task.Delay(100, cancellationToken);
    }

    /// <summary>
    /// Envia notificação de pedido enviado/despachado
    /// </summary>
    public async Task SendOrderShippedAsync(
        string customerId,
        string orderId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(orderId);

        _logger.LogInformation(
            "Enviando notificação de pedido enviado: CustomerId={CustomerId}, OrderId={OrderId}",
            customerId, orderId);

        await Task.Delay(100, cancellationToken);
    }

    /// <summary>
    /// Envia notificação de pedido entregue
    /// </summary>
    public async Task SendOrderDeliveredAsync(
        string customerId,
        string orderId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(orderId);

        _logger.LogInformation(
            "Enviando notificação de pedido entregue: CustomerId={CustomerId}, OrderId={OrderId}",
            customerId, orderId);

        await Task.Delay(100, cancellationToken);
    }

    /// <summary>
    /// Envia notificação de pedido cancelado
    /// </summary>
    public async Task SendOrderCancelledAsync(
        string customerId,
        string orderId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(orderId);

        _logger.LogInformation(
            "Enviando notificação de pedido cancelado: CustomerId={CustomerId}, OrderId={OrderId}, Reason={Reason}",
            customerId, orderId, reason);

        await Task.Delay(100, cancellationToken);
    }

    /// <summary>
    /// Envia notificação customizada
    /// </summary>
    public async Task SendCustomNotificationAsync(
        string customerId,
        string subject,
        string message,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        _logger.LogInformation(
            "Enviando notificação customizada: CustomerId={CustomerId}, Subject={Subject}, Message={Message}",
            customerId, subject, message);

        await Task.Delay(100, cancellationToken);
    }
}
