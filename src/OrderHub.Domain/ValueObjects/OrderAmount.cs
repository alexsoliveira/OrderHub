namespace OrderHub.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um valor monetário (Amount) com moeda
/// Completamente imutável - peso não pode ser alterado após criação
/// </summary>
public class OrderAmount : IEquatable<OrderAmount>
{
    /// <summary>
    /// Valor monetário (decimal com 2 casas para precisão)
    /// </summary>
    public readonly decimal Value;

    /// <summary>
    /// Código da moeda (ex: BRL, USD, EUR)
    /// </summary>
    public readonly string Currency;

    private OrderAmount(decimal value, string currency)
    {
        Value = value;
        Currency = currency;
    }

    /// <summary>
    /// Factory method para criar uma nova instância de OrderAmount
    /// </summary>
    /// <param name="value">Valor monetário, deve ser maior que 0</param>
    /// <param name="currency">Código da moeda (padrão: BRL)</param>
    /// <returns>Nova instância de OrderAmount</returns>
    /// <exception cref="ArgumentException">Lançado quando valor é inválido</exception>
    public static OrderAmount Create(decimal value, string currency = "BRL")
    {
        if (value <= 0)
            throw new ArgumentException("Valor deve ser maior que zero", nameof(value));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Moeda não pode estar vazia", nameof(currency));

        if (currency.Length != 3)
            throw new ArgumentException("Código de moeda deve ter 3 caracteres", nameof(currency));

        // Arredondar para 2 casas decimais
        var roundedValue = Math.Round(value, 2);

        return new OrderAmount(roundedValue, currency.ToUpperInvariant());
    }

    /// <summary>
    /// Adiciona um valor a este amount (retorna novo OrderAmount, pois é imutável)
    /// </summary>
    public OrderAmount Add(OrderAmount other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (other.Currency != Currency)
            throw new InvalidOperationException(
                $"Não é possível adicionar valores em moedas diferentes: {Currency} e {other.Currency}");

        return Create(Value + other.Value, Currency);
    }

    /// <summary>
    /// Subtrai um valor deste amount (retorna novo OrderAmount, pois é imutável)
    /// </summary>
    public OrderAmount Subtract(OrderAmount other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (other.Currency != Currency)
            throw new InvalidOperationException(
                $"Não é possível subtrair valores em moedas diferentes: {Currency} e {other.Currency}");

        var result = Value - other.Value;

        if (result <= 0)
            throw new InvalidOperationException("Resultado da subtração não pode ser menor ou igual a zero");

        return Create(result, Currency);
    }

    /// <summary>
    /// Multiplica este amount por um fator
    /// </summary>
    public OrderAmount Multiply(decimal factor)
    {
        if (factor <= 0)
            throw new ArgumentException("Fator de multiplicação deve ser maior que zero", nameof(factor));

        return Create(Value * factor, Currency);
    }

    /// <summary>
    /// Compara igualdade por valor (comparação de valor objects)
    /// </summary>
    public bool Equals(OrderAmount? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value && Currency == other.Currency;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((OrderAmount)obj);
    }

    /// <summary>
    /// GetHashCode deve ser consistente com igualdade
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, Currency);
    }

    /// <summary>
    /// Formata o valor como moeda brasileira (ou da moeda especificada)
    /// </summary>
    public override string ToString()
    {
        // Formatação simplificada - pode ser expandida com locale-awareness
        return Currency switch
        {
            "BRL" => $"R$ {Value:F2}",
            "USD" => $"$ {Value:F2}",
            "EUR" => $"€ {Value:F2}",
            _ => $"{Currency} {Value:F2}"
        };
    }

    // Operadores para facilitate comparações
    public static bool operator ==(OrderAmount? left, OrderAmount? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(OrderAmount? left, OrderAmount? right)
    {
        return !Equals(left, right);
    }

    public static OrderAmount operator +(OrderAmount left, OrderAmount right)
    {
        return left.Add(right);
    }

    public static OrderAmount operator -(OrderAmount left, OrderAmount right)
    {
        return left.Subtract(right);
    }

    public static OrderAmount operator *(OrderAmount left, decimal factor)
    {
        return left.Multiply(factor);
    }

    public static OrderAmount operator *(decimal factor, OrderAmount right)
    {
        return right.Multiply(factor);
    }
}
