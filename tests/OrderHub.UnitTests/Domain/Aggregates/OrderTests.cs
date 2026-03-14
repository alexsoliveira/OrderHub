using FluentAssertions;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.Exceptions;
using OrderHub.Domain.ValueObjects;
using Xunit;

namespace OrderHub.UnitTests.Domain.Aggregates;

/// <summary>
/// Unit tests for the Order aggregate root.
/// Tests validate business rules, state changes, and domain invariants.
/// Following DDD principles - tests focus on domain behavior, not implementation.
/// </summary>
public class OrderTests
{
    /// <summary>
    /// Test: Creating an Order with valid data should succeed.
    /// Scenario: Valid OrderId and CustomerId
    /// Expected: Order created successfully with New status
    /// </summary>
    [Fact]
    public void CreateOrder_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var orderId = OrderId.Create();
        var customerId = CustomerId.Create();

        // Act
        var order = Order.CreateOrder(orderId, customerId);

        // Assert
        order.Should().NotBeNull();
        order.OrderId.Should().Be(orderId);
        order.CustomerId.Should().Be(customerId);
        order.Status.Should().Be(OrderStatus.New);
        order.Items.Should().BeEmpty();
    }

    /// <summary>
    /// Test: Creating an Order with null OrderId should throw InvalidOrderException.
    /// Scenario: OrderId is null
    /// Expected: InvalidOrderException thrown
    /// </summary>
    [Fact]
    public void CreateOrder_WithNullOrderId_ShouldThrowInvalidOrderException()
    {
        // Arrange
        var customerId = CustomerId.Create();

        // Act & Assert
        var act = () => Order.CreateOrder(null!, customerId);
        act.Should().Throw<InvalidOrderException>()
            .WithMessage("*OrderId*");
    }

    /// <summary>
    /// Test: Creating an Order with null CustomerId should throw InvalidOrderException.
    /// Scenario: CustomerId is null
    /// Expected: InvalidOrderException thrown
    /// </summary>
    [Fact]
    public void CreateOrder_WithNullCustomerId_ShouldThrowInvalidOrderException()
    {
        // Arrange
        var orderId = OrderId.Create();

        // Act & Assert
        var act = () => Order.CreateOrder(orderId, null!);
        act.Should().Throw<InvalidOrderException>()
            .WithMessage("*CustomerId*");
    }

    /// <summary>
    /// Test: Adding an item to Order should succeed.
    /// Scenario: Order is in New status and item is valid
    /// Expected: Item added successfully to order
    /// </summary>
    [Fact]
    public void AddItem_WithValidItem_ShouldAddSuccessfully()
    {
        // Arrange
        var orderId = OrderId.Create();
        var customerId = CustomerId.Create();
        var order = Order.CreateOrder(orderId, customerId);
        var item = OrderItem.Create("Product 1", 2, 100.00m);

        // Act
        order.AddItem(item);

        // Assert
        order.Items.Should().HaveCount(1);
        order.Items.First().Should().Be(item);
    }

    /// <summary>
    /// Test: Adding null item to Order should throw InvalidOrderException.
    /// Scenario: Item parameter is null
    /// Expected: InvalidOrderException thrown
    /// </summary>
    [Fact]
    public void AddItem_WithNullItem_ShouldThrowInvalidOrderException()
    {
        // Arrange
        var orderId = OrderId.Create();
        var customerId = CustomerId.Create();
        var order = Order.CreateOrder(orderId, customerId);

        // Act & Assert
        var act = () => order.AddItem(null!);
        act.Should().Throw<InvalidOrderException>();
    }

    /// <summary>
    /// Test: Adding more than 10 distinct items should throw InvalidOrderException.
    /// Business Rule: REGRA 5 - Não pode adicionar mais de 10 itens distintos
    /// Scenario: Order tries to exceed 10 distinct items
    /// Expected: InvalidOrderException thrown
    /// </summary>
    [Theory]
    [InlineData(11)]
    public void AddItem_ExceedingMaximumDistinct_ShouldThrowInvalidOrderException(int itemCount)
    {
        // Arrange
        var orderId = OrderId.Create();
        var customerId = CustomerId.Create();
        var order = Order.CreateOrder(orderId, customerId);

        // Add items up to maximum
        for (int i = 0; i < itemCount; i++)
        {
            var item = OrderItem.Create($"Product {i}", 1, 100.00m);
            
            if (i < 10)
            {
                order.AddItem(item);
            }
            else
            {
                // Act & Assert - 11th item should throw
                var act = () => order.AddItem(item);
                act.Should().Throw<InvalidOrderException>();
            }
        }

        order.Items.Should().HaveCount(10);
    }

    /// <summary>
    /// Test: Removing last item from Order should throw InvalidOrderException.
    /// Business Rule: REGRA 3 - Não pode remover último item (impedindo pedido vazio)
    /// Scenario: Order has 1 item, trying to remove it
    /// Expected: InvalidOrderException thrown
    /// </summary>
    [Fact]
    public void RemoveItem_WhenOnlyItemRemains_ShouldThrowInvalidOrderException()
    {
        // Arrange
        var orderId = OrderId.Create();
        var customerId = CustomerId.Create();
        var order = Order.CreateOrder(orderId, customerId);
        var item = OrderItem.Create("Product 1", 1, 100.00m);
        order.AddItem(item);

        // Act & Assert
        var act = () => order.RemoveItem(item);
        act.Should().Throw<InvalidOrderException>()
            .WithMessage("*último item*");
    }

    /// <summary>
    /// Test: Removing null item should throw InvalidOrderException.
    /// Scenario: RemoveItem called with null parameter
    /// Expected: InvalidOrderException thrown
    /// </summary>
    [Fact]
    public void RemoveItem_WithNullItem_ShouldThrowInvalidOrderException()
    {
        // Arrange
        var orderId = OrderId.Create();
        var customerId = CustomerId.Create();
        var order = Order.CreateOrder(orderId, customerId);

        // Act & Assert
        var act = () => order.RemoveItem(null!);
        act.Should().Throw<InvalidOrderException>();
    }

    /// <summary>
    /// Test: Removing item not in Order should throw InvalidOrderException.
    /// Scenario: Item doesn't exist in order
    /// Expected: InvalidOrderException thrown with "Item não encontrado" message
    /// </summary>
    [Fact]
    public void RemoveItem_NotInOrder_ShouldThrowInvalidOrderException()
    {
        // Arrange
        var orderId = OrderId.Create();
        var customerId = CustomerId.Create();
        var order = Order.CreateOrder(orderId, customerId);
        var item1 = OrderItem.Create("Product 1", 1, 100.00m);
        var item2 = OrderItem.Create("Product 2", 1, 50.00m);
        order.AddItem(item1);

        // Act & Assert
        var act = () => order.RemoveItem(item2);
        act.Should().Throw<InvalidOrderException>()
            .WithMessage("*não encontrado*");
    }

    /// <summary>
    /// Test: Order should have unique ID after creation.
    /// Scenario: Create two orders with same data
    /// Expected: Each order has different OrderId
    /// </summary>
    [Fact]
    public void CreateOrder_EachOrderShouldHaveUniqueId()
    {
        // Arrange
        var orderId1 = OrderId.Create();
        var orderId2 = OrderId.Create();
        var customerId = CustomerId.Create();

        // Act
        var order1 = Order.CreateOrder(orderId1, customerId);
        var order2 = Order.CreateOrder(orderId2, customerId);

        // Assert
        order1.OrderId.Should().NotBe(order2.OrderId);
    }

    /// <summary>
    /// Test: Order creation with custom date should preserve OrderDate.
    /// Scenario: Create order with specific date
    /// Expected: OrderDate is set to provided date
    /// </summary>
    [Fact]
    public void CreateOrder_WithCustomOrderDate_ShouldSetOrderDate()
    {
        // Arrange
        var orderId = OrderId.Create();
        var customerId = CustomerId.Create();
        var customDate = new DateTime(2026, 03, 01, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var order = Order.CreateOrder(orderId, customerId, customDate);

        // Assert
        order.OrderDate.Should().Be(customDate);
    }

    /// <summary>
    /// Test: Multiple items can be added to the same order.
    /// Scenario: Add multiple items sequentially
    /// Expected: All items are in order
    /// </summary>
    [Fact]
    public void AddMultipleItems_ShouldAccumulateCorrectly()
    {
        // Arrange
        var orderId = OrderId.Create();
        var customerId = CustomerId.Create();
        var order = Order.CreateOrder(orderId, customerId);

        var item1 = OrderItem.Create("Product 1", 1, 100.00m);
        var item2 = OrderItem.Create("Product 2", 2, 50.00m);
        var item3 = OrderItem.Create("Product 3", 3, 25.00m);

        // Act
        order.AddItem(item1);
        order.AddItem(item2);
        order.AddItem(item3);

        // Assert
        order.Items.Should().HaveCount(3);
        order.Items.Should().Contain(item1);
        order.Items.Should().Contain(item2);
        order.Items.Should().Contain(item3);
    }

    /// <summary>
    /// Test: Order should maintain status as New initially.
    /// Scenario: Create order without adding items
    /// Expected: Status remains New
    /// </summary>
    [Fact]
    public void CreateOrder_ShouldHaveNewStatus()
    {
        // Arrange
        var orderId = OrderId.Create();
        var customerId = CustomerId.Create();

        // Act
        var order = Order.CreateOrder(orderId, customerId);

        // Assert
        order.Status.Should().Be(OrderStatus.New);
    }
}
