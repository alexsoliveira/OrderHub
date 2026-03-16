using OrderHub.Application.Exceptions;
using OrderHub.Domain.Ports;
using OrderHub.Application.UseCases;
using OrderHub.Domain.ValueObjects;
using OrderHub.Domain.Exceptions;

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
        if (unitOfWork == null)
            throw InvalidRequestException.CreateForNullField(nameof(unitOfWork), "dependency injection failed");
        if (notification == null)
            throw InvalidRequestException.CreateForNullField(nameof(notification), "dependency injection failed");
        
        _unitOfWork = unitOfWork;
        _notification = notification;
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
            throw InvalidRequestException.CreateForNullField(nameof(orderId));

        // Iniciar transação
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            try
            {
                // Converter string para ValueObject OrderId
                if (!Guid.TryParse(orderId, out var guidId))
                    throw new InvalidRequestException(nameof(orderId), "deve ser um GUID válido");

                var orderIdValueObject = OrderId.Create(guidId);

                // Recuperar pedido
                var order = await _unitOfWork.Orders.GetByIdAsync(orderIdValueObject, cancellationToken);

                if (order == null)
                    throw new OrderNotFoundException(orderId);
                
                // Remover pedido (delete lógico ou físico conforme implementação do repositório)
                await _unitOfWork.Orders.DeleteAsync(orderIdValueObject, cancellationToken);

                // Confirmar transação
                await _unitOfWork.CommitAsync(cancellationToken);

                // Notificar cliente
                _ = _notification.SendOrderCancelledAsync(
                    order.CustomerId.Value.ToString(),
                    orderId,
                    reason ?? "Cancelado pelo cliente",
                    cancellationToken);
            }
            catch (Exception ex) when (!(ex is OrderHub.Application.Exceptions.ApplicationException))
            {
                // Traduzir exceções não-application
                throw RepositoryException.CreateForDelete(orderId, ex);
            }
        }
        catch
        {
            // Desfazer transação em caso de erro
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
