using OrderHub.Application.DTOs;
using OrderHub.Application.Ports;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Application.UseCases.Orders;

/// <summary>
/// Application Service para atualizar um pedido existente
/// </summary>
public class UpdateOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Executa a atualização de um pedido existente
    /// </summary>
    public async Task<OrderResponse> ExecuteAsync(
        UpdateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.OrderId))
            throw new ArgumentException("OrderId é obrigatório", nameof(request.OrderId));

        if (request.Items == null || request.Items.Count == 0)
            throw new ArgumentException("Pedido deve ter no mínimo 1 item", nameof(request.Items));

        // Iniciar transação
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Recuperar pedido existente
            var orderId = OrderId.Parse(request.OrderId);
            var orderResponse = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);

            if (orderResponse == null)
                throw new InvalidOperationException($"Pedido com ID '{request.OrderId}' não encontrado");

            // Recrear entidade de domínio com dados atualizados
            var customerId = CustomerId.Parse(orderResponse.CustomerId);
            var order = Order.CreateOrder(orderId, customerId, orderResponse.OrderDate);

            // Limpar itens atuais e adicionar novos
            foreach (var itemDto in request.Items)
            {
                var amount = OrderAmount.Create(itemDto.UnitPrice);
                var productId = ProductId.Create(itemDto.ProductId);
                var orderItem = new OrderItem(productId, itemDto.Quantity, amount);
                order.AddItem(orderItem);
            }

            // Persistir alterações
            await _unitOfWork.Orders.SaveAsync(order, cancellationToken);

            // Confirmar transação
            await _unitOfWork.CommitAsync(cancellationToken);

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
