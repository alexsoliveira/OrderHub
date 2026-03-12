using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.Exceptions;
using OrderHub.Domain.Tests.Fixtures;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Domain.Tests.Aggregates;

public class OrderTests : IDisposable
{
    private readonly OrderTestFixture _fixture = new();

    #region Testes de Criação de Order

    [Fact]
    public void CreateOrder_WithValidData_ReturnsOrderEntity()
    {
        // Arrange
        var orderId = _fixture.CreateOrderId();
        var customerId = _fixture.CreateCustomerId();

        // Act
        var order = Order.CreateOrder(orderId, customerId);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(orderId, order.OrderId);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Equal(OrderStatus.New, order.Status);
        Assert.Empty(order.Items);
    }

    [Fact]
    public void CreateOrder_WithNullOrderId_ThrowsInvalidOrderException()
    {
        // Arrange
        var customerId = _fixture.CreateCustomerId();

        // Act & Assert
        Assert.Throws<InvalidOrderException>(() => Order.CreateOrder(null!, customerId));
    }

    [Fact]
    public void CreateOrder_WithNullCustomerId_ThrowsInvalidOrderException()
    {
        // Arrange
        var orderId = _fixture.CreateOrderId();

        // Act & Assert
        Assert.Throws<InvalidOrderException>(() => Order.CreateOrder(orderId, null!));
    }

    #endregion

    #region Testes de Adição de Itens

    [Fact]
    public void AddItem_WithValidItem_ItemAddedSuccessfully()
    {
        // Arrange
        var order = _fixture.CreateOrder();
        var item = _fixture.CreateOrderItem();

        // Act
        order.AddItem(item);

        // Assert
        Assert.Single(order.Items);
        Assert.Contains(item, order.Items);
    }

    [Fact]
    public void AddItem_ToShippedOrder_ThrowsInvalidOrderException()
    {
        // Arrange
        var order = _fixture.CreateOrderWithItem();
        order.ChangeStatus(OrderStatus.Pending);
        order.ChangeStatus(OrderStatus.Processing);
        order.ChangeStatus(OrderStatus.Shipped);
        var newItem = _fixture.CreateOrderItem("Produto 2");

        // Act & Assert
        Assert.Throws<InvalidOrderException>(() => order.AddItem(newItem));
    }

    [Fact]
    public void AddItem_Exceeding10Items_ThrowsInvalidOrderException()
    {
        // Arrange
        var order = _fixture.CreateOrder();
        for (int i = 1; i <= 10; i++)
        {
            var item = _fixture.CreateOrderItem($"Produto {i}", 1, 100m + i);
            order.AddItem(item);
        }

        var eleventhItem = _fixture.CreateOrderItem("Produto 11", 1, 111m);

        // Act & Assert
        Assert.Throws<InvalidOrderException>(() => order.AddItem(eleventhItem));
    }

    [Fact]
    public void AddItem_WithValidItem_UpdatesOrderTotal()
    {
        // Arrange
        var order = _fixture.CreateOrder();
        var item = _fixture.CreateOrderItem("Produto Teste", 2, 50m);

        // Act
        order.AddItem(item);
        var total = order.GetTotal();

        // Assert
        Assert.Equal(100m, total);
    }

    [Fact]
    public void AddItem_WithNullItem_ThrowsInvalidOrderException()
    {
        // Arrange
        var order = _fixture.CreateOrder();

        // Act & Assert
        Assert.Throws<InvalidOrderException>(() => order.AddItem(null!));
    }

    #endregion

    #region Testes de Remoção de Itens

    [Fact]
    public void RemoveItem_WithValidItem_ItemRemovedSuccessfully()
    {
        // Arrange
        var order = _fixture.CreateOrder();
        var item1 = _fixture.CreateOrderItem("Produto 1");
        var item2 = _fixture.CreateOrderItem("Produto 2", 1, 200m);
        order.AddItem(item1);
        order.AddItem(item2);

        // Act
        order.RemoveItem(item1);

        // Assert
        Assert.Single(order.Items);
        Assert.DoesNotContain(item1, order.Items);
        Assert.Contains(item2, order.Items);
    }

    [Fact]
    public void RemoveItem_LastItem_ThrowsInvalidOrderException()
    {
        // Arrange
        var order = _fixture.CreateOrderWithItem();

        // Act & Assert
        Assert.Throws<InvalidOrderException>(() => order.RemoveItem(order.Items.First()));
    }

    [Fact]
    public void CanRemoveItem_WithSingleItem_ReturnsFalse()
    {
        // Arrange
        var order = _fixture.CreateOrderWithItem();
        var item = order.Items.First();

        // Act
        var canRemove = order.CanRemoveItem(item);

        // Assert
        Assert.False(canRemove);
    }

    [Fact]
    public void RemoveItem_UpdatesOrderTotal()
    {
        // Arrange
        var order = _fixture.CreateOrder();
        var item1 = _fixture.CreateOrderItem("Produto 1", 1, 100m);
        var item2 = _fixture.CreateOrderItem("Produto 2", 1, 200m);
        order.AddItem(item1);
        order.AddItem(item2);

        // Act
        order.RemoveItem(item1);
        var total = order.GetTotal();

        // Assert
        Assert.Equal(200m, total);
    }

