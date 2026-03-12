namespace OrderHub.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando ocorre violação de regra relacionada a Order
/// </summary>
public class InvalidOrderException : DomainException
{
    public InvalidOrderException(string message) : base(message) { }

    public InvalidOrderException(string message, Exception? innerException) 
        : base(message, innerException) { }
}
