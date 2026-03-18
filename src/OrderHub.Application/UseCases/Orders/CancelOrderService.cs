using OrderHub.Application.Exceptions;
using OrderHub.Domain.Ports;
using OrderHub.Application.UseCases;
using OrderHub.Domain.ValueObjects;
using OrderHub.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace OrderHub.Application.UseCases.Orders;

/// <summary>
/// Application Service para cancelar um pedido existente
/// Implementa ICancelOrderUseCase (Input Port) da arquitetura hexagonal
/// </summary>
public class CancelOrderService : ICancelOrderUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationPort _notification;
    private readonly ILogger<CancelOrderService> _logger;

    public CancelOrderService(
        IUnitOfWork unitOfWork,
        INotificationPort notification,
        ILogger<CancelOrderService> logger)
    {
        if (unitOfWork == null)
            throw InvalidRequestException.CreateForNullField(nameof(unitOfWork), "dependency injection failed");
        if (notification == null)
            throw InvalidRequestException.CreateForNullField(nameof(notification), "dependency injection failed");
        if (logger == null)
            throw InvalidRequestException.CreateForNullField(nameof(logger), "dependency injection failed");
        
        _unitOfWork = unitOfWork;
        _notification = notification;
        _logger = logger;
    }

    /// <summary>
    /// Executa o cancelamento de um pedido
    /// </summary>
    public async Task ExecuteAsync(
        string orderId,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando CancelOrderUseCase para OrderId: {OrderId}, Reason: {Reason}", orderId, reason ?? "Não informado");

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
                {
                    _logger.LogWarning("Pedido não encontrado para cancelamento. OrderId: {OrderId}", orderId);
                    throw new OrderNotFoundException(orderId);
                }
                
                _logger.LogInformation("Pedido encontrado para cancelamento. OrderId: {OrderId}, CustomerId: {CustomerId}", 
                    orderId, order.CustomerId.Value);

                // Remover pedido (delete lógico ou físico conforme implementação do repositório)
                await _unitOfWork.Orders.DeleteAsync(orderIdValueObject, cancellationToken);

                // Confirmar transação
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("CancelOrderUseCase concluído com sucesso. OrderId: {OrderId}", orderId);

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
                _logger.LogError(ex, "Erro ao cancelar pedido. OrderId: {OrderId}", orderId);
                throw RepositoryException.CreateForDelete(orderId, ex);
            }
        }
        catch
        {
            // Desfazer transação em caso de erro
            _logger.LogWarning("Desfazendo transação - erro ao cancelar pedido");
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
