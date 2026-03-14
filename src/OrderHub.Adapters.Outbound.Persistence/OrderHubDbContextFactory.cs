using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderHub.Adapters.Outbound.Persistence;

/// <summary>
/// Factory para criar instâncias de OrderHubDbContext em tempo de design (para migrations)
/// Implementa IDesignTimeDbContextFactory para suporte a EF Core migrations
/// </summary>
public class OrderHubDbContextFactory : IDesignTimeDbContextFactory<OrderHubDbContext>
{
    /// <summary>
    /// Cria uma instância de OrderHubDbContext para uso em migrations
    /// </summary>
    /// <param name="args">Argumentos de linha de comando</param>
    /// <returns>Nova instância de OrderHubDbContext configurada</returns>
    public OrderHubDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrderHubDbContext>();

        // Connection string para desenvolvimento local
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=OrderHubDb;Trusted_Connection=true;Encrypt=false;";

        optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.CommandTimeout(30);
        });

        return new OrderHubDbContext(optionsBuilder.Options);
    }
}
