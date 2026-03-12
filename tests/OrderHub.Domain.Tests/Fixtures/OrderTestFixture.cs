using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Domain.Tests.Fixtures;

/// <summary>
/// Fixture com dados reutilizáveis para testes de Order
/// </summary>
public class OrderTestFixture : IDisposable
{
    public OrderId CreateOrderId(Guid? id = null)
    {
        return OrderId.Create(id ?? Guid.NewGuid());
    }

    public CustomerId CreateCustomerId(Guid? id = null)
    {
        return CustomerId.Create(id ?? Guid.NewGuid());
    }

    public OrderItem CreateOrderItem(string productName = "Produto Teste", int quantity = 1, decimal unitPrice = 100.00m)
    {
        return OrderItem.Create(productName, quantity, unitPrice);
    }

    public Order CreateOrder(OrderId? orderId = null, CustomerId? customerId = null, DateTime? orderDate = null)
    {
        var id = orderId ?? CreateOrderId();
        var customerId_ = customerId ?? CreateCustomerId();
        var date = orderDate ?? DateTime.UtcNow;

        return Order.CreateOrder(id, customerId_, date);
    }

    public Order CreateOrderWithItem(OrderId? orderId = null, CustomerId? customerId = null)
    {
        var order = CreateOrder(orderId, customerId);
        var item = CreateOrderItem();
        order.AddItem(item);
        return order;
    }

    public OrderAmount CreateOrderAmount(decimal value = 100.00m, string currency = "BRL")
    {
        return OrderAmount.Create(value, currency);
    }

    public void Dispose()
    {
        // Cleanup resources if needed
    }
}
