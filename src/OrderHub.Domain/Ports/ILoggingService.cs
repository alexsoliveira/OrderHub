namespace OrderHub.Domain.Ports;

/// <summary>
/// Logging Service Port (Interface) - OPCIONAL
/// Abstração para logging de eventos e erros da aplicação
/// Permite diferentes implementações (Serilog, NLog, Console, etc)
/// sem acoplamento direto com bibliotecas específicas
/// Arquitetura Hexagonal: Port que o Domain precisa para logging externo
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// Log em nível DEBUG - Informações detalhadas para desenvolvimento
    /// Usado para rastrear execução e valores de variáveis
    /// </summary>
    /// <param name="message">Mensagem de log (pode conter placeholders {0}, {1}, etc)</param>
    /// <param name="args">Argumentos para formatação da mensagem</param>
    /// <example>
    /// logger.LogDebug("Starting order creation for customer {0}", customerId);
    /// </example>
    void LogDebug(string message, params object[] args);

    /// <summary>
    /// Log em nível INFORMATION - Eventos normais e operações importantes
    /// Usado para registrar fluxo de negócio bem-sucedido
    /// </summary>
    /// <param name="message">Mensagem de log (pode conter placeholders)</param>
    /// <param name="args">Argumentos para formatação da mensagem</param>
    /// <example>
    /// logger.LogInformation("Order {0} created successfully", orderId);
    /// </example>
    void LogInformation(string message, params object[] args);

    /// <summary>
    /// Log em nível WARNING - Situações anômalas ou potenciais problemas
    /// Usado quando algo inesperado ocorre mas a execução continua
    /// </summary>
    /// <param name="message">Mensagem de log (pode conter placeholders)</param>
    /// <param name="args">Argumentos para formatação da mensagem</param>
    /// <example>
    /// logger.LogWarning("Customer {0} has {1} orders, consider cleanup", customerId, count);
    /// </example>
    void LogWarning(string message, params object[] args);

    /// <summary>
    /// Log em nível ERROR - Erros e exceções da aplicação
    /// Usado quando operação falha ou exceção não esperada ocorre
    /// </summary>
    /// <param name="message">Mensagem descritiva do erro</param>
    /// <param name="exception">Exceção capturada (pode ser null)</param>
    /// <param name="args">Argumentos adicionais para contexto</param>
    /// <example>
    /// logger.LogError("Failed to create order for customer {0}", ex, customerId);
    /// </example>
    void LogError(string message, Exception? exception = null, params object[] args);
}
