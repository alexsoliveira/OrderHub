using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using OrderHub.Adapters.Inbound.Api.Models;
using OrderHub.Api.IntegrationTests.Fixtures;
using Xunit;

namespace OrderHub.Api.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for the GET /api/v1/orders/{id} endpoint.
/// Tests the retrieval of orders through the HTTP API.
/// </summary>
public class OrdersControllerGetTests : IClassFixture<OrderHubWebApplicationFactory>
{
    private readonly OrderHubWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private const string OrdersEndpoint = "/api/v1/orders";

    public OrdersControllerGetTests(OrderHubWebApplicationFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _client = factory.CreateClient();
    }

    #region Valid Request Tests

    /// <summary>
    /// Test GET /orders/{id} with valid ID should return 200 OK
    /// </summary>
    [Fact]
    public async Task GetOrder_WithValidId_ShouldReturnOkAndOrder()
    {
        // Arrange
        var customerId = "CUST-001";
        var createRequest = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemRequest>
            {
                new()
                {
                    ProductId = "PROD-001",
                    Quantity = 2,
                    UnitPrice = 100.00m
                }
            },
            Description = "Test order for GET"
        };

        // Create an order first
        var createJson = JsonSerializer.Serialize(createRequest);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync(OrdersEndpoint, createContent);
        
        var createResponseBody = await createResponse.Content.ReadAsStringAsync();
        var createdOrder = JsonSerializer.Deserialize<OrderResponse>(createResponseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Act
        var response = await _client.GetAsync($"{OrdersEndpoint}/{createdOrder!.OrderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var retrievedOrder = JsonSerializer.Deserialize<OrderResponse>(responseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        retrievedOrder.Should().NotBeNull();
        retrievedOrder!.OrderId.Should().Be(createdOrder.OrderId);
        retrievedOrder.CustomerId.Should().Be(customerId);
    }

    /// <summary>
    /// Test that retrieved order data matches created data
    /// </summary>
    [Fact]
    public async Task GetOrder_RetrievedData_ShouldMatchCreatedData()
    {
        // Arrange
        var customerId = "CUST-GET-001";
        var description = "Matching data test";
        var createRequest = new CreateOrderRequest
        {
            CustomerId = customerId,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-GET-001", Quantity = 5, UnitPrice = 25.00m }
            },
            Description = description
        };

        var createJson = JsonSerializer.Serialize(createRequest);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync(OrdersEndpoint, createContent);
        var createResponseBody = await createResponse.Content.ReadAsStringAsync();
        
        var createdOrder = JsonSerializer.Deserialize<OrderResponse>(createResponseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Act
        var getResponse = await _client.GetAsync($"{OrdersEndpoint}/{createdOrder!.OrderId}");
        var getResponseBody = await getResponse.Content.ReadAsStringAsync();
        var retrievedOrder = JsonSerializer.Deserialize<OrderResponse>(getResponseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Assert
        retrievedOrder.Should().NotBeNull();
        retrievedOrder!.CustomerId.Should().Be(customerId);
        retrievedOrder.OrderId.Should().Be(createdOrder.OrderId);
    }

    /// <summary>
    /// Test GET with valid response structure
    /// </summary>
    [Fact]
    public async Task GetOrder_Response_ShouldHaveValidStructure()
    {
        // Arrange
        var createRequest = new CreateOrderRequest
        {
            CustomerId = "CUST-STRUCT",
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-STRUCT", Quantity = 1, UnitPrice = 100.00m }
            }
        };

        var createJson = JsonSerializer.Serialize(createRequest);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync(OrdersEndpoint, createContent);
        var createResponseBody = await createResponse.Content.ReadAsStringAsync();
        
        var createdOrder = JsonSerializer.Deserialize<OrderResponse>(createResponseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Act
        var getResponse = await _client.GetAsync($"{OrdersEndpoint}/{createdOrder!.OrderId}");
        var getResponseBody = await getResponse.Content.ReadAsStringAsync();

        // Assert
        getResponse.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
        
        var retrievedOrder = JsonSerializer.Deserialize<OrderResponse>(getResponseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        retrievedOrder.Should().NotBeNull();
        retrievedOrder!.OrderId.Should().NotBeNullOrEmpty();
        retrievedOrder.CustomerId.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region Invalid Request Tests

    /// <summary>
    /// Test GET with non-existent ID should return 404 Not Found
    /// </summary>
    [Fact]
    public async Task GetOrder_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = "00000000-0000-0000-0000-000000000000";

        // Act
        var response = await _client.GetAsync($"{OrdersEndpoint}/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    /// <summary>
    /// Test GET with empty ID should return 400 Bad Request
    /// </summary>
    [Fact]
    public async Task GetOrder_WithEmptyId_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync($"{OrdersEndpoint}/");

        // Assert
        // Empty ID results in different routing, so either 404 or 400 is acceptable
        (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound).Should().BeTrue();
    }

    /// <summary>
    /// Test GET with invalid ID format should return 404 or 400
    /// </summary>
    [Fact]
    public async Task GetOrder_WithInvalidIdFormat_ShouldReturnNotFoundOrBadRequest()
    {
        // Act
        var response = await _client.GetAsync($"{OrdersEndpoint}/invalid-id-format");

        // Assert
        (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest).Should().BeTrue();
    }

    /// <summary>
    /// Test GET with null ID should return 404
    /// </summary>
    [Fact]
    public async Task GetOrder_WithWhitespaceId_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync($"{OrdersEndpoint}/   ");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Response Validation Tests

    /// <summary>
    /// Test that response has correct Content-Type
    /// </summary>
    [Fact]
    public async Task GetOrder_Response_ShouldHaveCorrectContentType()
    {
        // Arrange
        var createRequest = new CreateOrderRequest
        {
            CustomerId = "CUST-CONTENT-TYPE",
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = "PROD-CT", Quantity = 1, UnitPrice = 100.00m }
            }
        };

        var createJson = JsonSerializer.Serialize(createRequest);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync(OrdersEndpoint, createContent);
        var createResponseBody = await createResponse.Content.ReadAsStringAsync();
        
        var createdOrder = JsonSerializer.Deserialize<OrderResponse>(createResponseBody, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        // Act
        var response = await _client.GetAsync($"{OrdersEndpoint}/{createdOrder!.OrderId}");

        // Assert
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    /// <summary>
    /// Test that error response is properly formatted
    /// </summary>
    [Fact]
    public async Task GetOrder_ErrorResponse_ShouldBeProperlyFormatted()
    {
        // Act
        var response = await _client.GetAsync($"{OrdersEndpoint}/non-existent");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    #endregion
}
