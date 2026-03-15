using Microsoft.EntityFrameworkCore;
using OrderHub.Application.DTOs;
using OrderHub.Application.Ports;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.Exceptions;
using OrderHub.Domain.Ports;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Adapters.Outbound.Persistence.Repositories;

/// <summary>
/// Implementação do repositório de pedidos usando Entity Framework Core
/// Fornece acesso a dados de Order através de patterns assíncronos
/// Implementa as interfaces IOrderRepository (Application Port e Domain Port)
/// </summary>
public class OrderRepository : Application.Ports.IOrderRepository
{
    private readonly OrderHubDbContext _context;

    /// <summary>
    /// Construtor que recebe o DbContext
    /// </summary>
    /// <param name="context">Contexto do banco de dados</param>
    /// <exception cref="ArgumentNullException">Lançado quando context é nulo</exception>
    public OrderRepository(OrderHubDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Recupera um pedido pelo seu identificador
    /// </summary>
    /// <param name="orderId">ID do pedido em formato string (GUID)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>OrderResponse ou null se não encontrado</returns>
    public async Task<OrderResponse?> GetByIdAsync(string orderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return null;

        if (!Guid.TryParse(orderId, out var id))
            return null;

        var order = await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId.Value == id, cancellationToken);

        if (order == null)
            return null;

        return MapToOrderResponse(order);
    }

    /// <summary>
    /// Recupera todos os pedidos de um cliente
    /// </summary>
    /// <param name="customerId">ID do cliente em formato string (GUID)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de OrderResponse do cliente</returns>
    public async Task<List<OrderResponse>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return new List<OrderResponse>();

        if (!Guid.TryParse(customerId, out var id))
            return new List<OrderResponse>();

        var orders = await _context.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId.Value == id)
            .ToListAsync(cancellationToken);

        return orders.Select(MapToOrderResponse).ToList();
    }

    /// <summary>
    /// Persiste um novo pedido ou atualiza um existente
    /// </summary>
    /// <param name="order">Entidade Order para persistir</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <exception cref="ArgumentNullException">Lançado quando order é nulo</exception>
    public async Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        // Verificar se já existe
        var existing = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderId.Value == order.OrderId.Value, cancellationToken);

        if (existing != null)
        {
            // Atualizar existente - remover itens antigos e adicionar novos
            _context.Entry(existing).State = EntityState.Detached;
            _context.Orders.Update(order);
        }
        else
        {
            // Adicionar novo
            await _context.Orders.AddAsync(order, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Remove um pedido pelo seu identificador
    /// </summary>
    /// <param name="orderId">ID do pedido em formato string (GUID)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <exception cref="InvalidOrderException">Lançado quando pedido não existe</exception>
    public async Task DeleteAsync(string orderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new InvalidOrderException("OrderId não pode estar vazio");

        if (!Guid.TryParse(orderId, out var id))
            throw new InvalidOrderException($"Formato de OrderId inválido: {orderId}");

        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderId.Value == id, cancellationToken);

        if (order == null)
            throw new InvalidOrderException($"Pedido com ID {orderId} não encontrado");

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica se um pedido existe
    /// </summary>
    /// <param name="orderId">ID do pedido em formato string (GUID)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se existe, False caso contrário</returns>
    public async Task<bool> ExistsAsync(string orderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return false;

        if (!Guid.TryParse(orderId, out var id))
            return false;

        return await _context.Orders
            .AnyAsync(o => o.OrderId.Value == id, cancellationToken);
    }

    #region Domain.Ports.IOrderRepository Implementation

    /// <summary>
    /// Implementação explícita para Domain Port: Recupera agregado Order usando ValueObject OrderId
    /// </summary>
    async Task<Order?> Domain.Ports.IOrderRepository.GetByIdAsync(OrderId orderId, CancellationToken cancellationToken)
    {
        if (orderId == null)
            return null;

        var order = await _context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId.Value == orderId.Value, cancellationToken);

        return order;
    }

    /// <summary>
    /// Implementação explícita para Domain Port: Recupera agregados Order de um cliente
    /// </summary>
    async Task<List<Order>> Domain.Ports.IOrderRepository.GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            return new List<Order>();

        if (!Guid.TryParse(customerId, out var id))
            return new List<Order>();

        var orders = await _context.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId.Value == id)
            .ToListAsync(cancellationToken);

        return orders;
    }

    /// <summary>
    /// Implementação explícita para Domain Port: Remove usando ValueObject OrderId
    /// </summary>
    async Task Domain.Ports.IOrderRepository.DeleteAsync(OrderId orderId, CancellationToken cancellationToken)
    {
        if (orderId == null)
            throw new InvalidOrderException("OrderId não pode estar vazio");

        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.OrderId.Value == orderId.Value, cancellationToken);

        if (order == null)
            throw new InvalidOrderException($"Pedido com ID {orderId.Value} não encontrado");

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Implementação explícita para Domain Port: Verifica existência usando ValueObject OrderId
    /// </summary>
    async Task<bool> Domain.Ports.IOrderRepository.ExistsAsync(OrderId orderId, CancellationToken cancellationToken)
    {
        if (orderId == null)
            return false;

        return await _context.Orders
            .AnyAsync(o => o.OrderId.Value == orderId.Value, cancellationToken);
    }

    #endregion

    /// <summary>
    /// Mapeia entidade Order para DTO OrderResponse
    /// </summary>
    /// <param name="order">Entidade Order</param>
    /// <returns>DTO OrderResponse</returns>
    private static OrderResponse MapToOrderResponse(Order order)
    {
        var items = order.Items.Select(item => new OrderItemResponse
        {
            ProductId = item.ProductId.Value,
            Quantity = item.Quantity,
            UnitPrice = item.Amount.Value,
            SubTotal = item.GetSubtotal()
        }).ToList();

        var totalAmount = items.Sum(i => i.SubTotal);

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
}
