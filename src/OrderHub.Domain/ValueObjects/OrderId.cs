namespace OrderHub.Domain.ValueObjects;

/// <summary>
/// Value Object que representa um identificador único de pedido
/// </summary>
public class OrderId : IEquatable<OrderId>
{
    public Guid Value { get; }

    private OrderId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("OrderId não pode ser vazio", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Factory method para criar uma nova instância de OrderId
    /// </summary>
    public static OrderId Create(Guid? value = null)
    {
        var id = value ?? Guid.NewGuid();
        return new OrderId(id);
    }

    /// <summary>
    /// Factory method para criar OrderId a partir de string
    /// </summary>
    public static OrderId Parse(string value)
    {
        if (!Guid.TryParse(value, out var id))
            throw new ArgumentException("Formato de OrderId inválido", nameof(value));

        return new OrderId(id);
    }

    public bool Equals(OrderId? other)
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
        return Equals((OrderId)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString("D");
    }

    public static bool operator ==(OrderId? left, OrderId? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(OrderId? left, OrderId? right)
    {
        return !Equals(left, right);
    }
}
