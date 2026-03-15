namespace OrderHub.Domain.Ports;

/// <summary>
/// Output Port (Interface) para Unit of Work Pattern
/// Gerencia transações e coordena múltiplos repositórios
/// Arquitetura Hexagonal: Port que o Domain precisa para gerenciar transações e persistência
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
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirma a transação e persiste todas as mudanças
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Desfaz a transação (rollback) e descarta todas as mudanças
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se há uma transação ativa
    /// </summary>
    bool HasActiveTransaction { get; }
}
