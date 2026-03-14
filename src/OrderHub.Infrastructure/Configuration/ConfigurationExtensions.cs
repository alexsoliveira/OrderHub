using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace OrderHub.Infrastructure.Configuration
{
    /// <summary>
    /// Extensões para configuração centralizada de carregamento de settings
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Carrega configurações de arquivos appsettings.json com suporte a ambiente
        /// </summary>
        /// <param name="builder">Configuration builder</param>
        /// <param name="basePath">Caminho base para os arquivos de configuração</param>
        /// <returns>Configuration builder para encadeamento</returns>
        public static IConfigurationBuilder AddCustomConfiguration(
            this IConfigurationBuilder builder, 
            string basePath)
        {
            // Determina o caminho base se não fornecido
            var path = string.IsNullOrEmpty(basePath) 
                ? Directory.GetCurrentDirectory() 
                : basePath;

            // Carrega configuração base (obrigatória)
            builder.AddJsonFile(Path.Combine(path, "appsettings.json"), optional: false, reloadOnChange: true);
            
            // Carrega configuração específica do ambiente (opcional)
            // Sobrescreve as configurações do arquivo base
            builder.AddJsonFile(
                Path.Combine(path, $"appsettings.{GetEnvironment()}.json"), 
                optional: true, 
                reloadOnChange: true);
            
            // Carrega variáveis de ambiente do sistema
            // Permite sobrescrever valores via variáveis de ambiente
            builder.AddEnvironmentVariables();

            return builder;
        }

        /// <summary>
        /// Obtém o ambiente atual (Development, Staging, Production)
        /// Lê a variável ASPNETCORE_ENVIRONMENT ou usa Production como padrão
        /// </summary>
        /// <returns>Nome do ambiente</returns>
        private static string GetEnvironment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        }
    }
}
