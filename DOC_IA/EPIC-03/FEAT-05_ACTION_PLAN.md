# FEAT-05 | Plano de Ação - Output Ports & Infrastructure Abstractions

**Feature**: FEAT-05 | Output Ports  
**Tipo**: Infrastructure Layer - Port Interfaces  
**Epic**: EPIC-03 | Implementação dos Ports  
**Data de Criação**: 12 de Março de 2026  
**Status**: 📋 Planejado  

---

## 📋 Visão Geral

**FEAT-05** formaliza as interfaces de saída (Output Ports) da arquitetura hexagonal, definindo contratos para acesso a dados (persistência) e outras funcionalidades de infraestrutura. Esta feature complementa FEAT-04 (Input Ports) ao estabelecer a "outra ponta" da comunicação entre Application Layer e Infrastructure Layer.

**Escopo**: 4 tasks relacionadas a Output Port interfaces  
**Tempo Estimado**: 4-5 horas  
**Arquitetura**: Hexagonal Architecture (Ports & Adapters)  

---

## 🎯 Objetivos

1. ✅ Formalizar interface `IOrderRepository` - persistência de pedidos
2. ✅ Formalizar interface `IUnitOfWork` - gerenciamento de transações
3. ✅ Criar abstração de persistência genérica (optional)
4. ✅ Criar abstração de logging (optional)

---

## 📊 Tasks

### TASK-27 (ID 110): Criar interface IOrderRepository
**Status**: ⏳ To Do  
**Tempo Estimado**: 1.5 horas  
**Prioridade**: 🔴 Alta

#### Objetivo
Formalizar a interface de saída (Output Port) para persistência de agregados `Order`. Esta interface define o contrato que qualquer implementação de repositório deve cumprir (EntityFramework, MongoDB, RavenDB, etc).

#### Entregáveis
- **Arquivo**: `src/OrderHub.Application/Ports/IOrderRepository.cs`
- **Interface**: `IOrderRepository` com 4 métodos
- **Linhas Estimadas**: 35-40 linhas + documentação XML
- **Padrão**: Generic Repository Pattern (mas monolítico por agregado)

#### Métodos a Implementar

```csharp
namespace OrderHub.Application.Ports;

/// <summary>
/// Output Port para persistência de agregado Order
/// Define o contrato de acesso a dados para pedidos
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Recupera um pedido pelo seu identificador único
    /// </summary>
    Task<OrderResponse?> GetByIdAsync(string orderId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recupera todos os pedidos de um cliente específico
    /// </summary>
    Task<List<OrderResponse>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Persiste um novo pedido ou atualiza um existente
    /// </summary>
    Task SaveAsync(Order order, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Remove um pedido pelo seu identificador
    /// </summary>
    Task DeleteAsync(string orderId, CancellationToken cancellationToken = default);
}
```

#### Características
- ✅ Retorna `OrderResponse` (DTO) ao consultar
- ✅ Aceita `Order` (Entidade de Domínio) para persistência
- ✅ Suporta operações assíncronas com `CancellationToken`
- ✅ Exceções: `InvalidOperationException` para "não encontrado"
- ✅ Documentação XML completa

#### Validações
- ✅ Interface não contém lógica (apenas contrato)
- ✅ BuildTime: < 3 segundos
- ✅ Sem warnings ou erros

---

### TASK-28 (ID 111): Criar interface IUnitOfWork
**Status**: ⏳ To Do  
**Tempo Estimado**: 1.5 horas  
**Prioridade**: 🔴 Alta

#### Objetivo
Formalizar a interface de saída para gerenciamento de transações. O UnitOfWork coordena múltiplas operações de persistência em uma única transação atômica, garantindo ACID compliance. Padrão crítico para manter consistência dos agregados.

#### Entregáveis
- **Arquivo**: `src/OrderHub.Application/Ports/IUnitOfWork.cs`
- **Interface**: `IUnitOfWork` com 4 métodos
- **Linhas Estimadas**: 40-45 linhas + documentação XML
- **Padrão**: Unit of Work Pattern

#### Métodos a Implementar

```csharp
namespace OrderHub.Application.Ports;

/// <summary>
/// Output Port para gerenciamento de transações (Unit of Work)
/// Coordena múltiplas operações de persistência em transação única
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Repository para operações com agregado Order
    /// </summary>
    IOrderRepository Orders { get; }
    
    /// <summary>
    /// Inicia uma transação de banco de dados
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Confirma a transação (commit)
    /// Persiste todas as alterações de forma atômica
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Descarta a transação (rollback)
    /// Cancela todas as alterações pendentes
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
```

#### Características
- ✅ Propriedade `Orders` expõe `IOrderRepository`
- ✅ Métodos para iniciar, confirmar e desfazer transações
- ✅ Implementa `IAsyncDisposable` para cleanup de recursos
- ✅ Garante ACID properties
- ✅ Documentação XML completa

#### Padrão: Coordenação de Transações

