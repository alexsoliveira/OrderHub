using Microsoft.Extensions.DependencyInjection;
using OrderHub.Application.Ports;
using OrderHub.Domain.Ports;
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
        // Registra uma única instância que implementa ambas as interfaces
        services.AddScoped<NotificationService>();
        
        // Mapeia NotificationService para as duas interfaces (Domain e Application)
        services.AddScoped<Domain.Ports.INotificationPort>(provider =>
            provider.GetRequiredService<NotificationService>());
        services.AddScoped<Application.Ports.INotificationPort>(provider =>
            provider.GetRequiredService<NotificationService>());
        
        // TODO: Registrar outros serviços conforme forem implementados
        // services.AddScoped<IPaymentPort, PaymentService>();
        // services.AddScoped<ILoggingService, LoggingService>();
        // services.AddScoped<IEmailService, EmailService>();
        
        return services;
    }
}


