using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderHub.Domain.Aggregates.Order;
using OrderHub.Domain.ValueObjects;

namespace OrderHub.Adapters.Outbound.Persistence.Mappings;

/// <summary>
/// Configuração de mapeamento da entidade OrderItem para o Entity Framework Core
/// Define como os itens de pedido devem ser persistidos no banco de dados
/// Implementa padrão Fluent API
/// </summary>
public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    /// <summary>
    /// Configura o mapeamento da entidade OrderItem
    /// </summary>
    /// <param name="builder">Builder para configurar o mapeamento</param>
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // Nome da tabela em português
        builder.ToTable("ItensPedido");

        // Configurar chave primária
        builder.HasKey(oi => oi.Id);

        // Configurar coluna Id
        builder.Property(oi => oi.Id)
            .HasColumnType("uniqueidentifier")
            .IsRequired();

        // Converter ProductId ValueObject para string no banco
        builder.Property(oi => oi.ProductId)
            .HasConversion(
                id => id.Value,
                value => ProductId.Create(value))
            .HasColumnName("ProductId")
            .HasColumnType("nvarchar(100)")
            .IsRequired();

        // Configurar coluna Quantity
        builder.Property(oi => oi.Quantity)
            .HasColumnType("int")
            .IsRequired();

        // Converter OrderAmount ValueObject - armazenar como JSON ou coluna única
        builder.Property(oi => oi.Amount)
            .HasConversion(
                amount => amount.Value,
                value => OrderAmount.Create(value))
            .HasColumnName("Amount")
            .HasColumnType("decimal(18, 2)")
            .IsRequired();

        // Armazenar moeda em coluna separada (shadow property)
        builder.Property<string>("Currency")
            .HasColumnName("Currency")
            .HasColumnType("nvarchar(3)")
            .IsRequired()
            .HasDefaultValue("BRL");

        // Adicionar índices para performance
        builder.HasIndex(oi => oi.ProductId)
            .HasDatabaseName("IX_ItensPedido_ProductId");

        // Configuração de timestamps para auditoria (shadow properties)
        builder.Property<DateTime>("CreatedAt")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property<DateTime>("UpdatedAt")
            .HasDefaultValueSql("GETUTCDATE()")
            .ValueGeneratedOnAddOrUpdate()
            .IsRequired();
    }
}