```
Fluxo Típico:
1. Serviço inicia transação: await _unitOfWork.BeginTransactionAsync()
2. Realiza múltiplas operações: await _unitOfWork.Orders.SaveAsync()
3. Se sucesso: await _unitOfWork.CommitAsync() -> persiste tudo
4. Se erro: await _unitOfWork.RollbackAsync() -> desfaz tudo
5. Cleanup: await _unitOfWork.DisposeAsync()
```

#### Validações
- ✅ Interface não contém lógica
- ✅ BuildTime: < 3 segundos
- ✅ Sem warnings ou erros

---

### TASK-29 (ID 112): Criar abstração de persistência
**Status**: ⏳ To Do  
**Tempo Estimado**: 1.5 horas  
**Prioridade**: 🟡 Média

#### Objetivo
Criar abstrações genéricas optionais para suportar diferentes estratégias de persistência. Padrão adicional oferecendo maior flexibilidade para diferentes tipos de dados e bancos de dados.

#### Entregáveis
- **Arquivo**: `src/OrderHub.Application/Ports/IRepository.cs` (genérico)
- **Interface**: `IRepository<TEntity, TId>`
- **Linhas Estimadas**: 30-35 linhas
- **Padrão**: Generic Repository Pattern

#### Métodos a Implementar

```csharp
namespace OrderHub.Application.Ports;

/// <summary>
/// Output Port genérico para persistência (opcional)
/// Oferece contrato padrão para qualquer entidade
/// </summary>
/// <typeparam name="TEntity">Tipo de entidade (Aggregates)</typeparam>
/// <typeparam name="TId">Tipo de identificador (Guid, string, int)</typeparam>
public interface IRepository<TEntity, TId> where TEntity : class
{
    /// <summary>
    /// Recupera entidade pelo identificador
    /// </summary>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Recupera todas as entidades
    /// </summary>
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adiciona entidade (insert ou update)
    /// </summary>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Remove entidade
    /// </summary>
    Task RemoveAsync(TId id, CancellationToken cancellationToken = default);
}
```

#### Características
- ✅ Genérico para qualquer TEntity e TId
- ✅ Oferece CRUD básico padronizado
- ✅ Complementa `IOrderRepository` específica
- ✅ Optional: pode ser usado como base ou descartado
- ✅ Suporta herança: `IOrderRepository : IRepository<Order, OrderId>`

#### Nota
Esta interface é **OPCIONAL**. Pode ser útil para futuras entidades ou pode ser descartada se `IOrderRepository` for suficiente. Decisão: será criada mas há flexibilidade de não usar se não agregar valor.

#### Validações
- ✅ Genéricos bem tipados
- ✅ Sem constraints desnecessárias
- ✅ BuildTime: < 2 segundos

---

### TASK-30 (ID 113): Criar abstração de logging (opcional)
**Status**: ⏳ To Do  
**Tempo Estimado**: 1 hora  
**Prioridade**: 🟢 Baixa (Optional)

#### Objetivo
Criar abstração para logging, permitindo diferentes implementações (Console, File, Serilog, NLog, etc) sem acoplamento direto com a aplicação. Padrão útil para auditoria, troubleshooting e monitoramento em produção.

#### Entregáveis
- **Arquivo**: `src/OrderHub.Application/Ports/ILogger.cs` (ou `ILoggingService.cs`)
- **Interface**: `ILogger<T>` ou `ILoggingService`
- **Linhas Estimadas**: 25-30 linhas
- **Padrão**: Adapter Pattern

#### Métodos a Implementar

```csharp
namespace OrderHub.Application.Ports;

/// <summary>
/// Output Port para logging (opcional)
/// Abstração sobre implementações específicas (Serilog, NLog, etc)
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// Log em nível Information (operações normais)
    /// </summary>
    void LogInformation(string message, params object[] args);
    
    /// <summary>
    /// Log em nível Warning (situações anômalas)
    /// </summary>
    void LogWarning(string message, params object[] args);
    
    /// <summary>
    /// Log em nível Error (erros da aplicação)
    /// </summary>
    void LogError(string message, Exception? exception = null, params object[] args);
    
    /// <summary>
    /// Log em nível Debug (informações detalhadas para desenvolvimento)
    /// </summary>
    void LogDebug(string message, params object[] args);
}
```

#### Características
- ✅ 4 níveis de log: Debug, Information, Warning, Error
- ✅ Suporta exceções e argumentos formatáveis
- ✅ Interface simples e genérica
- ✅ Não depende de bibliotecas específicas
- ✅ Pode ser implementada com Serilog, NLog, Console, etc.

#### Nota
Esta interface é **OPCIONAL**. A aplicação funciona perfeitamente sem ela. Será criada para demonstrar boas práticas de abstração, mas sua implementação em FEAT-06 é opcional.

#### Validações
- ✅ Métodos síncronos (logging é rápido)
- ✅ Sem throws de exceções (logging não deve quebrar aplicação)
- ✅ BuildTime: < 2 segundos

