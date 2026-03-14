using Microsoft.Extensions.DependencyInjection;
using OrderHub.Adapters.Outbound.Persistence.Repositories;
using OrderHub.Application.Ports;

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
            // Registrar Repositories com padrão Scoped
            // Cada requisição HTTP terá sua própria instância do repositório
            
            // Repositório de Pedidos (Order Repository)
            // Implementa a interface IOrderRepository (Output Port definida no Domain)
            services.AddScoped<IOrderRepository, OrderRepository>();
            
            // TODO: Registrar otros repositories conforme forem implementados
            // services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            // services.AddScoped<ICustomerRepository, CustomerRepository>();
            
            return services;
        }
    }
}
