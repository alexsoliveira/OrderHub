using OrderHub.Application.DTOs;
using OrderHub.Application.Exceptions;
using OrderHub.Domain.Ports;
using OrderHub.Application.UseCases;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.ValueObjects;
using OrderHub.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace OrderHub.Application.UseCases.Orders;

/// <summary>
/// Application Service para criar um novo pedido
/// Orquestra a criação, persistência e notificação de novo pedido
/// Implementa ICreateOrderUseCase (Input Port) da arquitetura hexagonal
/// </summary>
public class CreateOrderService : ICreateOrderUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationPort _notification;
    private readonly ILogger<CreateOrderService> _logger;

    public CreateOrderService(
        IUnitOfWork unitOfWork,
        INotificationPort notification,
        ILogger<CreateOrderService> logger)
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
    /// Executa a criação de um novo pedido
    /// </summary>
    public async Task<OrderResponse> ExecuteAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando CreateOrderUseCase com CustomerId: {CustomerId}", request?.CustomerId);

        if (request == null)
            throw InvalidRequestException.CreateForNullField(nameof(request));

        // Validar request (validação básica)
        if (string.IsNullOrWhiteSpace(request.CustomerId))
            throw InvalidRequestException.CreateForNullField(nameof(request.CustomerId));

        if (request.Items == null || request.Items.Count == 0)
            throw InvalidRequestException.CreateForEmptyCollection(nameof(request.Items));

        // Iniciar transação
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            try
            {
                // Criar agregado de domínio
                var orderId = OrderId.Create();
                var customerId = CustomerId.Parse(request.CustomerId);
                var order = Order.CreateOrder(orderId, customerId);

                _logger.LogInformation("Agregado Order criado com sucesso. OrderId: {OrderId}, CustomerId: {CustomerId}", 
                    orderId.Value, customerId.Value);

                // Adicionar itens ao pedido
                foreach (var itemDto in request.Items)
                {
                    var amount = OrderAmount.Create(itemDto.UnitPrice);
                    var productId = ProductId.Create(itemDto.ProductId);
                    var orderItem = new OrderItem(productId, itemDto.Quantity, amount);
                    order.AddItem(orderItem);
                }

                _logger.LogInformation("Itens adicionados ao pedido com sucesso. ItemCount: {ItemCount}", 
                    order.Items.Count);

                // Persistir pedido
                await _unitOfWork.Orders.SaveAsync(order, cancellationToken);

                // Confirmar transação
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("CreateOrderUseCase concluído com sucesso. OrderId: {OrderId}, ItemCount: {ItemCount}", 
                    orderId.Value, order.Items.Count);

                // Notificar cliente (assincrono, não bloqueia o retorno)
                _ = _notification.SendOrderConfirmationAsync(
                    request.CustomerId,
                    order.OrderId.Value.ToString(),
                    cancellationToken);

                // Converter para DTO e retornar
                return Mappers.OrderMapper.ToResponse(order);
            }
            catch (DomainException ex)
            {
                // Traduzir exceção de domínio para exceção de aplicação
                _logger.LogWarning(ex, "Erro de domínio ao criar pedido. CustomerId: {CustomerId}", request.CustomerId);
                throw new InvalidOrderStateException($"Erro ao criar pedido: {ex.Message}");
            }
            catch (Exception ex) when (!(ex is OrderHub.Application.Exceptions.ApplicationException))
            {
                // Traduzir outras exceções para RepositoryException se for do repositório
                _logger.LogError(ex, "Erro inesperado ao criar pedido. CustomerId: {CustomerId}", request.CustomerId);
                throw RepositoryException.CreateForSave(request.CustomerId, ex);
            }
        }
        catch
        {
            // Desfazer transação em caso de erro
            _logger.LogWarning("Desfazendo transação - erro ao criar pedido");
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
