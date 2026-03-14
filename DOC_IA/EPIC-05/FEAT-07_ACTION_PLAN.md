# FEAT-07 | Plano de Ação - Persistência com EF Core

**Data**: 13 de Março de 2026  
**Feature**: FEAT-07 | Persistência com EF Core  
**Status**: Em Planejamento  
**Baseado em**: Azure DevOps Issue 72 (FEAT-07 | Persistência com EF Core)  
**Total de Tasks**: 6  
**Sprint**: Sprint 1  
**Consultado via MCP**: ✅ GetWorkItem (ID 72) e GetChildWorkItems  

---

## 📋 Visão Geral

Implementação da camada de persistência usando **Entity Framework Core** seguindo os princípios de **Hexagonal Architecture** e padrão **Repository Pattern**.

O projeto integrará o banco de dados SQL Server com a lógica de domínio através de adapters de saída bem definidos.

Este documento detalha as **6 tasks reais** da FEAT-07 conforme definidas no Azure DevOps Issue 72.

---

## 🎯 Objetivo

Criar uma camada de persistência robusta que:
- **Configura Entity Framework Core** para .NET 8
- **Implementa DbContext** adequadamente
- **Define Mappings** entre Domain e Database
- **Implementa Repository Pattern** com acesso a dados
- **Gerencia Migrations** de banco de dados
- **Configura SQL Server** como banco de produção

---

## 📊 Tasks da FEAT-07

### ✅ TASK-40: Configurar Entity Framework Core

**ID Azure DevOps**: 123  
**Título**: TASK-40 | Configurar Entity Framework Core  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Configurar Entity Framework Core 8.0 como ORM principal para persistência de dados.

**O que fazer**:

1. **Criar projeto Persistence**:
   ```bash
   cd src/
   dotnet new classlib -n OrderHub.Adapters.Outbound.Persistence -f net8.0
   cd ..
   dotnet sln add src/OrderHub.Adapters.Outbound.Persistence/OrderHub.Adapters.Outbound.Persistence.csproj
   ```

2. **Adicionar packages NuGet**:
   ```bash
   cd src/OrderHub.Adapters.Outbound.Persistence
   dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
   dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
   dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
   ```

3. **Configurar referências de projeto**:
   ```bash
   dotnet add reference ../OrderHub.Domain/OrderHub.Domain.csproj
   dotnet add reference ../OrderHub.Application/OrderHub.Application.csproj
   ```

4. **Criar estrutura de pastas**:
   ```
   OrderHub.Adapters.Outbound.Persistence/
   ├── Mappings/
   ├── Repositories/
   ├── Migrations/
   └── OrderHubDbContext.cs
   ```

**Checklist**:
- [ ] Projeto criado e compilável
- [ ] Todos packages NuGet adicionados
- [ ] Referências de projeto configuradas
- [ ] Estrutura de pastas criada
- [ ] Arquivo `.csproj` com `nullable` enabled
- [ ] Sem warnings de compilação
- [ ] Commit: `feat: Setup Entity Framework Core configuration`

**Critério de Aceitação**:
- ✅ Projeto compila sem erros
- ✅ Packages EF Core instalados corretamente
- ✅ Estrutura de pastas implementada
- ✅ Pode-se criar DbContext com sucesso

---

### ✅ TASK-41: Criar DbContext

**ID Azure DevOps**: 124  
**Título**: TASK-41 | Criar DbContext  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Implementar `OrderHubDbContext` que gerencia as entidades e configurações do banco de dados.

**Estrutura Esperada**:
```csharp
namespace OrderHub.Adapters.Outbound.Persistence
{
    public class OrderHubDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public OrderHubDbContext(DbContextOptions<OrderHubDbContext> options) 
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Aplicar configurações
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(OrderHubDbContext).Assembly);
        }
    }
}
```

