namespace OrderHub.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando ocorre violação de regra relacionada a OrderAmount
/// </summary>
public class InvalidOrderAmountException : DomainException
{
    public InvalidOrderAmountException(string message) : base(message) { }

    public InvalidOrderAmountException(string message, Exception? innerException) 
        : base(message, innerException) { }
}
