using Microsoft.AspNetCore.Mvc;
using OrderHub.Adapters.Inbound.Api.Models;
using OrderHub.Application.DTOs;
using OrderHub.Application.UseCases;
using ApiModels = OrderHub.Adapters.Inbound.Api.Models;
using AppDtos = OrderHub.Application.DTOs;

namespace OrderHub.Adapters.Inbound.Api.Controllers;

/// <summary>
/// Controller para gerenciar operações de pedidos
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ICreateOrderUseCase _createOrderUseCase;
    private readonly IGetOrderUseCase _getOrderUseCase;
    private readonly IUpdateOrderUseCase _updateOrderUseCase;
    private readonly ICancelOrderUseCase _cancelOrderUseCase;
    private readonly IListOrdersUseCase _listOrdersUseCase;

    /// <summary>
    /// Construtor do controller
    /// </summary>
    public OrdersController(
        ICreateOrderUseCase createOrderUseCase,
        IGetOrderUseCase getOrderUseCase,
        IUpdateOrderUseCase updateOrderUseCase,
        ICancelOrderUseCase cancelOrderUseCase,
        IListOrdersUseCase listOrdersUseCase)
    {
        _createOrderUseCase = createOrderUseCase ?? throw new ArgumentNullException(nameof(createOrderUseCase));
        _getOrderUseCase = getOrderUseCase ?? throw new ArgumentNullException(nameof(getOrderUseCase));
        _updateOrderUseCase = updateOrderUseCase ?? throw new ArgumentNullException(nameof(updateOrderUseCase));
        _cancelOrderUseCase = cancelOrderUseCase ?? throw new ArgumentNullException(nameof(cancelOrderUseCase));
        _listOrdersUseCase = listOrdersUseCase ?? throw new ArgumentNullException(nameof(listOrdersUseCase));
    }

    /// <summary>
    /// Cria um novo pedido
    /// </summary>
    /// <param name="request">Dados do pedido a ser criado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Pedido criado</returns>
    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync(
        [FromBody] ApiModels.CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            return BadRequest("Requisição inválida");

        try
        {
            // Map API request DTO to Application DTO
            var appRequest = MapToApplicationCreateOrderRequest(request);
            var response = await _createOrderUseCase.ExecuteAsync(appRequest, cancellationToken);
            
            var orderResponse = MapToOrderResponse(response);
            
            return CreatedAtAction(nameof(GetOrderAsync), 
                new { orderId = orderResponse.OrderId }, 
                orderResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém um pedido específico por ID
    /// </summary>
    /// <param name="orderId">ID do pedido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do pedido</returns>
    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrderAsync(
        [FromRoute] string orderId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return BadRequest("ID do pedido é obrigatório");

        try
        {
            var response = await _getOrderUseCase.ExecuteAsync(orderId, cancellationToken);
            
            if (response == null)
                return NotFound($"Pedido com ID {orderId} não encontrado");
            
            var orderResponse = MapToOrderResponse(response);
            return Ok(orderResponse);
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtém todos os pedidos
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de pedidos</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllOrdersAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Call use case to list all orders
            var orders = await _listOrdersUseCase.ExecuteAsync(cancellationToken);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "Erro ao listar pedidos", details = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza um pedido existente
    /// </summary>
    /// <param name="orderId">ID do pedido</param>
    /// <param name="request">Dados atualizados</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Pedido atualizado</returns>
    [HttpPut("{orderId}")]
    public async Task<IActionResult> UpdateOrderAsync(
        [FromRoute] string orderId,
        [FromBody] ApiModels.UpdateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return BadRequest("ID do pedido é obrigatório");

        if (request == null)
            return BadRequest("Requisição inválida");

        try
        {
            // Map API request to Application DTO
            var appRequest = new AppDtos.UpdateOrderRequest
            {
                OrderId = orderId,
                Description = request.Description,
                Items = request.Items?.Select(item => new AppDtos.OrderItemRequest
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList() ?? new List<AppDtos.OrderItemRequest>()
            };

            // Call use case
            var response = await _updateOrderUseCase.ExecuteAsync(appRequest, cancellationToken);
            
            if (response == null)
                return NotFound($"Pedido com ID {orderId} não encontrado");
            
            var orderResponse = MapToOrderResponse(response);
            return Ok(orderResponse);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "Erro ao atualizar pedido", details = ex.Message });
        }
    }

    /// <summary>
    /// Cancela um pedido (delete lógico)
    /// </summary>
    /// <param name="orderId">ID do pedido</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>No Content</returns>
    [HttpDelete("{orderId}")]
    public async Task<IActionResult> DeleteOrderAsync(
        [FromRoute] string orderId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return BadRequest("ID do pedido é obrigatório");

        try
        {
            // Call cancel order use case
            await _cancelOrderUseCase.ExecuteAsync(orderId, "Cancelado via API", cancellationToken);
            
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "Erro ao cancelar pedido", details = ex.Message });
        }
    }

    /// <summary>
    /// Mapeia CreateOrderRequest (API) para CreateOrderRequest (Application)
    /// </summary>
    private static AppDtos.CreateOrderRequest MapToApplicationCreateOrderRequest(
        ApiModels.CreateOrderRequest apiRequest)
    {
        return new AppDtos.CreateOrderRequest
        {
            CustomerId = apiRequest.CustomerId,
            Description = apiRequest.Description,
            Items = apiRequest.Items?.Select(item => new AppDtos.OrderItemRequest
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList() ?? new List<AppDtos.OrderItemRequest>()
        };
    }

    /// <summary>
    /// Mapeia OrderResponse DTO para OrderResponse model da API
    /// </summary>
    private static ApiModels.OrderResponse MapToOrderResponse(AppDtos.OrderResponse dtoResponse)
    {
        return new ApiModels.OrderResponse
        {
            OrderId = dtoResponse.OrderId,
            CustomerId = dtoResponse.CustomerId,
            Status = dtoResponse.Status,
            TotalAmount = dtoResponse.TotalAmount,
            CreatedAt = dtoResponse.OrderDate,
            Items = dtoResponse.Items?.Select(item => new ApiModels.OrderItemResponse
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.SubTotal
            }).ToList() ?? new List<ApiModels.OrderItemResponse>()
        };
    }
}
