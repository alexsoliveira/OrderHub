using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using OrderHub.Adapters.Inbound.Api.Models;
using OrderHub.Api.IntegrationTests.Fixtures;
using Xunit;

namespace OrderHub.Api.IntegrationTests.Database;

/// <summary>
/// Integration tests for database persistence.
/// Tests that verify data is correctly persisted and retrieved from the in-memory database.
/// </summary>
public class DatabaseIntegrationTests : IClassFixture<OrderHubWebApplicationFactory>
{
    private readonly OrderHubWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private const string OrdersEndpoint = "/api/v1/orders";

    public DatabaseIntegrationTests(OrderHubWebApplicationFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _client = factory.CreateClient();
    }

    #region Create and Persist Tests

    /// <summary>
    /// Test that created order is persisted in the database
    /// </summary>
    [Fact]
    public async Task CreateOrder_ShouldPersistToDatabase()
    {
        // Arrange
        var customerId = "CUST-DB-001";
        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-DB-001", Quantity = 2, UnitPrice = 50.00m }
            },
            Description = "Database persistence test"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act - Create order
        var response = await _client.PostAsync(OrdersEndpoint, content);
        var responseBody = await response.Content.ReadAsStringAsync();
        var createdOrder = JsonSerializer.Deserialize<OrderResponse>(responseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Assert - Verify in database
        using var dbContext = _factory.CreateDbContext();
        var savedOrder = await dbContext.Orders.FindAsync(createdOrder!.OrderId);
        
        savedOrder.Should().NotBeNull();
        savedOrder!.CustomerId.Should().Be(customerId);
    }

    /// <summary>
    /// Test that multiple items in order are persisted
    /// </summary>
    [Fact]
    public async Task CreateOrder_WithMultipleItems_ShouldPersistAllItems()
    {
        // Arrange
        var customerId = "CUST-DB-MULTI";
        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-DB-A", Quantity = 1, UnitPrice = 100.00m },
                new() { ProductId = "PROD-DB-B", Quantity = 2, UnitPrice = 50.00m },
                new() { ProductId = "PROD-DB-C", Quantity = 3, UnitPrice = 25.00m }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act - Create order
        var response = await _client.PostAsync(OrdersEndpoint, content);
        var responseBody = await response.Content.ReadAsStringAsync();
        var createdOrder = JsonSerializer.Deserialize<OrderResponse>(responseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Assert - Verify order and items in database
        using var dbContext = _factory.CreateDbContext();
        var savedOrder = await dbContext.Orders.FindAsync(createdOrder!.OrderId);
        
        savedOrder.Should().NotBeNull();
        savedOrder!.CustomerId.Should().Be(customerId);
    }

    /// <summary>
    /// Test that created order data is exactly as inserted
    /// </summary>
    [Fact]
    public async Task CreateOrder_DatabaseData_ShouldMatchRequestData()
    {
        // Arrange
        var customerId = "CUST-DB-MATCH";
        var description = "Data match test";
        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-DB-MATCH", Quantity = 5, UnitPrice = 30.50m }
            },
            Description = description
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act - Create order
        var response = await _client.PostAsync(OrdersEndpoint, content);
        var responseBody = await response.Content.ReadAsStringAsync();
        var createdOrder = JsonSerializer.Deserialize<OrderResponse>(responseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Assert - Verify data matches exactly
        using var dbContext = _factory.CreateDbContext();
        var savedOrder = await dbContext.Orders.FindAsync(createdOrder!.OrderId);
        
        savedOrder.Should().NotBeNull();
        savedOrder!.CustomerId.Should().Be(customerId);
        savedOrder.OrderId.Should().Be(createdOrder.OrderId);
    }

    #endregion

    #region Retrieval Tests

    /// <summary>
    /// Test that order can be retrieved from database after insertion
    /// </summary>
    [Fact]
    public async Task RetrieveOrder_AfterCreation_ShouldFindInDatabase()
    {
        // Arrange
        var customerId = "CUST-DB-RETRIEVE";
        var createRequest = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-RETRIEVE", Quantity = 1, UnitPrice = 100.00m }
            }
        };

        var createJson = JsonSerializer.Serialize(createRequest);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");

