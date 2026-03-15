using OrderHub.Domain.Ports;

namespace OrderHub.Application.Ports;

/// <summary>
/// Application-level Port (Interface) para notificações
/// Reexporta Domain.Ports.INotificationPort que define o contrato para envio de notificações
/// </summary>
public interface INotificationPort : Domain.Ports.INotificationPort
{
    // Reexporta o contrato de Domain.Ports.INotificationPort
    // A implementação fornece notificações para a Application layer
}
