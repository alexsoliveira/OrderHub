using System.Text.RegularExpressions;
using OrderHub.Domain.Exceptions;

namespace OrderHub.Domain;

/// <summary>
/// Validador centralizado para regras de domínio
/// Fornece métodos estáticos para validações comuns com mensagens em português
/// </summary>
public static class DomainValidator
{
    /// <summary>
    /// Valida se um valor é nulo e lança exceção se for
    /// </summary>
    public static void ThrowIfNull<T>(T? value, string message) where T : class
    {
        if (value == null)
            throw new DomainException(message);
    }

    /// <summary>
    /// Valida se um valor decimal é negativo ou zero
    /// </summary>
    public static void ThrowIfNegativeOrZero(decimal value, string message)
    {
        if (value <= 0)
            throw new DomainException(message);
    }

    /// <summary>
    /// Valida se um valor decimal é negativo
    /// </summary>
    public static void ThrowIfNegative(decimal value, string message)
    {
        if (value < 0)
            throw new DomainException(message);
    }

    /// <summary>
    /// Valida se uma string está vazia ou nula
    /// </summary>
    public static void ThrowIfEmpty(string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(message);
    }

    /// <summary>
    /// Valida se uma string excede um tamanho máximo
    /// </summary>
    public static void ThrowIfExceedsLength(string? value, int maxLength, string message)
    {
        if (value == null)
            return;

        if (value.Length > maxLength)
            throw new DomainException(message);
    }

    /// <summary>
    /// Valida se um email está em formato correto
    /// </summary>
    public static void ThrowIfInvalidEmail(string? email, string message)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email não pode estar vazio");

        // Regex simples para validação de email
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailPattern))
            throw new DomainException(message);
    }

    /// <summary>
    /// Valida se um valor está dentro de um intervalo
    /// </summary>
    public static void ThrowIfNotInRange(decimal value, decimal min, decimal max, string message)
    {
        if (value < min || value > max)
            throw new DomainException(message);
    }

    /// <summary>
    /// Valida se um Guid está vazio
    /// </summary>
    public static void ThrowIfEmpty(Guid value, string message)
    {
        if (value == Guid.Empty)
            throw new DomainException(message);
    }

    /// <summary>
    /// Valida se uma condição é verdadeira, caso contrário lança exceção
    /// </summary>
    public static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new DomainException(message);
    }

    /// <summary>
    /// Valida se uma condição é falsa, caso contrário lança exceção
    /// </summary>
    public static void ThrowIfNot(bool condition, string message)
    {
        if (!condition)
            throw new DomainException(message);
    }

    /// <summary>
    /// Valida um valor com uma função de validação customizada
    /// </summary>
    public static void ThrowIfInvalid<T>(T value, Func<T, bool> isValid, string message)
    {
        if (!isValid(value))
            throw new DomainException(message);
    }
}