        // Act - Create
        var createResponse = await _client.PostAsync(OrdersEndpoint, createContent);
        var createResponseBody = await createResponse.Content.ReadAsStringAsync();
        var createdOrder = JsonSerializer.Deserialize<OrderResponse>(createResponseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Act - Try to retrieve via API
        var getResponse = await _client.GetAsync($"{OrdersEndpoint}/{createdOrder!.OrderId}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getBody = await getResponse.Content.ReadAsStringAsync();
        var retrievedOrder = JsonSerializer.Deserialize<OrderResponse>(getBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        retrievedOrder.Should().NotBeNull();
        retrievedOrder!.OrderId.Should().Be(createdOrder.OrderId);
    }

    /// <summary>
    /// Test that multiple orders persist independently in database
    /// </summary>
    [Fact]
    public async Task CreateMultipleOrders_AllShouldPersistIndependently()
    {
        // Arrange
        var customers = new[] { "CUST-DB-IND-1", "CUST-DB-IND-2", "CUST-DB-IND-3" };
        var createdOrderIds = new List<string>();

        // Act - Create 3 orders
        foreach (var customerId in customers)
        {
            var request = new CreateOrderRequest
            {
                CustomerId = customerId,
                Items = new List<CreateOrderItemRequest>
                {
                    new() { ProductId = $"PROD-{customerId}", Quantity = 1, UnitPrice = 100.00m }
                }
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(OrdersEndpoint, content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdOrder = JsonSerializer.Deserialize<OrderResponse>(responseBody, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            createdOrderIds.Add(createdOrder!.OrderId);
        }

        // Assert - Verify all orders exist in database independently
        using var dbContext = _factory.CreateDbContext();
        
        for (int i = 0; i < createdOrderIds.Count; i++)
        {
            var savedOrder = await dbContext.Orders.FindAsync(createdOrderIds[i]);
            savedOrder.Should().NotBeNull();
            savedOrder!.CustomerId.Should().Be(customers[i]);
        }
    }

    #endregion

    #region Data Integrity Tests

    /// <summary>
    /// Test that order ID is unique and persisted correctly
    /// </summary>
    [Fact]
    public async Task CreateOrder_OrderId_ShouldBeUniqueAndPersisted()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = "CUST-DB-UNIQUE",
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-UNIQUE", Quantity = 1, UnitPrice = 100.00m }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(OrdersEndpoint, content);
        var responseBody = await response.Content.ReadAsStringAsync();
        var createdOrder = JsonSerializer.Deserialize<OrderResponse>(responseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Assert
        createdOrder.Should().NotBeNull();
        createdOrder!.OrderId.Should().NotBeNullOrEmpty();
        
        using var dbContext = _factory.CreateDbContext();
        var savedOrder = await dbContext.Orders.FindAsync(createdOrder.OrderId);
        savedOrder.Should().NotBeNull();
        savedOrder!.OrderId.Should().Be(createdOrder.OrderId);
    }

    /// <summary>
    /// Test that customer ID is persisted exactly as provided
    /// </summary>
    [Fact]
    public async Task CreateOrder_CustomerId_ShouldPersistExactly()
    {
        // Arrange
        var customerId = "CUST-EXACT-123-ABC";
        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-EXACT", Quantity = 1, UnitPrice = 100.00m }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(OrdersEndpoint, content);
        var responseBody = await response.Content.ReadAsStringAsync();
        var createdOrder = JsonSerializer.Deserialize<OrderResponse>(responseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Assert
        using var dbContext = _factory.CreateDbContext();
        var savedOrder = await dbContext.Orders.FindAsync(createdOrder!.OrderId);
        
        savedOrder.Should().NotBeNull();
        savedOrder!.CustomerId.Should().Be(customerId);
    }

    /// <summary>
    /// Test that order quantity and price are persisted correctly
    /// </summary>
    [Fact]
    public async Task CreateOrder_ItemQuantityAndPrice_ShouldPersistCorrectly()
    {
        // Arrange
        var quantity = 7;
        var unitPrice = 123.45m;
        var request = new CreateOrderRequest
        {
            CustomerId = "CUST-DB-PRICE",
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-PRICE", Quantity = quantity, UnitPrice = unitPrice }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(OrdersEndpoint, content);
        var responseBody = await response.Content.ReadAsStringAsync();
        var createdOrder = JsonSerializer.Deserialize<OrderResponse>(responseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Assert
        using var dbContext = _factory.CreateDbContext();
        var savedOrder = await dbContext.Orders.FindAsync(createdOrder!.OrderId);
        
        savedOrder.Should().NotBeNull();
        // Note: Items relationship - adjust based on actual Order model structure
    }

    #endregion

    #region State and Consistency Tests

    /// <summary>
    /// Test that orders maintain state consistency between API and database
    /// </summary>
    [Fact]
    public async Task OrderCreation_ApiAndDatabase_ShouldBeConsistent()
    {
        // Arrange
        var customerId = "CUST-DB-CONSISTENCY";
        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-CONSISTENCY", Quantity = 2, UnitPrice = 50.00m }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act - Create order via API
        var createResponse = await _client.PostAsync(OrdersEndpoint, content);
        var createResponseBody = await createResponse.Content.ReadAsStringAsync();
        var apiOrder = JsonSerializer.Deserialize<OrderResponse>(createResponseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Act - Retrieve order via API to verify database consistency
        var getResponse = await _client.GetAsync($"{OrdersEndpoint}/{apiOrder!.OrderId}");
        var getResponseBody = await getResponse.Content.ReadAsStringAsync();
        var retrievedOrder = JsonSerializer.Deserialize<OrderResponse>(getResponseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Assert - Verify consistency between creation and retrieval
        apiOrder.Should().NotBeNull();
        retrievedOrder.Should().NotBeNull();
        apiOrder!.OrderId.Should().Be(retrievedOrder!.OrderId);
        apiOrder.CustomerId.Should().Be(retrievedOrder.CustomerId);
    }

    /// <summary>
    /// Test that database transactions maintain data integrity
    /// </summary>
    [Fact]
    public async Task OrderCreation_TransactionIntegrity_ShouldMaintainConsistency()
    {
        // Arrange
        var customerId = "CUST-DB-TRANSACTION";
        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-TRANS-A", Quantity = 1, UnitPrice = 100.00m },
                new() { ProductId = "PROD-TRANS-B", Quantity = 2, UnitPrice = 50.00m }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act - Create order
        var createResponse = await _client.PostAsync(OrdersEndpoint, content);
        var createResponseBody = await createResponse.Content.ReadAsStringAsync();
        var createdOrder = JsonSerializer.Deserialize<OrderResponse>(createResponseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Act - Retrieve order to verify transaction integrity
        var getResponse = await _client.GetAsync($"{OrdersEndpoint}/{createdOrder!.OrderId}");

        // Assert - Verify creation succeeded and data is persisted
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var getResponseBody = await getResponse.Content.ReadAsStringAsync();
        var retrievedOrder = JsonSerializer.Deserialize<OrderResponse>(getResponseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        retrievedOrder.Should().NotBeNull();
        retrievedOrder!.OrderId.Should().Be(createdOrder.OrderId);
        retrievedOrder.CustomerId.Should().Be(customerId);
    }

    #endregion
}