**O que fazer**:
- [ ] Criar classe `OrderHubDbContext` herança de `DbContext`
- [ ] Adicionar `DbSet<Order> Orders { get; set; }`
- [ ] Adicionar `DbSet<OrderItem> OrderItems { get; set; }`
- [ ] Implementar construtor recebendo `DbContextOptions<OrderHubDbContext>`
- [ ] Implementar `OnModelCreating()` com configurações fluentes
- [ ] Aplicar auto-configurations via `ApplyConfigurationsFromAssembly()`
- [ ] Configurar table names em português: `Pedidos` e `ItensPedido`
- [ ] Implementar shadow properties para auditoria (CreatedAt, UpdatedAt)
- [ ] Arquivo em `src/OrderHub.Adapters.Outbound.Persistence/OrderHubDbContext.cs`
- [ ] Commit: `feat: Implement OrderHubDbContext`

**Critério de Aceitação**:
- ✅ DbContext criado e configurável via DI
- ✅ Entidades mapeadas corretamente
- ✅ ApplyConfigurationsFromAssembly funciona
- ✅ Compila sem errors ou warnings

---

### ✅ TASK-42: Criar mapeamento da entidade Order

**ID Azure DevOps**: 125  
**Título**: TASK-42 | Criar mapeamento da entidade Order  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Implementar Entity Type Configurations para mapear as entidades Order e OrderItem com o banco de dados.

**Estrutura Esperada**:
```csharp
namespace OrderHub.Adapters.Outbound.Persistence.Mappings
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Pedidos");
            
            builder.HasKey(o => o.OrderId);
            
            builder.Property(o => o.OrderId)
                .HasConversion(id => id.Value, value => new OrderId(value))
                .IsRequired();
            
            builder.Property(o => o.CustomerId)
                .HasConversion(id => id.Value, value => new CustomerId(value))
                .IsRequired();
            
            builder.Property(o => o.OrderDate)
                .IsRequired();
            
            builder.Property(o => o.Status)
                .HasConversion<string>()
                .IsRequired();
            
            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasQueryFilter(o => o.Status != OrderStatus.Deleted);
        }
    }
}
```

**O que fazer**:
- [ ] Criar classe `OrderConfiguration implements IEntityTypeConfiguration<Order>`
- [ ] Implementar método `Configure(EntityTypeBuilder<Order> builder)`
- [ ] Configurar nome da tabela: `Pedidos`
- [ ] Configurar chave primária: `OrderId`
- [ ] Adicionar property converters para Value Objects (OrderId, CustomerId)
- [ ] Mapear OrderStatus como string no banco
- [ ] Configurar relacionamento um-para-muitos: Order → OrderItems
- [ ] Adicionar On Delete Cascade para órfãos
- [ ] Implementar soft delete via Query Filter (opcional)
- [ ] Criar `OrderItemConfiguration` para `OrderItem`
- [ ] Arquivo em `src/OrderHub.Adapters.Outbound.Persistence/Mappings/OrderConfiguration.cs`
- [ ] Commit: `feat: Implement Entity Framework mappings for Order aggregate`

**Critério de Aceitação**:
- ✅ Value Objects convertidos corretamente
- ✅ Enums mapeados como strings
- ✅ Relacionamentos bem definidos
- ✅ Shadow properties para auditoria configuradas
- ✅ DbContext aplica configurações automaticamente

---

### ✅ TASK-43: Implementar OrderRepository (EF Core)

**ID Azure DevOps**: 126  
**Título**: TASK-43 | Implementar OrderRepository (EF Core)  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Implementar padrão Repository para abstração de acesso a dados de Order.

**Estrutura Esperada**:
```csharp
namespace OrderHub.Adapters.Outbound.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderHubDbContext _context;

        public OrderRepository(OrderHubDbContext context)
        {
            _context = context;
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> GetByIdAsync(OrderId orderId)
        {
            return await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId)
                ?? throw new OrderNotFoundException(orderId);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<bool> DeleteAsync(OrderId orderId)
        {
            var order = await GetByIdAsync(orderId);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
```

