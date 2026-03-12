namespace OrderHub.Domain.ValueObjects;

/// <summary>
/// Value Object que representa o identificador único de um cliente
/// </summary>
public class CustomerId : IEquatable<CustomerId>
{
    public Guid Value { get; }

    private CustomerId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("CustomerId não pode ser vazio", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Factory method para criar uma nova instância de CustomerId
    /// </summary>
    public static CustomerId Create(Guid? value = null)
    {
        var id = value ?? Guid.NewGuid();
        return new CustomerId(id);
    }

    /// <summary>
    /// Factory method para criar CustomerId a partir de string
    /// </summary>
    public static CustomerId Parse(string value)
    {
        if (!Guid.TryParse(value, out var id))
            throw new ArgumentException("Formato de CustomerId inválido", nameof(value));

        return new CustomerId(id);
    }

    public bool Equals(CustomerId? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CustomerId)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString("D");
    }

    public static bool operator ==(CustomerId? left, CustomerId? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(CustomerId? left, CustomerId? right)
    {
        return !Equals(left, right);
    }
}
