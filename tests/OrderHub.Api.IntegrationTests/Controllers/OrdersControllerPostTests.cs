using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using OrderHub.Adapters.Inbound.Api.Models;
using OrderHub.Api.IntegrationTests.Fixtures;
using Xunit;

namespace OrderHub.Api.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for the POST /api/v1/orders endpoint.
/// Tests the creation of new orders through the HTTP API.
/// </summary>
public class OrdersControllerPostTests : IClassFixture<OrderHubWebApplicationFactory>
{
    private readonly OrderHubWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private const string OrdersEndpoint = "/api/v1/orders";

    public OrdersControllerPostTests(OrderHubWebApplicationFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _client = factory.CreateClient();
    }

    #region Valid Request Tests

    /// <summary>
    /// Test POST /orders with valid data should return 201 Created
    /// </summary>
    [Fact]
    public async Task PostOrder_WithValidRequest_ShouldReturnCreatedWithOrderId()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = "CUST-001",
            Items = new List<CreateOrderItemRequest>
            {
                new()
                {
                    ProductId = "PROD-001",
                    Quantity = 2,
                    UnitPrice = 100.00m
                }
            },
            Description = "Test order"
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(OrdersEndpoint, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var orderResponse = JsonSerializer.Deserialize<OrderResponse>(responseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        orderResponse.Should().NotBeNull();
        orderResponse!.OrderId.Should().NotBeNullOrEmpty();
        orderResponse.CustomerId.Should().Be(request.CustomerId);
    }

    /// <summary>
    /// Test that created order persists in the database
    /// </summary>
    [Fact]
    public async Task PostOrder_CreatedOrder_ShouldBeRetrievableViaGet()
    {
        // Arrange
        var customerId = "CUST-002";
        var request = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemRequest>
            {
                new()
                {
                    ProductId = "PROD-002",
                    Quantity = 1,
                    UnitPrice = 50.00m
                }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act - Create the order
        var createResponse = await _client.PostAsync(OrdersEndpoint, content);
        var createResponseBody = await createResponse.Content.ReadAsStringAsync();
        var createdOrder = JsonSerializer.Deserialize<OrderResponse>(createResponseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Act - Retrieve the created order
        var getResponse = await _client.GetAsync($"{OrdersEndpoint}/{createdOrder!.OrderId}");

        // Assert
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

    /// <summary>
    /// Test POST with multiple items in the order
    /// </summary>
    [Fact]
    public async Task PostOrder_WithMultipleItems_ShouldCreateSuccessfully()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = "CUST-003",
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-003A", Quantity = 2, UnitPrice = 75.00m },
                new() { ProductId = "PROD-003B", Quantity = 3, UnitPrice = 45.00m },
                new() { ProductId = "PROD-003C", Quantity = 1, UnitPrice = 120.00m }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(OrdersEndpoint, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var orderResponse = JsonSerializer.Deserialize<OrderResponse>(responseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        orderResponse.Should().NotBeNull();
        orderResponse!.OrderId.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region Invalid Request Tests

    /// <summary>
    /// Test POST with null request should return 400 Bad Request
    /// </summary>
    [Fact]
    public async Task PostOrder_WithNullRequest_ShouldReturnBadRequest()
    {
        // Arrange
        var content = new StringContent("null", Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(OrdersEndpoint, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Test POST with empty customer ID should return 400 Bad Request
    /// </summary>
    [Fact]
    public async Task PostOrder_WithEmptyCustomerId_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = string.Empty,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-004", Quantity = 1, UnitPrice = 100.00m }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(OrdersEndpoint, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Test POST with empty items list should return 400 Bad Request
    /// </summary>
    [Fact]
    public async Task PostOrder_WithEmptyItems_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = "CUST-005",
            Items = new List<CreateOrderItemRequest>() // Empty items
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(OrdersEndpoint, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Test POST with invalid JSON should return 400 Bad Request
    /// </summary>
    [Fact]
    public async Task PostOrder_WithInvalidJson_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidJson = "{ invalid json }";
        var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(OrdersEndpoint, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Test POST with negative quantity should return 400 Bad Request
    /// </summary>
    [Fact]
    public async Task PostOrder_WithNegativeQuantity_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = "CUST-006",
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-006", Quantity = -1, UnitPrice = 100.00m }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(OrdersEndpoint, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Test POST with negative unit price should return 400 Bad Request
    /// </summary>
    [Fact]
    public async Task PostOrder_WithNegativePrice_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = "CUST-007",
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-007", Quantity = 1, UnitPrice = -50.00m }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(OrdersEndpoint, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Response Validation Tests

    /// <summary>
    /// Test that response contains correct Content-Type header
    /// </summary>
    [Fact]
    public async Task PostOrder_Response_ShouldHaveCorrectContentType()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = "CUST-008",
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-008", Quantity = 1, UnitPrice = 100.00m }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(OrdersEndpoint, content);

        // Assert
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    /// <summary>
    /// Test that Location header is properly set in response
    /// </summary>
    [Fact]
    public async Task PostOrder_Response_ShouldContainLocationHeader()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = "CUST-009",
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-009", Quantity = 1, UnitPrice = 100.00m }
            }
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync(OrdersEndpoint, content);

        // Assert
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain("/api/v1/orders/");
    }

    #endregion
}