---

## 🏗️ Arquitetura

### Posicionamento das Output Ports

```
APLICAÇÃO ────────────── DEPENDÊNCIAS DE INFRAESTRUTURA
         (camada Application)
         
Input Ports:                      Output Ports:
├─ ICreateOrderUseCase            ├─ IOrderRepository
├─ IGetOrderUseCase               ├─ IUnitOfWork
├─ ...                            ├─ IRepository<T, TId> (optional)
                                  └─ ILoggingService (optional)
```

### Injeção de Dependência Esperada

```csharp
// Em Startup/DI Configuration (FEAT-06):
services.AddScoped<IOrderRepository, OrderRepositoryImpl>();
services.AddScoped<IUnitOfWork, UnitOfWorkImpl>();
services.AddSingleton<ILoggingService, SerilogService>();

// Em Application Services (FEAT-03):
public CreateOrderService(IUnitOfWork unitOfWork, INotificationPort notification)
{
    // Usa IOrderRepository via unitOfWork.Orders
    // Depende apenas da abstração, não da implementação concreta
}
```

---

## ✅ Checklist de Implementação

### TASK-27: IOrderRepository
- [ ] Arquivo criado em `Ports/IOrderRepository.cs`
- [ ] Interface com 4 métodos (GetById, GetByCustomer, Save, Delete)
- [ ] Documentação XML completa
- [ ] Build: 0 erros, 0 warnings
- [ ] Commit atômico

### TASK-28: IUnitOfWork
- [ ] Arquivo criado em `Ports/IUnitOfWork.cs`
- [ ] Interface com propriedade `Orders` e 3 métodos (Begin, Commit, Rollback)
- [ ] Implementa `IAsyncDisposable`
- [ ] Documentação XML completa
- [ ] Build: 0 erros, 0 warnings
- [ ] Commit atômico

### TASK-29: IRepository<T, TId> (Optional)
- [ ] Arquivo criado em `Ports/IRepository.cs`
- [ ] Genérico com type parameters bem definidos
- [ ] 4 métodos genéricos (GetById, GetAll, Add, Remove)
- [ ] Documentação XML completa
- [ ] Build: 0 erros, 0 warnings
- [ ] Commit atômico (ou poderia ser omitido)

### TASK-30: ILoggingService (Optional)
- [ ] Arquivo criado em `Ports/ILoggingService.cs`
- [ ] Interface com 4 métodos de log (Debug, Info, Warning, Error)
- [ ] Suporta exceções e formatação
- [ ] Documentação XML completa
- [ ] Build: 0 erros, 0 warnings
- [ ] Commit atômico (ou poderia ser omitido)

---

## 📈 Progresso

| Task | ID | Descrição | Status | Tempo |
|------|----|---------| -------|-------|
| **TASK-27** | 110 | IOrderRepository | ⏳ To Do | 1.5h |
| **TASK-28** | 111 | IUnitOfWork | ⏳ To Do | 1.5h |
| **TASK-29** | 112 | IRepository<T, TId> | ⏳ To Do | 1.5h |
| **TASK-30** | 113 | ILoggingService | ⏳ To Do | 1h |

---

## 🚀 Próximas Etapas (FEAT-06)

Após FEAT-05, a próxima feature (FEAT-06) implementará as infraestruturas concretas:

- **Infrastructure Implementations**:
  - `OrderRepositoryImpl` (EntityFramework)
  - `UnitOfWorkImpl` (EntityFramework DbContext)
  - `SerilogService` ou `ConsoleLoggerService`
  - Configuração de DI (Dependency Injection)
  
- **Database Setup**:
  - EntityFramework DbContext
  - Migration para criar tabelas
  - Seeding de dados de teste

- **Testes de Integração**:
  - Testes contra banco de dados real
  - Transações e rollback
  - Validação de persistência

---

## 📝 Notas Importantes

1. **Já Existentes**: `IOrderRepository` e `IUnitOfWork` já estão implementados em FEAT-03. TASK-27 e TASK-28 formalizarão o que já existe.

2. **Documentação Prospectiva**: `IRepository<T, TId>` e `ILoggingService` são criadas como exemplos, mas podem ser descartadas na implementação se não agregarem valor.

3. **SOLID Principles**: Todas essas interfaces seguem:
   - **S**ingular Responsibility: cada interface tem um propósito
   - **O**pen/Closed: aberta para extensão (implementações)
   - **L**iskov Substitution: implementações são intercambiáveis
   - **I**nterface Segregation: interfaces focadas
   - **D**ependency Inversion: depender de abstrações

4. **Alinhamento Hexagonal**: Estas interfaces são a "outra ponta" das Input Ports, completando o padrão Hexagonal Architecture.

---

**Pronto para começar FEAT-05!**

Last Updated: 2026-03-12 23:20 UTC  
Status: 📋 Planejado - Aguardando Início
