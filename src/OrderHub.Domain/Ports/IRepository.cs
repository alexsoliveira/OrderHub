namespace OrderHub.Application.Ports;

/// <summary>
/// Generic Repository Port (Interface) para persistência
/// Define contrato padrão para operações CRUD de qualquer entidade
/// Oferece maior flexibilidade e reutilização de código
/// </summary>
/// <typeparam name="TEntity">Tipo de entidade (Aggregate ou Entity)</typeparam>
/// <typeparam name="TId">Tipo de identificador (Guid, string, int, etc.)</typeparam>
public interface IRepository<TEntity, TId> where TEntity : class
{
    /// <summary>
    /// Recupera uma entidade pelo seu identificador único
    /// </summary>
    /// <param name="id">Identificador da entidade</param>
    /// <param name="cancellationToken">Token para cancelamento assíncrono</param>
    /// <returns>Entidade se encontrada, null caso contrário</returns>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupera todas as entidades do repositório
    /// </summary>
    /// <param name="cancellationToken">Token para cancelamento assíncrono</param>
    /// <returns>Lista de todas as entidades (vazia se nenhuma existe)</returns>
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona uma nova entidade ou atualiza uma existente
    /// Segue padrão "Insert or Update" (Upsert)
    /// </summary>
    /// <param name="entity">Entidade a ser persistida</param>
    /// <param name="cancellationToken">Token para cancelamento assíncrono</param>
    /// <exception cref="ArgumentNullException">Se entity é nulo</exception>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove uma entidade pelo seu identificador
    /// </summary>
    /// <param name="id">Identificador da entidade a remover</param>
    /// <param name="cancellationToken">Token para cancelamento assíncrono</param>
    /// <returns>True se entidade foi removida, false se não encontrada</returns>
    Task<bool> RemoveAsync(TId id, CancellationToken cancellationToken = default);
}
