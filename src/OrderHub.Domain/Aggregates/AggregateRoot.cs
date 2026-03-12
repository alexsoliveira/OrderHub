namespace OrderHub.Domain.Aggregates;

/// <summary>
/// Classe base para Agregados (Aggregate Roots) no padrão Hexagonal Architecture
/// </summary>
public abstract class AggregateRoot
{
    /// <summary>
    /// Eventos de domínio associados a este agregado
    /// </summary>
    private readonly List<object> _domainEvents = new();

    /// <summary>
    /// Coleção somente leitura de eventos de domínio
    /// </summary>
    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adiciona um evento de domínio à coleção
    /// </summary>
    protected void RaiseDomainEvent(object @event)
    {
        _domainEvents.Add(@event);
    }

    /// <summary>
    /// Limpa todos os eventos de domínio
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