**O que fazer**:
- [ ] Criar interface `IOrderRepository` em `src/OrderHub.Application/Ports/IOrderRepository.cs`
- [ ] Criar classe `OrderRepository implements IOrderRepository`
- [ ] Implementar `CreateAsync(Order order)`
- [ ] Implementar `GetByIdAsync(OrderId orderId)`
- [ ] Implementar `GetAllAsync()`
- [ ] Implementar `UpdateAsync(Order order)`
- [ ] Implementar `DeleteAsync(OrderId orderId)`
- [ ] Implementar `GetByCustomerIdAsync(CustomerId customerId)` (bonus)
- [ ] Adicionar métodos assíncronos (async/await)
- [ ] Adicionar tratamento de exceções (OrderNotFoundException)
- [ ] Arquivo em `src/OrderHub.Adapters.Outbound.Persistence/Repositories/OrderRepository.cs`
- [ ] Commit: `feat: Implement OrderRepository with EF Core`

**Critério de Aceitação**:
- ✅ Todas operações CRUD funcionam
- ✅ Métodos assíncronos (async/await)
- ✅ Exception handling apropriado
- ✅ Implements IOrderRepository corretamente
- ✅ Desacoplado do DbContext (via port/interface)

---

### ✅ TASK-44: Implementar migrations do banco

**ID Azure DevOps**: 127  
**Título**: TASK-44 | Implementar migrations do banco  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Criar e gerenciar migrations de banco de dados para versioning de schema.

**O que fazer**:

1. **Criar migration inicial**:
   ```bash
   cd src/OrderHub.Adapters.Outbound.Persistence
   dotnet ef migrations add InitialCreate \
       --startup-project ../OrderHub.Adapters.Inbound.Api/OrderHub.Adapters.Inbound.Api.csproj
   ```

2. **Estrutura de Migrations**:
   ```
   Migrations/
   ├── 20260313000000_InitialCreate.cs
   ├── 20260313000001_AddOrderTimestamps.cs
   └── OrderHubDbContextModelSnapshot.cs
   ```

3. **Configuração de connection string** em `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrderHubDb;Trusted_Connection=true;"
     }
   }
   ```

**Checklist**:
- [ ] Migration InitialCreate criada com `add-migration`
- [ ] Tabelas: `Pedidos`, `ItensPedido` definidas
- [ ] Índices em colunas de busca frequent (OrderId, CustomerId)
- [ ] Constraints de FK configurados corretamente
- [ ] Migration pode ser aplicada localmente
- [ ] Migration pode ser revertida sem erro
- [ ] `ModelSnapshot` atualizado automaticamente
- [ ] Connection string em `appsettings.Development.json`
- [ ] Arquivo `.sql` gerado para review (opcional)
- [ ] Commit: `feat: Add initial EF Core migrations`

**Critério de Aceitação**:
- ✅ `dotnet ef database update` funciona
- ✅ `dotnet ef database update --connection [sql]` cria schema
- ✅ Tabelas criadas com estrutura correta
- ✅ Migrations reversíveis
- ✅ Sem data loss em rollback

---

### ✅ TASK-45: Configurar SQL Server

**ID Azure DevOps**: 128  
**Título**: TASK-45 | Configurar SQL Server  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Configurar conexão com SQL Server e setup do banco de desenvolvimento/produção.

**O que fazer**:

1. **Conexão string com variáveis de ambiente**:
   ```json
   // appsettings.json (development)
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrderHubDb;Trusted_Connection=true;Encrypt=false;"
     }
   }
   
   // appsettings.Production.json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=OrderHubDb;User Id=sa;Password=YOUR_PASSWORD;Encrypt=true;"
     }
   }
   ```

2. **Registrar contexto em DI** (`Program.cs`):
   ```csharp
   services.AddDbContext<OrderHubDbContext>(options =>
       options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
   ```

3. **Seed data inicial** (opcional):
   - Criar classe `OrderHubDbContextSeeder`
   - Popular dados iniciais de teste
   - Executar em `Program.cs` se Environment == Development

