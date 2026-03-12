using OrderHub.Application.DTOs;
using OrderHub.Application.Ports;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Application.UseCases.Orders;

/// <summary>
/// Application Service para criar um novo pedido
/// Orquestra a criação, persistência e notificação de novo pedido
/// </summary>
public class CreateOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationPort _notification;

    public CreateOrderService(
        IUnitOfWork unitOfWork,
        INotificationPort notification)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _notification = notification ?? throw new ArgumentNullException(nameof(notification));
    }

    /// <summary>
    /// Executa a criação de um novo pedido
    /// </summary>
    public async Task<OrderResponse> ExecuteAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Validar request (validação básica)
        if (string.IsNullOrWhiteSpace(request.CustomerId))
            throw new ArgumentException("CustomerId é obrigatório", nameof(request.CustomerId));

        if (request.Items == null || request.Items.Count == 0)
            throw new ArgumentException("Pedido deve ter no mínimo 1 item", nameof(request.Items));

        // Iniciar transação
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Criar agregado de domínio
            var orderId = OrderId.Create();
            var customerId = CustomerId.Parse(request.CustomerId);
            var order = Order.CreateOrder(orderId, customerId);

            // Adicionar itens ao pedido
            foreach (var itemDto in request.Items)
            {
                var amount = OrderAmount.Create(itemDto.UnitPrice);
                var productId = ProductId.Create(itemDto.ProductId);
                var orderItem = new OrderItem(productId, itemDto.Quantity, amount);
                order.AddItem(orderItem);
            }

            // Persistir pedido
            await _unitOfWork.Orders.SaveAsync(order, cancellationToken);

            // Confirmar transação
            await _unitOfWork.CommitAsync(cancellationToken);

            // Notificar cliente (assincrono, não bloqueia o retorno)
            _ = _notification.SendOrderConfirmationAsync(
                request.CustomerId,
                order.OrderId.Value.ToString(),
                cancellationToken);

            // Converter para DTO e retornar
            return Mappers.OrderMapper.ToResponse(order);
        }
        catch
        {
            // Desfazer transação em caso de erro
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
