namespace OrderHub.Application.Exceptions;

/// <summary>
/// Exceção lançada quando uma operação viola o estado esperado de um pedido.
/// 
/// Status HTTP esperado: 409 Conflict
/// Exemplos:
/// - Tentar cancelar um pedido que já foi entregue
/// - Tentar atualizar um pedido em estado de erro
/// - Transição de estado inválida
/// </summary>
public sealed class InvalidOrderStateException : ApplicationException
{
    /// <summary>
    /// Inicializa uma nova instância de InvalidOrderStateException.
    /// </summary>
    /// <param name="message">Descrição da violação de estado</param>
    public InvalidOrderStateException(string message)
        : base(message, "INVALID_ORDER_STATE")
    {
    }

    /// <summary>
    /// Factory method para criação com detalhes de estado.
    /// </summary>
    /// <param name="orderId">ID do pedido</param>
    /// <param name="currentState">Estado atual do pedido</param>
    /// <param name="attemptedOperation">Operação que foi tentada</param>
    /// <returns>Nova instância com mensagem detalhada</returns>
    public static InvalidOrderStateException CreateForInvalidTransition(
        string orderId,
        string currentState,
        string attemptedOperation)
    {
        var message = $"Não é possível '{attemptedOperation}' um pedido no estado '{currentState}'. " +
                      $"Pedido ID: {orderId}";
        return new InvalidOrderStateException(message);
    }
}
