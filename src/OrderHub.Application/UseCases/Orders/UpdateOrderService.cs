using OrderHub.Application.DTOs;
using OrderHub.Application.Exceptions;
using OrderHub.Domain.Ports;
using OrderHub.Application.UseCases;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.ValueObjects;
using OrderHub.Domain.Exceptions;

namespace OrderHub.Application.UseCases.Orders;

/// <summary>
/// Application Service para atualizar um pedido existente
/// Implementa IUpdateOrderUseCase (Input Port) da arquitetura hexagonal
/// </summary>
public class UpdateOrderService : IUpdateOrderUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderService(IUnitOfWork unitOfWork)
    {
        if (unitOfWork == null)
            throw InvalidRequestException.CreateForNullField(nameof(unitOfWork), "dependency injection failed");
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Executa a atualização de um pedido existente
    /// </summary>
    public async Task<OrderResponse> ExecuteAsync(
        UpdateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw InvalidRequestException.CreateForNullField(nameof(request));

        if (string.IsNullOrWhiteSpace(request.OrderId))
            throw InvalidRequestException.CreateForNullField(nameof(request.OrderId));

        if (request.Items == null || request.Items.Count == 0)
            throw InvalidRequestException.CreateForEmptyCollection(nameof(request.Items));

        // Iniciar transação
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            try
            {
                // Converter string para ValueObject OrderId
                if (!Guid.TryParse(request.OrderId, out var guidId))
                    throw new InvalidRequestException(nameof(request.OrderId), "deve ser um GUID válido");

                var orderId = OrderId.Create(guidId);
                var orderDomain = await _unitOfWork.Orders.GetByIdAsync(orderId, cancellationToken);

                if (orderDomain == null)
                    throw new OrderNotFoundException(request.OrderId);

                // Usar order existente do domínio
                var order = orderDomain;

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
            catch (DomainException ex)
            {
                // Traduzir exceção de domínio
                throw new InvalidOrderStateException($"Erro ao atualizar pedido: {ex.Message}");
            }
            catch (Exception ex) when (!(ex is OrderHub.Application.Exceptions.ApplicationException))
            {
                // Traduzir outras exceções
                throw RepositoryException.CreateForSave(request.OrderId, ex);
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
