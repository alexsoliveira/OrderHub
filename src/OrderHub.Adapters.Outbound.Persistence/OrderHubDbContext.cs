using Microsoft.EntityFrameworkCore;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Adapters.Outbound.Persistence;

/// <summary>
/// DbContext que gerencia as entidades e configurações do banco de dados para o OrderHub
/// Implementa o padrão Repository e Hexagonal Architecture
/// </summary>
public class OrderHubDbContext : DbContext
{
    /// <summary>
    /// Conjunto de pedidos (Aggregate Root)
    /// </summary>
    public DbSet<Order> Orders { get; set; } = null!;

    /// <summary>
    /// Conjunto de itens de pedido
    /// </summary>
    public DbSet<OrderItem> OrderItems { get; set; } = null!;

    /// <summary>
    /// Construtor que recebe as opções de configuração do DbContext
    /// </summary>
    /// <param name="options">Opções de configuração do DbContext</param>
    public OrderHubDbContext(DbContextOptions<OrderHubDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Configura o modelo de dados, mapeamentos e comportamentos do EF Core
    /// Aplica automaticamente todas as configurações de entidades da assembly
    /// </summary>
    /// <param name="modelBuilder">Builder para configurar o modelo</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações de IEntityTypeConfiguration da assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderHubDbContext).Assembly);

        // Configurações globais podem ser adicionadas aqui
        // Exemplo: Converter enums para strings globalmente
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType.IsEnum)
                {
                    property.SetColumnType("nvarchar(50)");
                }
            }
        }
    }

    /// <summary>
    /// Configuração adicional do DbContext (ex: change tracking)
    /// </summary>
    /// <param name="optionsBuilder">Builder para configurar opções</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Configurações adicionais podem ser adicionadas aqui
        // Exemplo: optionsBuilder.EnableSensitiveDataLogging();
    }
}
