using OrderHub.Domain.ValueObjects;

namespace OrderHub.Domain.Aggregates.Order;

/// <summary>
/// Entidade que representa um item dentro de um pedido
/// </summary>
public class OrderItem : IEquatable<OrderItem>
{
    public Guid Id { get; private set; }
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public OrderAmount Amount { get; private set; }

    public OrderItem(ProductId productId, int quantity, OrderAmount amount)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantity));

        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        Quantity = quantity;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Id = Guid.NewGuid();
    }

    private OrderItem(Guid id, ProductId productId, int quantity, OrderAmount amount)
    {
        Id = id;
        ProductId = productId;
        Quantity = quantity;
        Amount = amount;
    }

    /// <summary>
    /// Factory method para criar um novo item de pedido (mantido para compatibilidade)
    /// </summary>
    public static OrderItem Create(string productName, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Nome do produto não pode estar vazio", nameof(productName));

        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantity));

        if (unitPrice <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(unitPrice));

        var productId = ProductId.Create(productName);
        var amount = OrderAmount.Create(unitPrice);
        return new OrderItem(productId, quantity, amount);
    }

    /// <summary>
    /// Calcula o subtotal deste item (quantidade × preço unitário)
    /// </summary>
    public decimal GetSubtotal() => Quantity * Amount.Value;

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
        return $"{ProductId.Value} - Qtd: {Quantity}, Preço: {Amount}";
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
