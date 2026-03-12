namespace OrderHub.Domain.ValueObjects;

/// <summary>
/// Value Object que representa o identificador único de um produto
/// </summary>
public class ProductId : IEquatable<ProductId>
{
    /// <summary>
    /// Obtém o valor do identificador do produto
    /// </summary>
    public string Value { get; }

    private ProductId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("ProductId não pode ser vazio", nameof(value));

        Value = value.Trim();
    }

    /// <summary>
    /// Factory method para criar um novo ProductId
    /// </summary>
    public static ProductId Create(string value)
    {
        return new ProductId(value);
    }

    /// <summary>
    /// Factory method para gerar um novo ProductId único
    /// </summary>
    public static ProductId CreateNew()
    {
        return new ProductId($"PROD-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}");
    }

    public override bool Equals(object? obj)
    {
        return obj is ProductId other && Equals(other);
    }

    public bool Equals(ProductId? other)
    {
        return other != null && Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(ProductId? left, ProductId? right)
    {
        return left?.Equals(right) ?? right is null;
    }

    public static bool operator !=(ProductId? left, ProductId? right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return Value;
    }
}
