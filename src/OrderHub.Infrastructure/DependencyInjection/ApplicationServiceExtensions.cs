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
            // Registrar Use Cases/Application Services com padrão Scoped
            // Cada requisição HTTP terá sua própria instância do serviço
            
            // Serviços de Ordem (Order Services)
            services.AddScoped<CreateOrderService>();
            services.AddScoped<GetOrderService>();
            services.AddScoped<UpdateOrderService>();
            services.AddScoped<CancelOrderService>();
            
            // TODO: Registrar Use Cases adicionais conforme forem implementados
            // services.AddScoped<IDeleteOrderUseCase, DeleteOrderUseCase>();
            // services.AddScoped<IListOrdersUseCase, ListOrdersUseCase>();
            
            return services;
        }
    }
}
