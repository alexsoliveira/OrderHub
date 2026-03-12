using OrderHub.Domain.Exceptions;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Domain.Aggregates.Order;

/// <summary>
/// Aggregate Root que representa um pedido no sistema
/// Encapsula toda a lógica e regras de negócio relacionadas a pedidos
/// Implementa 6 regras fundamentais de negócio:
/// 1. Não pode adicionar itens a pedido que já foi enviado
/// 2. Pedido deve ter no mínimo 1 item
/// 3. Não pode remover último item (impedindo pedido vazio)
/// 4. Remover último item automaticamente marca com Cancelled
/// 5. Não pode adicionar mais de 10 itens distintos
/// 6. Transições de status validadas e controladas
/// </summary>
public class Order : AggregateRoot, IEquatable<Order>
{
    // Constantes de regras de negócio
    private const int MaximumDistinctItems = 10;
    private const int MinimumItems = 1;

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
            throw new InvalidOrderException("OrderId não pode ser nulo");

        if (customerId == null)
            throw new InvalidOrderException("CustomerId não pode ser nulo");

        var order = new Order(orderId, customerId, orderDate ?? DateTime.UtcNow);
        return order;
    }

    /// <summary>
    /// Adiciona um item ao pedido, respeitando 2 regras de negócio:
    /// REGRA 1: Não pode adicionar itens a pedido que já foi enviado (Status = Shipped)
    /// REGRA 5: Não pode adicionar mais de 10 itens distintos no pedido
    /// </summary>
    public void AddItem(OrderItem item)
    {
        if (item == null)
            throw new InvalidOrderException("Item não pode ser nulo");

        // REGRA 1: Não pode adicionar itens a pedido que já foi enviado
        if (Status == OrderStatus.Shipped)
            throw new InvalidOrderException("Não é possível adicionar itens a um pedido que já foi enviado");

        // REGRA 5: Não pode adicionar mais de 10 itens distintos no pedido
        if (_items.Count >= MaximumDistinctItems && !_items.Contains(item))
            throw new InvalidOrderException($"Não é possível adicionar mais de {MaximumDistinctItems} itens distintos no pedido");

        // Se o item já existe, substituir
        var existingItem = _items.FirstOrDefault(i => i.Equals(item));
        if (existingItem != null)
        {
            _items.Remove(existingItem);
        }

        _items.Add(item);
    }

    /// <summary>
    /// Remove um item do pedido, respeitando regras de negócio:
    /// REGRA 3: Não pode remover último item (impedindo pedido vazio)
    /// REGRA 4: Se ficar vazio, automaticamente marca pedido como Cancelled
    /// </summary>
    public void RemoveItem(OrderItem item)
    {
        if (item == null)
            throw new InvalidOrderException("Item não pode ser nulo");

        // REGRA 3: Não pode remover último item (impedindo pedido vazio)
        if (_items.Count <= MinimumItems)
            throw new InvalidOrderException("Não é possível remover o último item do pedido. Pedido deve ter no mínimo 1 item");

        if (!_items.Remove(item))
            throw new InvalidOrderException("Item não encontrado no pedido");

        // REGRA 4: Se ficar vazio, automaticamente marca como Cancelled
        if (_items.Count < MinimumItems)
        {
            Status = OrderStatus.Cancelled;
        }
    }

    /// <summary>
    /// Verifica se é possível adicionar um item respeitando regras de negócio
    /// </summary>
    public bool CanAddItem(OrderItem? item)
    {
        if (item == null)
            return false;

        // Não pode adicionar a pedido enviado
        if (Status == OrderStatus.Shipped)
            return false;

        // Não pode ter mais de 10 itens distintos
        if (_items.Count >= MaximumDistinctItems && !_items.Contains(item))
            return false;

        return true;
    }

    /// <summary>
    /// Verifica se é possível remover um item respeitando regras de negócio
    /// </summary>
    public bool CanRemoveItem(OrderItem? item)
    {
        if (item == null)
            return false;

        // Não pode remover se é o último item
        if (_items.Count <= MinimumItems)
            return false;

        return _items.Contains(item);
    }

    /// <summary>
    /// REGRA 6: Verifica se é possível fazer transição para um novo status
    /// Transições permitidas:
    /// - New → Pending → Processing → Shipped → Delivered
    /// - Cancelled pode ser atingido de qualquer estado
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
    /// REGRA 6: Muda o status do pedido com validação de transição
    /// Lança exceção se a transição não for permitida
    /// </summary>
    public void ChangeStatus(OrderStatus newStatus)
    {
        if (!CanTransitionTo(newStatus))
            throw new InvalidOrderException($"Transição de status inválida: não é possível passar de '{Status}' para '{newStatus}'");

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
    /// REGRA 2: Verifica se o pedido tem itens (deve ter no mínimo 1)
    /// </summary>
    public bool HasMinimumItems => _items.Count >= MinimumItems;

    /// <summary>
    /// Verifica se o pedido tem itens
    /// </summary>
    public bool HasItems => _items.Count > 0;

    /// <summary>
    /// Retorna a quantidade de itens distintos
    /// </summary>
    public int ItemCount => _items.Count;

    /// <summary>
    /// Valida todas as regras de negócio do pedido
    /// Lança InvalidOrderException se alguma regra for violada
    /// </summary>
    public void ValidateBusinessRules()
    {
        // REGRA 2: Pedido deve ter no mínimo 1 item
        if (!HasMinimumItems)
            throw new InvalidOrderException($"Pedido deve ter no mínimo {MinimumItems} item(ns)");

        // Validação de status
        if (Status == default)
            throw new InvalidOrderException("Status do pedido é inválido");
    }

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
