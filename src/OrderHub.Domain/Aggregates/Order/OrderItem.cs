namespace OrderHub.Domain.Aggregates.Order;

/// <summary>
/// Entidade que representa um item dentro de um pedido
/// </summary>
public class OrderItem : IEquatable<OrderItem>
{
    public Guid Id { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    private OrderItem(Guid id, string productName, int quantity, decimal unitPrice)
    {
        Id = id;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    /// <summary>
    /// Factory method para criar um novo item de pedido
    /// </summary>
    public static OrderItem Create(string productName, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Nome do produto não pode estar vazio", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantity));

        if (unitPrice <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(unitPrice));

        return new OrderItem(Guid.NewGuid(), productName, quantity, unitPrice);
    }

    /// <summary>
    /// Calcula o subtotal deste item (quantidade × preço unitário)
    /// </summary>
    public decimal GetSubtotal() => Quantity * UnitPrice;

    public bool Equals(OrderItem? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((OrderItem)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
        return $"{ProductName} - Qtd: {Quantity}, Preço: R$ {UnitPrice:F2}";
    }

    public static bool operator ==(OrderItem? left, OrderItem? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(OrderItem? left, OrderItem? right)
    {
        return !Equals(left, right);
    }
}
