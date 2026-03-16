using OrderHub.Application.DTOs;
using OrderHub.Application.Exceptions;
using OrderHub.Domain.Ports;

namespace OrderHub.Application.UseCases.Orders;

/// <summary>
/// Application Service para listar todos os pedidos
/// Implementa IListOrdersUseCase (Input Port) da arquitetura hexagonal
/// Recupera todos os pedidos do sistema através do repositório
/// </summary>
public class ListOrdersService : IListOrdersUseCase
{
    private readonly IOrderRepository _orderRepository;

    public ListOrdersService(IOrderRepository orderRepository)
    {
        if (orderRepository == null)
            throw InvalidRequestException.CreateForNullField(nameof(orderRepository), "dependency injection failed");
        _orderRepository = orderRepository;
    }

    /// <summary>
    /// Executa a recuperação de todos os pedidos do sistema
    /// </summary>
    public async Task<List<OrderResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
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
                
                return await Task.FromResult(orders);
            }
            catch (Exception ex)
            {
                throw RepositoryException.CreateForGetById("*", ex);
            }
        }
        catch (OrderHub.Application.Exceptions.ApplicationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RepositoryException("Erro ao listar pedidos", "ListAsync", ex);
        }
    }
}
