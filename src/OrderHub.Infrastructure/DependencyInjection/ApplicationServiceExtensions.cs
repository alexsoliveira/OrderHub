using Microsoft.Extensions.DependencyInjection;
using OrderHub.Application.UseCases;
using OrderHub.Application.UseCases.Orders;

namespace OrderHub.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Extensões para registro de Use Cases no container de Dependency Injection
    /// </summary>
    public static class ApplicationServiceExtensions
    {
        /// <summary>
        /// Registra todos os Use Cases (Application Services) no container de DI
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        /// <returns>Coleção de serviços para encadeamento</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registrar Use Cases com suas Input Port interfaces
            // Padrão: services.AddScoped<IInterface, ConcreteImplementation>();
            // Cada requisição HTTP terá sua própria instância do serviço (Scoped lifetime)
            // Isso implementa o princípio de Inversão de Dependência (IoC) da arquitetura hexagonal
            
            // Registrar Use Cases de Ordem (Order Use Cases)
            // Controllers injetam as interfaces, não os tipos concretos
            services.AddScoped<ICreateOrderUseCase, CreateOrderService>();
            services.AddScoped<IGetOrderUseCase, GetOrderService>();
            services.AddScoped<IUpdateOrderUseCase, UpdateOrderService>();
            services.AddScoped<ICancelOrderUseCase, CancelOrderService>();
            
            return services;
        }
    }
}
