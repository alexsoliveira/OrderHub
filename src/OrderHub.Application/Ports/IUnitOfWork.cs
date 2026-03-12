namespace OrderHub.Application.Ports;

/// <summary>
/// Port (Interface) para Unit of Work Pattern
/// Gerencia transações e coordena múltiplos repositórios
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Acesso ao repositório de pedidos
    /// </summary>
    IOrderRepository Orders { get; }

    /// <summary>
    /// Inicia uma nova transação
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma a transação e persiste todas as mudanças
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Desfaz a transação (rollback) e descarta todas as mudanças
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se há uma transação ativa
    /// </summary>
    bool HasActiveTransaction { get; }
}
