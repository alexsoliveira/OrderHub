using OrderHub.Domain.ValueObjects;

namespace OrderHub.Domain.Aggregates.Order;

/// <summary>
/// Aggregate Root que representa um pedido no sistema
/// Encapsula toda a lógica e regras de negócio relacionadas a pedidos
/// </summary>
public class Order : AggregateRoot, IEquatable<Order>
{
    public OrderId OrderId { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    private readonly List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    private Order(OrderId orderId, CustomerId customerId, DateTime orderDate)
    {
        OrderId = orderId ?? throw new ArgumentNullException(nameof(orderId));
        CustomerId = customerId ?? throw new ArgumentNullException(nameof(customerId));
        OrderDate = orderDate;
        Status = OrderStatus.New;
    }

    /// <summary>
    /// Factory method para criar um novo pedido
    /// </summary>
    public static Order CreateOrder(OrderId orderId, CustomerId customerId, DateTime? orderDate = null)
    {
        if (orderId == null)
            throw new ArgumentNullException(nameof(orderId), "OrderId não pode ser nulo");

        if (customerId == null)
            throw new ArgumentNullException(nameof(customerId), "CustomerId não pode ser nulo");

        var order = new Order(orderId, customerId, orderDate ?? DateTime.UtcNow);
        return order;
    }

    /// <summary>
    /// Adiciona um item ao pedido, respeitando regras de negócio
    /// </summary>
    public void AddItem(OrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item), "Item não pode ser nulo");

        // Regra 1: Não pode adicionar itens a pedido que já foi enviado
        if (Status == OrderStatus.Shipped)
            throw new InvalidOperationException("Não é possível adicionar itens a um pedido que já foi enviado");

        // Regra 5: Não pode adicionar mais de 10 itens distintos no pedido
        if (_items.Count >= 10 && !_items.Contains(item))
            throw new InvalidOperationException("Não é possível adicionar mais de 10 itens distintos no pedido");

        // Se o item já existe, incrementar a quantidade
        var existingItem = _items.FirstOrDefault(i => i.Equals(item));
        if (existingItem != null)
        {
            _items.Remove(existingItem);
        }

        _items.Add(item);
    }

    /// <summary>
    /// Remove um item do pedido, respeitando regras de negócio
    /// </summary>
    public void RemoveItem(OrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item), "Item não pode ser nulo");

        // Regra 3: Não pode remover último item (impedindo pedido vazio)
        if (_items.Count == 1)
            throw new InvalidOperationException("Não é possível remover o último item do pedido");

        if (!_items.Remove(item))
            throw new InvalidOperationException("Item não encontrado no pedido");

        // Regra 4: Remover o último item automaticamente marca pedido como Cancelled
        if (_items.Count == 0)
        {
            Status = OrderStatus.Cancelled;
        }
    }

    /// <summary>
    /// Verifica se é possível adicionar um item
    /// </summary>
    public bool CanAddItem(OrderItem item)
    {
        if (item == null)
            return false;

        // Não pode adicionar a pedido enviado
        if (Status == OrderStatus.Shipped)
            return false;

        // Não pode ter mais de 10 itens distintos
        if (_items.Count >= 10 && !_items.Contains(item))
            return false;

        return true;
    }

    /// <summary>
    /// Verifica se é possível remover um item
    /// </summary>
    public bool CanRemoveItem(OrderItem item)
    {
        if (item == null)
            return false;

        // Não pode remover único item
        if (_items.Count == 1)
            return false;

        return _items.Contains(item);
    }

    /// <summary>
    /// Verifica se é possível fazer transição para um novo status
    /// </summary>
    public bool CanTransitionTo(OrderStatus newStatus)
    {
        // Transições permitidas:
        // New → Pending → Processing → Shipped → Delivered
        // Cancelled pode ser atingido de qualquer estado
        return newStatus switch
        {
            OrderStatus.Cancelled => true,
            OrderStatus.Pending => Status == OrderStatus.New,
            OrderStatus.Processing => Status == OrderStatus.Pending,
            OrderStatus.Shipped => Status == OrderStatus.Processing,
            OrderStatus.Delivered => Status == OrderStatus.Shipped,
            _ => false
        };
    }

    /// <summary>
    /// Muda o status do pedido com validação de transição
    /// </summary>
    public void ChangeStatus(OrderStatus newStatus)
    {
        if (!CanTransitionTo(newStatus))
            throw new InvalidOperationException($"Não é possível transicionar de {Status} para {newStatus}");

        Status = newStatus;
    }

    /// <summary>
    /// Calcula o total do pedido
    /// </summary>
    public decimal GetTotal()
    {
        return _items.Sum(item => item.GetSubtotal());
    }

    /// <summary>
    /// Verifica se o pedido tem itens
    /// </summary>
    public bool HasItems => _items.Count > 0;

    public bool Equals(Order? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return OrderId.Equals(other.OrderId);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Order)obj);
    }

    public override int GetHashCode()
    {
        return OrderId.GetHashCode();
    }

    public override string ToString()
    {
        return $"Pedido {OrderId} - Cliente: {CustomerId} - Status: {Status} - Total: R$ {GetTotal():F2}";
    }

    public static bool operator ==(Order? left, Order? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Order? left, Order? right)
    {
        return !Equals(left, right);
    }
}