**Checklist**:
- [ ] Connection string configurada corretamente
- [ ] SQL Server LocalDB instalado (ou Azure SQL)
- [ ] Variáveis de ambiente mapeadas (USER SECRETS em dev)
- [ ] DbContext registrado em DI (Program.cs)
- [ ] Database pode ser criado: `dotnet ef database update`
- [ ] Connection pooling configurado (máx 100 pools)
- [ ] Timeout em 30 segundos configurado
- [ ] Logging de queries EF (para debug)
- [ ] Sem hardcoded secrets
- [ ] Commit: `feat: Configure SQL Server connection and DI registration`

**Critério de Aceitação**:
- ✅ Aplicação inicia sem erro de conexão
- ✅ `dotnet ef database update` funciona
- ✅ Pode fazer queries básicas (select, insert)
- ✅ Sem exposição de secrets em código
- ✅ Performance adequada (< 500ms primeira query)

---

## 📈 Sequência Recomendada de Execução

| # | Task | Objetivo | Estimativa | Dependências |
|---|------|----------|-----------|--------------|
| 1 | TASK-40 | Setup EF Core | 30 min | Nenhuma |
| 2 | TASK-41 | Criar DbContext | 45 min | TASK-40 |
| 3 | TASK-42 | Mappings Order | 1h | TASK-41 |
| 4 | TASK-45 | Configurar SQL Server | 30 min | TASK-41 |
| 5 | TASK-44 | Migrations Banco | 30 min | TASK-42, TASK-45 |
| 6 | TASK-43 | OrderRepository | 1h | TASK-42, TASK-44 |
| | **TOTAL** | | **~4 horas** | |

---

## ✅ Checklist Geral FEAT-07

### Setup
- [ ] Projeto Persistence criado
- [ ] EF Core packages instalados
- [ ] Referências configuradas
- [ ] Compila sem erros

### DbContext
- [ ] OrderHubDbContext implementado
- [ ] DbSets configurados
- [ ] OnModelCreating aplica configs
- [ ] Pode ser injetado via DI

### Mappings
- [ ] OrderConfiguration implementado
- [ ] OrderItemConfiguration implementado
- [ ] Value Objects convertidos corretamente
- [ ] Enums como strings no banco

### Persistência
- [ ] IOrderRepository definida
- [ ] OrderRepository implementado
- [ ] CRUD operations funcionam
- [ ] Async/await implementado

### Database
- [ ] Migrations criadas
- [ ] Schema correto no banco
- [ ] Connection string configurada
- [ ] DI do DbContext em Program.cs
- [ ] Sem secrets expostos

### Qualidade
- [ ] Compila sem warnings
- [ ] Nomenclatura PascalCase
- [ ] Tabelas em português
- [ ] Comentários em métodos públicos

---

## 🔗 Sincronização com Azure DevOps

Para cada task completada, atualizar o status no Azure DevOps:

```powershell
# Exemplo: Marcar TASK-40 como Done
mcp_azure-devops-_UpdateWorkItem -id 123 -state "Done"

# Marcação em lote (ao finalizar full FEAT-07)
for ($id in 123, 124, 125, 126, 127, 128) {
    mcp_azure-devops-_UpdateWorkItem -id $id -state "Done"
}

# Marcar Issue FEAT-07 como Done
mcp_azure-devops-_UpdateWorkItem -id 72 -state "Done"
```

---

## 📝 Notas Importantes

1. **Fonte de Verdade**: Este plano é **100% baseado em Azure DevOps Issue 72**
2. **Sem Especulação**: Cada task corresponde exatamente ao que foi definido no MCP
3. **Rastreabilidade**: Cada task tem ID Azure DevOps referenciado
4. **Commits Atomizados**: Um commit por task (ou lógico)
5. **Review**: Antes de marcar Done, validar todos aceitate criteria completados
6. **SQL Server**: LocalDB para dev, Azure SQL/Premium para produção
7. **Migrations**: Sempre versionar no git, nunca dropar tables em produção
8. **Performance**: Considerar índices e batch operations após baseline

---

## 🎓 Referências

- [EF Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [FluentAPI Mapping](https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding)
- [Repository Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)
- [ValueObject Conversion](https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions)
