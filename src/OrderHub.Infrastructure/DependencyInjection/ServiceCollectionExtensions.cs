using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderHub.Adapters.Outbound.Persistence;
using OrderHub.Application.Ports;

namespace OrderHub.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Extensões para configuração centralizada de Dependency Injection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registra todos os serviços de infraestrutura, aplicação e persistência
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        /// <param name="configuration">Configurações da aplicação</param>
        /// <returns>Coleção de serviços para encadeamento</returns>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Registrar serviços de aplicação (Use Cases / Input Ports)
            // Chamado através da extensão em ApplicationServiceExtensions
            ApplicationServiceExtensions.AddApplicationServices(services);
            
            // Registrar repositórios e Unit of Work (Output Ports)
            // Chamado através da extensão em RepositoryServiceExtensions
            RepositoryServiceExtensions.AddRepositories(services);
            
            // Registrar serviços de infraestrutura (Notificações, etc)
            // Chamado através da extensão em InfrastructureServiceExtensions
            InfrastructureServiceExtensions.AddInfrastructureServices(services);
            
            // Configurar banco de dados
            ConfigurePersistence(services, configuration);
            
            return services;
        }

        /// <summary>
        /// Configura a persistência (Entity Framework Core e banco de dados)
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        /// <param name="configuration">Configurações da aplicação</param>
        /// <returns>Coleção de serviços para encadeamento</returns>
        private static IServiceCollection ConfigurePersistence(
            IServiceCollection services, 
            IConfiguration configuration)
        {
            // A persistência será configurada quando a camada Persistence estiver completa
            // Por enquanto, apenas reservamos o espaço estrutural
            
            return services;
        }
    }
}

