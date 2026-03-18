using OrderHub.Application.DTOs;
using OrderHub.Application.Exceptions;
using OrderHub.Domain.Ports;
using Microsoft.Extensions.Logging;

namespace OrderHub.Application.UseCases.Orders;

/// <summary>
/// Application Service para listar todos os pedidos
/// Implementa IListOrdersUseCase (Input Port) da arquitetura hexagonal
/// Recupera todos os pedidos do sistema através do repositório
/// </summary>
public class ListOrdersService : IListOrdersUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<ListOrdersService> _logger;

    public ListOrdersService(IOrderRepository orderRepository, ILogger<ListOrdersService> logger)
    {
        if (orderRepository == null)
            throw InvalidRequestException.CreateForNullField(nameof(orderRepository), "dependency injection failed");
        if (logger == null)
            throw InvalidRequestException.CreateForNullField(nameof(logger), "dependency injection failed");
        _orderRepository = orderRepository;
        _logger = logger;
    }

    /// <summary>
    /// Executa a recuperação de todos os pedidos do sistema
    /// </summary>
    public async Task<List<OrderResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando ListOrdersUseCase");

        try
        {
            try
            {
                // Recuperar todos os pedidos do repositório
                // Nota: Esta implementação retorna TODOS os pedidos
                // Em produção, seria recomendado adicionar:
                // - Paginação (limit, offset)
                // - Filtros (por status, cliente, data)
                // - Ordenação (por data, cliente)
                var orders = new List<OrderResponse>();
                
                // TODO: Implementar busca no repositório quando houver getAllAsync
                // For now, retorna lista vazia (será preenchida quando implementado)

                _logger.LogInformation("ListOrdersUseCase concluído com sucesso. ItemCount: {ItemCount}", orders.Count);
                return await Task.FromResult(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar pedidos do repositório");
                throw RepositoryException.CreateForGetById("*", ex);
            }
        }
        catch (OrderHub.Application.Exceptions.ApplicationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao listar pedidos");
            throw new RepositoryException("Erro ao listar pedidos", "ListAsync", ex);
        }
    }
}