    [Fact]
    public void RemoveItem_WithNullItem_ThrowsInvalidOrderException()
    {
        // Arrange
        var order = _fixture.CreateOrderWithItem();

        // Act & Assert
        Assert.Throws<InvalidOrderException>(() => order.RemoveItem(null!));
    }

    #endregion

    #region Testes de OrderAmount

    [Fact]
    public void OrderAmount_Created_WithValidValue_Success()
    {
        // Arrange & Act
        var amount = _fixture.CreateOrderAmount(99.99m);

        // Assert
        Assert.NotNull(amount);
        Assert.Equal(99.99m, amount.Value);
        Assert.Equal("BRL", amount.Currency);
    }

    [Fact]
    public void OrderAmount_Created_WithInvalidValue_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderAmount.Create(-50m));
        Assert.Throws<ArgumentException>(() => OrderAmount.Create(0m));
    }

    [Fact]
    public void OrderAmount_Comparison_ReturnsTrueForEqualValues()
    {
        // Arrange
        var amount1 = _fixture.CreateOrderAmount(100m);
        var amount2 = _fixture.CreateOrderAmount(100m);

        // Act & Assert
        Assert.Equal(amount1, amount2);
        Assert.True(amount1 == amount2);
    }

    [Fact]
    public void OrderAmount_GetHashCode_SameForEqualValues()
    {
        // Arrange
        var amount1 = _fixture.CreateOrderAmount(100m);
        var amount2 = _fixture.CreateOrderAmount(100m);

        // Act & Assert
        Assert.Equal(amount1.GetHashCode(), amount2.GetHashCode());
    }

    [Fact]
    public void OrderAmount_ToString_FormattedCorrectly()
    {
        // Arrange
        var amount = _fixture.CreateOrderAmount(100m, "BRL");

        // Act
        var result = amount.ToString();

        // Assert
        Assert.Contains("R$", result);
        Assert.Contains("100", result);
    }

    #endregion

    #region Testes de Transições de Status

    [Fact]
    public void CanTransitionTo_ValidTransition_ReturnsTrue()
    {
        // Arrange
        var order = _fixture.CreateOrder();

        // Act & Assert
        Assert.True(order.CanTransitionTo(OrderStatus.Pending));
    }

    [Fact]
    public void CanTransitionTo_InvalidTransition_ReturnsFalse()
    {
        // Arrange
        var order = _fixture.CreateOrder();

        // Act & Assert
        Assert.False(order.CanTransitionTo(OrderStatus.Shipped));
        Assert.False(order.CanTransitionTo(OrderStatus.Processing));
    }

    [Fact]
    public void ChangeStatus_ToValidStatus_UpdatesStatus()
    {
        // Arrange
        var order = _fixture.CreateOrder();

        // Act
        order.ChangeStatus(OrderStatus.Pending);

        // Assert
        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    [Fact]
    public void ChangeStatus_ToInvalidStatus_ThrowsInvalidOrderException()
    {
        // Arrange
        var order = _fixture.CreateOrder();

        // Act & Assert
        Assert.Throws<InvalidOrderException>(() => order.ChangeStatus(OrderStatus.Shipped));
    }

    [Fact]
    public void ChangeStatus_ToAnyStatusFromAny_CancelledAlwaysValid()
    {
        // Arrange
        var order = _fixture.CreateOrder();

        // Act
        order.ChangeStatus(OrderStatus.Cancelled);

        // Assert
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    #endregion

    #region Testes de Regras de Negócio Combinadas

    [Fact]
    public void Order_FullLifecycle_TransitionsCorrectly()
    {
        // Arrange
        var order = _fixture.CreateOrderWithItem();

        // Act & Assert
        Assert.Equal(OrderStatus.New, order.Status);

        order.ChangeStatus(OrderStatus.Pending);
        Assert.Equal(OrderStatus.Pending, order.Status);

        order.ChangeStatus(OrderStatus.Processing);
        Assert.Equal(OrderStatus.Processing, order.Status);

        order.ChangeStatus(OrderStatus.Shipped);
        Assert.Equal(OrderStatus.Shipped, order.Status);

        order.ChangeStatus(OrderStatus.Delivered);
        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    [Fact]
    public void Order_AddMultipleItems_ThenRemoveOne_Success()
    {
        // Arrange
        var order = _fixture.CreateOrder();
        var item1 = _fixture.CreateOrderItem("Produto 1");
        var item2 = _fixture.CreateOrderItem("Produto 2", 1, 200m);
        order.AddItem(item1);
        order.AddItem(item2);

        // Act
        Assert.Equal(2, order.ItemCount);
        order.RemoveItem(item2);

        // Assert
        Assert.Single(order.Items);
        Assert.Equal(100m, order.GetTotal());
    }

    [Fact]
    public void Order_CanAddItem_ChecksAllRules()
    {
        // Arrange
        var order = _fixture.CreateOrderWithItem();

        // Act
        var canAdd = order.CanAddItem(_fixture.CreateOrderItem());

        // Assert
        Assert.True(canAdd);

        // Now make it shipped
        order.ChangeStatus(OrderStatus.Pending);
        order.ChangeStatus(OrderStatus.Processing);
        order.ChangeStatus(OrderStatus.Shipped);

        // Act
        var canAddToShipped = order.CanAddItem(_fixture.CreateOrderItem("Produto X"));

        // Assert
        Assert.False(canAddToShipped);
    }

    #endregion

    public void Dispose()
    {
        _fixture?.Dispose();
    }
}
