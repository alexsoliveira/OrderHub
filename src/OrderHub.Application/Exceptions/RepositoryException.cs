namespace OrderHub.Application.Exceptions;

/// <summary>
/// Exceção lançada quando uma operação de repositório (banco de dados) falha.
/// 
/// Status HTTP esperado: 500 Internal Server Error
/// Exemplos:
/// - Falha ao salvar pedido no banco
/// - Falha ao buscar pedido no banco
/// - Erro de conexão com banco de dados
/// - Violação de constraint no banco
/// </summary>
public sealed class RepositoryException : ApplicationException
{
    /// <summary>
    /// Operação que falhou (ex: "Save", "GetById", "Delete").
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Inicializa uma nova instância de RepositoryException.
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="operation">Operação que falhou</param>
    /// <param name="innerException">Exceção interna (geralmente do EF Core)</param>
    public RepositoryException(
        string message,
        string operation = "Unknown",
        Exception? innerException = null)
        : base(
            $"Erro ao {operation}: {message}",
            "REPOSITORY_ERROR",
            innerException)
    {
        Operation = operation;
    }

    /// <summary>
    /// Factory method para criação de erro de save.
    /// </summary>
    public static RepositoryException CreateForSave(string orderId, Exception innerException)
    {
        var message = $"Falha ao salvar pedido (ID: {orderId}) no banco de dados";
        return new RepositoryException(message, "Save", innerException);
    }

    /// <summary>
    /// Factory method para criação de erro de get.
    /// </summary>
    public static RepositoryException CreateForGetById(string orderId, Exception innerException)
    {
        var message = $"Falha ao buscar pedido (ID: {orderId}) no banco de dados";
        return new RepositoryException(message, "GetById", innerException);
    }

    /// <summary>
    /// Factory method para criação de erro de delete.
    /// </summary>
    public static RepositoryException CreateForDelete(string orderId, Exception innerException)
    {
        var message = $"Falha ao deletar pedido (ID: {orderId}) do banco de dados";
        return new RepositoryException(message, "Delete", innerException);
    }
}
