using OrderHub.Domain.Ports;

namespace OrderHub.Application.Ports;

/// <summary>
/// Application-level Port (Interface) para Unit of Work Pattern
/// Estende Domain.Ports.IUnitOfWork e providencia repositórios tipados para a Application layer
/// </summary>
public interface IUnitOfWork : Domain.Ports.IUnitOfWork
{
    /// <summary>
    /// Acesso ao repositório de pedidos com métodos Application-layer (working com DTOs)
    /// Sobrescreve a propriedade de Domain para retornar um tipo mais específico
    /// </summary>
    new IOrderRepository Orders { get; }
}
