namespace OrderHub.Application.Exceptions;

/// <summary>
/// Exceção base para erros da Application Layer.
/// Representa falhas lógicas em casos de uso (não violações de regras de negócio do Domain).
/// 
/// Diferença com DomainException:
/// - DomainException: Violação de regra de negócio (ex: Order com CustomerId inválido)
/// - ApplicationException: Erro na lógica da aplicação (ex: Pedido não encontrado no banco)
/// </summary>
public abstract class ApplicationException : Exception
{
    /// <summary>
    /// Código de erro específico da aplicação.
    /// Usado para logging estruturado e tratamento em Controllers.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Inicializa uma nova instância da ApplicationException.
    /// </summary>
    /// <param name="message">Mensagem de erro descritiva</param>
    /// <param name="errorCode">Código de erro estruturado (ex: ORDER_NOT_FOUND)</param>
    /// <param name="innerException">Exceção interna (quando applicable)</param>
    protected ApplicationException(
        string message,
        string errorCode,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
