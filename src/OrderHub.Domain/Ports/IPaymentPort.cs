namespace OrderHub.Domain.Ports;

/// <summary>
/// Output Port (Interface) para processamento de pagamentos
/// Define o contrato para integrações com gateways de pagamento
/// Arquitetura Hexagonal: Port que o Domain precisa para processar pagamentos externos
/// </summary>
public interface IPaymentPort
{
    /// <summary>
    /// Valida se um cliente pode realizar um pagamento de um determinado valor
    /// </summary>
    /// <param name="customerId">Identificador do cliente</param>
    /// <param name="amount">Valor do pagamento</param>
    /// <param name="currency">Moeda (padrão: BRL)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se válido, false caso contrário</returns>
    Task<bool> ValidatePaymentAsync(string customerId, decimal amount, string currency = "BRL", CancellationToken cancellationToken = default);

    /// <summary>
    /// Processa um pagamento
    /// </summary>
    /// <param name="customerId">Identificador do cliente</param>
    /// <param name="amount">Valor do pagamento</param>
    /// <param name="currency">Moeda (padrão: BRL)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>ID da transação de pagamento</returns>
    Task<string> ProcessPaymentAsync(string customerId, decimal amount, string currency = "BRL", CancellationToken cancellationToken = default);

    /// <summary>
    /// Reembolsa um pagamento realizado previamente
    /// </summary>
    /// <param name="paymentTransactionId">ID da transação a reembolsar</param>
    /// <param name="amount">Valor a reembolsar</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se reembolso bem-sucedido, false caso contrário</returns>
    Task<bool> RefundPaymentAsync(string paymentTransactionId, decimal amount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica o status de um pagamento
    /// </summary>
    /// <param name="paymentTransactionId">ID da transação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Status atual do pagamento</returns>
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
