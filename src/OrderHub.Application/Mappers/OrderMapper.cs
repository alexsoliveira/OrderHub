using OrderHub.Application.DTOs;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Application.Mappers;

/// <summary>
/// Mapper responsável por converter entre Domain Models e DTOs
/// </summary>
public static class OrderMapper
{
    /// <summary>
    /// Converte CreateOrderRequest (DTO) para Order (Entity de Domínio)
    /// </summary>
    public static Order ToDomainEntity(CreateOrderRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var orderId = OrderId.Create();
        var customerId = CustomerId.Parse(request.CustomerId);

        var order = Order.CreateOrder(orderId, customerId);

        foreach (var item in request.Items ?? [])
        {
            var amount = OrderAmount.Create(item.UnitPrice);
            var orderItem = new OrderItem(
                ProductId.Create(item.ProductId),
                item.Quantity,
                amount
            );
            order.AddItem(orderItem);
        }

        return order;
    }

    /// <summary>
    /// Converte Order (Entity de Domínio) para OrderResponse (DTO)
    /// </summary>
    public static OrderResponse ToResponse(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        var items = order.Items.Select(item => new OrderItemResponse
        {
            ProductId = item.ProductId.Value,
            Quantity = item.Quantity,
            UnitPrice = item.Amount.Value,
            SubTotal = item.Quantity * item.Amount.Value
        }).ToList();

        var totalAmount = order.Items.Sum(i => (i.Quantity * i.Amount.Value));

        return new OrderResponse
        {
            OrderId = order.OrderId.Value.ToString(),
            CustomerId = order.CustomerId.Value.ToString(),
            OrderDate = order.OrderDate,
            Status = order.Status.ToString(),
            Items = items,
            TotalAmount = totalAmount,
            Currency = "BRL"
        };
    }

    /// <summary>
    /// Converte UpdateOrderRequest (DTO) para Order (Entity de Domínio)
    /// Substitui os items do pedido existente
    /// </summary>
    public static Order UpdateDomainEntity(Order order, UpdateOrderRequest request)
    {
        ArgumentNullException.ThrowIfNull(order);
        ArgumentNullException.ThrowIfNull(request);

        // Remover todos os itens atuais
        foreach (var item in order.Items.ToList())
        {
            order.RemoveItem(item);
        }

        // Adicionar novos itens
        foreach (var dto in request.Items ?? [])
        {
            var amount = OrderAmount.Create(dto.UnitPrice);
            var orderItem = new OrderItem(
                ProductId.Create(dto.ProductId),
                dto.Quantity,
                amount
            );
            order.AddItem(orderItem);
        }

        return order;
    }
}
