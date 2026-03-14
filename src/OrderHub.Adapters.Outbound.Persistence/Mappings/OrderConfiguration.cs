using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Adapters.Outbound.Persistence.Mappings;

/// <summary>
/// Configuração de mapeamento da entidade Order para o Entity Framework Core
/// Define como a entidade Order deve ser persistida no banco de dados
/// Implementa padrão Fluent API
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    /// <summary>
    /// Configura o mapeamento da entidade Order
    /// </summary>
    /// <param name="builder">Builder para configurar o mapeamento</param>
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Nome da tabela em português
        builder.ToTable("Pedidos");

        // Configurar chave primária
        builder.HasKey(o => o.OrderId);

        // Converter OrderId ValueObject para Guid no banco
        builder.Property(o => o.OrderId)
            .HasConversion(
                id => id.Value,
                value => OrderId.Create(value))
            .HasColumnName("OrderId")
            .IsRequired();

        // Converter CustomerId ValueObject para Guid no banco
        builder.Property(o => o.CustomerId)
            .HasConversion(
                id => id.Value,
                value => CustomerId.Create(value))
            .HasColumnName("CustomerId")
            .IsRequired();

        // Configurar coluna OrderDate
        builder.Property(o => o.OrderDate)
            .HasColumnType("datetime2")
            .IsRequired();

        // Converter OrderStatus Enum para string no banco
        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasColumnType("nvarchar(50)")
            .IsRequired();

        // Configurar relacionamento um-para-muitos: Order -> OrderItems
        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Adicionar índices para performance em buscas frequentes
        builder.HasIndex(o => o.CustomerId)
            .HasDatabaseName("IX_Pedidos_CustomerId");

        builder.HasIndex(o => o.OrderDate)
            .HasDatabaseName("IX_Pedidos_OrderDate");

        builder.HasIndex(o => o.Status)
            .HasDatabaseName("IX_Pedidos_Status");

        // Configuração de timestamps para auditoria (shadow properties)
        builder.Property<DateTime>("CreatedAt")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property<DateTime>("UpdatedAt")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate()
            .IsRequired();

        // Ignora as propriedades de navegação de read-only se necessário
        // builder.Ignore(o => o.Items); // Remover se Items deve ser navegável
    }
}
