namespace OrderHub.Application.Exceptions;

/// <summary>
/// Exceção lançada quando dados de entrada (request) são inválidos ou incompletos.
/// 
/// Status HTTP esperado: 400 Bad Request
/// Exemplos:
/// - CustomerId vazio ou nulo em CreateOrderRequest
/// - Items vazio em CreateOrderRequest
/// - OrderAmount negativo
/// </summary>
public sealed class InvalidRequestException : ApplicationException
{
    /// <summary>
    /// Nome do campo que contém o valor inválido.
    /// </summary>
    public string FieldName { get; }

    /// <summary>
    /// Inicializa uma nova instância de InvalidRequestException.
    /// </summary>
    /// <param name="fieldName">Nome do campo com valor inválido</param>
    /// <param name="message">Descrição do erro</param>
    public InvalidRequestException(string fieldName, string message)
        : base($"Campo '{fieldName}': {message}", "INVALID_REQUEST")
    {
        FieldName = fieldName;
    }

    /// <summary>
    /// Factory method para criação com valor e validação.
    /// </summary>
    /// <param name="fieldName">Nome do campo</param>
    /// <param name="value">Valor inválido</param>
    /// <param name="reason">Motivo da inválidez</param>
    /// <returns>Nova instância com mensagem formatada</returns>
    public static InvalidRequestException CreateForNullField(string fieldName, string reason = "é obrigatório")
    {
        return new InvalidRequestException(fieldName, $"{reason}");
    }

    /// <summary>
    /// Factory method para criação de campo vazio.
    /// </summary>
    public static InvalidRequestException CreateForEmptyCollection(string fieldName)
    {
        return new InvalidRequestException(fieldName, "não pode estar vazio");
    }
}
