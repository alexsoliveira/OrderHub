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

    /// <summary>
    /// Construtor do controller
    /// </summary>
    public OrdersController(
        ICreateOrderUseCase createOrderUseCase,
        IGetOrderUseCase getOrderUseCase)
    {
        _createOrderUseCase = createOrderUseCase ?? throw new ArgumentNullException(nameof(createOrderUseCase));
        _getOrderUseCase = getOrderUseCase ?? throw new ArgumentNullException(nameof(getOrderUseCase));
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
            var orders = new List<ApiModels.OrderResponse>();
            // TODO: Implementar quando houver IListOrdersUseCase
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
            // TODO: Implementar quando houver IUpdateOrderUseCase
            return NotFound($"Pedido com ID {orderId} não encontrado");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "Erro ao atualizar pedido", details = ex.Message });
        }
    }

    /// <summary>
    /// Deleta um pedido
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
            // TODO: Implementar quando houver IDeleteOrderUseCase
            return NotFound($"Pedido com ID {orderId} não encontrado");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "Erro ao deletar pedido", details = ex.Message });
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
