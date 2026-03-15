using Microsoft.Extensions.DependencyInjection;
using OrderHub.Adapters.Outbound.Persistence;
using OrderHub.Adapters.Outbound.Persistence.Repositories;
using OrderHub.Application.Ports;
using OrderHub.Domain.Ports;

namespace OrderHub.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Extensões para registro de Repositories no container de Dependency Injection
    /// </summary>
    public static class RepositoryServiceExtensions
    {
        /// <summary>
        /// Registra todos os Repositories (Implementações das Output Ports) no container de DI
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        /// <returns>Coleção de serviços para encadeamento</returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Registrar Repositories e Output Ports com padrão Scoped
            // Cada requisição HTTP terá sua própria instância
            
            // Unit of Work Pattern - Coordena transações e repositórios
            // Registra tanto a interface da Application quanto da Domain
            services.AddScoped<Domain.Ports.IUnitOfWork, UnitOfWork>();
            services.AddScoped<Application.Ports.IUnitOfWork, UnitOfWork>();
            
            // Repositório de Pedidos (Order Repository)
            // Implementa as interfaces IOrderRepository tanto da Application quanto da Domain
            // A implementação suporta ambas as interfaces polimorficamente
            services.AddScoped<Application.Ports.IOrderRepository, OrderRepository>();
            services.AddScoped<Domain.Ports.IOrderRepository>(provider => 
                provider.GetRequiredService<Application.Ports.IOrderRepository>());
            
            // TODO: Registrar otros repositories conforme forem implementados
            // services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            // services.AddScoped<ICustomerRepository, CustomerRepository>();
            
            return services;
        }
    }
}
