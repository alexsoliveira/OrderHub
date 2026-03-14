using Microsoft.Extensions.DependencyInjection;
using OrderHub.Application.Ports;
using OrderHub.Infrastructure.Services;

namespace OrderHub.Infrastructure.DependencyInjection;

/// <summary>
/// Extensões para registro de Serviços de Infraestrutura no container de Dependency Injection
/// Registra implementações de Output Ports que dependem de infraestrutura externa
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Registra todos os serviços de infraestrutura (notificações, logging, etc)
    /// </summary>
    /// <param name="services">Coleção de serviços</param>
    /// <returns>Coleção de serviços para encadeamento</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Registrar Output Ports de Infraestrutura com padrão Scoped
        
        // Serviço de Notificações
        // Implementa INotificationPort (Output Port definida no Application)
        // Responsável por enviar notificações (email, SMS, push, etc)
        services.AddScoped<INotificationPort, NotificationService>();
        
        // TODO: Registrar outros serviços conforme forem implementados
        // services.AddScoped<IPaymentPort, PaymentService>();
        // services.AddScoped<ILoggingService, LoggingService>();
        // services.AddScoped<IEmailService, EmailService>();
        
        return services;
    }
}
