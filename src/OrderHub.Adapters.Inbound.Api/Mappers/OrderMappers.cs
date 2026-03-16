using OrderHub.Adapters.Inbound.Api.Models;
using OrderHub.Application.DTOs;
using AppCreateOrderRequest = OrderHub.Application.DTOs.CreateOrderRequest;
using AppOrderResponse = OrderHub.Application.DTOs.OrderResponse;

namespace OrderHub.Adapters.Inbound.Api.Mappers;

/// <summary>
/// Mapeador para conversão entre API Models e Application DTOs
/// Responsável por transformar requisições HTTP em objetos de aplicação
/// </summary>
public static class OrderMappers
{
    /// <summary>
    /// Converte CreateOrderRequest (modelo de entrada da API) para CreateOrderRequest (DTO da aplicação)
    /// </summary>
    public static AppCreateOrderRequest ToCreateOrderRequest(this Models.CreateOrderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        return new AppCreateOrderRequest
        {
            CustomerId = request.CustomerId,
            Items = request.Items?.Select(item => new OrderItemRequest
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList() ?? new()
        };
    }

    /// <summary>
    /// Converte OrderResponse (DTO da aplicação) para OrderResponse (modelo de resposta da API)
    /// </summary>
    public static Models.OrderResponse ToOrderResponse(this AppOrderResponse dto)
    {
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        return new Models.OrderResponse
        {
            OrderId = dto.OrderId,
            CustomerId = dto.CustomerId,
            Status = dto.Status,
            CreatedAt = dto.OrderDate,
            TotalAmount = dto.TotalAmount,
            Items = dto.Items?.Select(item => new Models.OrderItemResponse
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.SubTotal
            }).ToList() ?? new()
        };
    }
}
