namespace OrderHub.Application.Ports;

/// <summary>
/// Port (Interface) para processamento de pagamentos
/// Define e contrato para integrações com gateways de pagamento
/// </summary>
public interface IPaymentPort
{
    /// <summary>
    /// Valida se um cliente pode realizar um pagamento de um determinado valor
    /// </summary>
    Task<bool> ValidatePaymentAsync(string customerId, decimal amount, string currency = "BRL", CancellationToken cancellationToken = default);

    /// <summary>
    /// Processa um pagamento
    /// </summary>
    /// <returns>ID da transação de pagamento</returns>
    Task<string> ProcessPaymentAsync(string customerId, decimal amount, string currency = "BRL", CancellationToken cancellationToken = default);

    /// <summary>
    /// Reembolsa um pagamento realizado previamente
    /// </summary>
    Task<bool> RefundPaymentAsync(string paymentTransactionId, decimal amount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica o status de um pagamento
    /// </summary>
    Task<PaymentStatus> GetPaymentStatusAsync(string paymentTransactionId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Enum que representa o status de um pagamento
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Pagamento pendente de processamento
    /// </summary>
    Pending,

    /// <summary>
    /// Pagamento processado com sucesso
    /// </summary>
    Completed,

    /// <summary>
    /// Pagamento falhou
    /// </summary>
    Failed,

    /// <summary>
    /// Pagamento foi reembolsado
    /// </summary>
    Refunded,

    /// <summary>
    /// Pagamento foi cancelado
    /// </summary>
    Cancelled
}
