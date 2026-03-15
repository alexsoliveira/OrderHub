using OrderHub.Application.Ports;
using OrderHub.Application.UseCases;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Application.UseCases.Orders;

/// <summary>
/// Application Service para cancelar um pedido existente
/// Implementa ICancelOrderUseCase (Input Port) da arquitetura hexagonal
/// </summary>
public class CancelOrderService : ICancelOrderUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationPort _notification;

    public CancelOrderService(
        IUnitOfWork unitOfWork,
        INotificationPort notification)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _notification = notification ?? throw new ArgumentNullException(nameof(notification));
    }

    /// <summary>
    /// Executa o cancelamento de um pedido
    /// </summary>
    public async Task ExecuteAsync(
        string orderId,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentException("OrderId é obrigatório", nameof(orderId));

        // Iniciar transação
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Recuperar pedido
            var orderResponse = await _unitOfWork.Orders.GetByIdAsync(orderId, cancellationToken);

            if (orderResponse == null)
                throw new InvalidOperationException($"Pedido com ID '{orderId}' não encontrado");

            // Converter string para ValueObject OrderId
            var orderIdValueObject = OrderId.Create(Guid.Parse(orderId));
            
            // Remover pedido (delete lógico ou físico conforme implementação do repositório)
            await _unitOfWork.Orders.DeleteAsync(orderIdValueObject, cancellationToken);

            // Confirmar transação
            await _unitOfWork.CommitAsync(cancellationToken);

            // Notificar cliente
            _ = _notification.SendOrderCancelledAsync(
                orderResponse.CustomerId,
                orderId,
                reason ?? "Cancelado pelo cliente",
                cancellationToken);
        }
        catch
        {
            // Desfazer transação em caso de erro
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
