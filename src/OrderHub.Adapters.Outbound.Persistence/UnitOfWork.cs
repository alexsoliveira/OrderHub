using OrderHub.Application.Ports;
using Microsoft.EntityFrameworkCore.Storage;
using OrderHub.Adapters.Outbound.Persistence.Repositories;

namespace OrderHub.Adapters.Outbound.Persistence;

/// <summary>
/// Implementação do padrão Unit of Work usando Entity Framework Core
/// Coordena transações e acesso a repositórios
/// Implementa Application.Ports.IUnitOfWork que estende Domain.Ports.IUnitOfWork
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly OrderHubDbContext _context;
    private IDbContextTransaction? _transaction;
    private OrderRepository? _orderRepository;

    public UnitOfWork(OrderHubDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Repositório de pedidos (lazy loaded)
    /// Retorna implementação que suporta ambas Application.Ports.IOrderRepository e Domain.Ports.IOrderRepository
    /// </summary>
    Application.Ports.IOrderRepository IUnitOfWork.Orders
    {
        get
        {
            _orderRepository ??= new OrderRepository(_context);
            return _orderRepository;
        }
    }

    /// <summary>
    /// Implementação para Domain.Ports.IUnitOfWork.Orders
    /// Retorna o mesmo repositório
    /// </summary>
    Domain.Ports.IOrderRepository Domain.Ports.IUnitOfWork.Orders
    {
        get
        {
            _orderRepository ??= new OrderRepository(_context);
            return _orderRepository;
        }
    }

    /// <summary>
    /// Verifica se há transação ativa
    /// </summary>
    public bool HasActiveTransaction => _transaction != null;

    /// <summary>
    /// Inicia uma nova transação no banco de dados
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
            throw new InvalidOperationException("Uma transação já está ativa. Finalize a transação atual antes de iniciar uma nova.");

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Confirma a transação e persiste todas as alterações no banco de dados
    /// </summary>
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Salvar todas as mudanças de contexto
            await _context.SaveChangesAsync(cancellationToken);

            // Confirmar a transação se existir
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            // Se houver erro, fazer rollback automáticos
            await RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            // Limpar a transação
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    /// <summary>
    /// Desfaz a transação (rollback) e descarta todas as alterações
    /// </summary>
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    /// <summary>
    /// Dispose assíncrono do UnitOfWork
    /// Limpa transações e contexto de banco de dados
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
