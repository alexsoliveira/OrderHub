# 📋 Plano de Ação - FEAT-02 Domínio

**Status:** 📝 Planejamento  
**Data:** 12 de Março de 2026  
**Feature:** FEAT-02 | Implementar Camada de Domínio (Domain Layer) e Ports  

---

## 🎯 Objetivo da Feature

Implementar a **camada de domínio** seguindo princípios de **Domain Driven Design (DDD)** e **Arquitetura Hexagonal**, criando as entidades, value objects, agregados e portas (interfaces) necessárias para o sistema de gerenciamento de pedidos.

---

## 📊 Escopo e Tarefas

### TASK-09: Criar Projeto OrderHub.Domain
**Objetivo:** Adicionar projeto Class Library para camada de domínio  
**Descrição:**
- Criar novo projeto Class Library: `src/OrderHub.Domain/`
- Configurar arquivos do projeto (.csproj)
- Estrutura de pastas inicial
- Referências necessárias

**Arquivos a Criar:**
```
src/OrderHub.Domain/
├── OrderHub.Domain.csproj
├── ...
```

**Comandos:**
```bash
cd src
dotnet new classlib -n OrderHub.Domain
cd OrderHub.Domain
# adicionar .csproj ao sln
cd ../..
dotnet sln add src/OrderHub.Domain/OrderHub.Domain.csproj
```

**Acceptance Criteria:**
- ✅ Projeto criado e compila sem erros
- ✅ Referenciado na solução OrderHub.sln
- ✅ Estrutura padrão de pastas criada
- ✅ Arquivo de teste incluído

**Estimativa:** 1 ponto  
**Prioridade:** Alta

---

### TASK-10: Implementar Order Aggregate Root
**Objetivo:** Criar a entidade raiz de agregação para Pedidos  
**Descrição:**
- Criar classe `Order` como Aggregate Root
- Implementar propriedades essenciais (Id, OrderNumber, CreatedAt, Status, Items, TotalAmount)
- Implementar value objects para status e valores monetários
- Validações de negócio na criação

**Arquivos a Criar:**
```
src/OrderHub.Domain/
├── Aggregates/
│   └── Order/
│       ├── Order.cs              # Aggregate Root
│       ├── OrderStatus.cs         # Value Object (Enum)
│       ├── OrderItem.cs           # Entity (parte do agregado)
│       └── Money.cs              # Value Object
├── Models/
│   └── ...
```

**Métodos da Classe Order:**
- `CreateOrder(customerId, items)` - Factory method
- `AddItem(product, quantity, price)`
- `RemoveItem(orderItemId)`
- `ChangeStatus(newStatus)`
- `CalculateTotalAmount()`

**Acceptance Criteria:**
- ✅ Classe Order criada e funcional
- ✅ Value Objects criados (Money, OrderStatus)
- ✅ Validações implementadas
- ✅ Métodos de negócio funcionam corretamente
- ✅ Compila sem warnings

**Estimativa:** 3 pontos  
**Prioridade:** Alta

---

### TASK-11: Implementar Order Item Entity
**Objetivo:** Criar a entidade que representa itens dentro do pedido  
**Descrição:**
- Criar classe `OrderItem` como Entity
- Referência para Product
- Quantidade e Preço unitário
- Cálculo de subtotal

**Arquivos a Criar:**
```
src/OrderHub.Domain/
├── Aggregates/Order/
│   └── OrderItem.cs
├── ValueObjects/
│   └── Money.cs
```

**Propriedades:**
- `Id: Guid`
- `ProductId: Guid`
- `Quantity: int`
- `UnitPrice: Money`
- `Subtotal: Money { get; }`

**Acceptance Criteria:**
- ✅ Entity criada com validações
- ✅ Subtotal calculado automaticamente
- ✅ Imutável onde apropriado
- ✅ Integra com Order Aggregate

**Estimativa:** 2 pontos  
**Prioridade:** Alta

---

### TASK-12: Implementar Customer Entity
**Objetivo:** Criar a entidade que representa clientes  
**Descrição:**
- Criar classe `Customer`
- Propriedades: Id, Name, Email, Phone, Address
- Value Objects: Email, PhoneNumber, Address
- Validações de email e telefone

**Arquivos a Criar:**
```
src/OrderHub.Domain/
├── Entities/
│   └── Customer.cs
├── ValueObjects/
│   ├── Email.cs
│   ├── PhoneNumber.cs
│   └── Address.cs
```

**Acceptance Criteria:**
- ✅ Entity criada com validações
- ✅ Value Objects implementados
- ✅ Email normalizado e validado
- ✅ Métodos de atualização de dados

**Estimativa:** 2 pontos  
**Prioridade:** Média

---

### TASK-13: Implementar Product Value Object
**Objetivo:** Criar value object para produtos  
**Descrição:**
- Criar classe `Product` (pode ser um aggregate ou value object)
- SKU, Name, Description, Price, StockQuantity
- Validações de preço e estoque

**Arquivos a Criar:**
```
src/OrderHub.Domain/
├── Aggregates/
│   └── Product/
│       ├── Product.cs         # Aggregate Root
│       ├── ProductSku.cs      # Value Object
│       └── Stock.cs           # Value Object
```

**Propriedades:**
- `Id: Guid`
- `Sku: ProductSku`
- `Name: string`
- `Description: string`
- `Price: Money`
- `StockQuantity: int`

**Acceptance Criteria:**
- ✅ Aggregate criado
- ✅ Value objects para SKU e preço
- ✅ Validações de negócio
- ✅ Métodos para atualizar estoque

**Estimativa:** 2 pontos  
**Prioridade:** Média

---

### TASK-14: Implementar Domain Ports (Interfaces)
**Objetivo:** Definir contratos para adaptadores da camada de infraestrutura  
**Descrição:**
- Criar interfaces de repositórios (Ports)
- IOrderRepository
- ICustomerRepository
- IProductRepository
- IUnitOfWork (para transações)
- INotificationService (para enviar notificações)

**Arquivos a Criar:**
```
src/OrderHub.Domain/
├── Ports/
│   ├── Repositories/
│   │   ├── IOrderRepository.cs
│   │   ├── ICustomerRepository.cs
│   │   └── IProductRepository.cs
│   ├── Services/
│   │   ├── INotificationService.cs
│   │   └── IUnitOfWork.cs
│   └── Outgoing/
│       └── IOrderEventPublisher.cs
```

**Métodos das Interfaces:**
```csharp
// IOrderRepository
Task<Order> GetByIdAsync(Guid id);
Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId);
Task AddAsync(Order order);
Task UpdateAsync(Order order);
Task DeleteAsync(Guid id);

// ICustomerRepository
Task<Customer> GetByIdAsync(Guid id);
Task<Customer?> GetByEmailAsync(Email email);
Task AddAsync(Customer customer);

// IProductRepository
Task<Product> GetByIdAsync(Guid id);
Task<Product?> GetBySkuAsync(ProductSku sku);
```

**Acceptance Criteria:**
- ✅ Todas as interfaces criadas
- ✅ Métodos bem definidos
- ✅ Sem dependências de infraestrutura
- ✅ Documentadas com XML comments

**Estimativa:** 2 pontos  
**Prioridade:** Alta

---

### TASK-15: Implementar Domain Events
**Objetivo:** Criar event sourcing para rastreabilidade de eventos de domínio  
**Descrição:**
- Classe base `DomainEvent`
- Eventos específicos:
  - `OrderCreatedEvent`
  - `OrderItemAddedEvent`
  - `OrderStatusChangedEvent`
  - `CustomerCreatedEvent`
  - `ProductStockUpdatedEvent`

**Arquivos a Criar:**
```
src/OrderHub.Domain/
├── Events/
│   ├── DomainEvent.cs          # Base abstrata
│   ├── DomainEventPublisher.cs
│   └── Specific/
│       ├── OrderCreatedEvent.cs
│       ├── OrderStatusChangedEvent.cs
│       ├── OrderItemAddedEvent.cs
│       ├── CustomerCreatedEvent.cs
│       └── ProductStockUpdatedEvent.cs
```

**Propriedades Base:**
```csharp
public abstract class DomainEvent
{
    public Guid AggregateId { get; }
    public DateTime OccurredAt { get; }
    public int Version { get; }
}
```

**Acceptance Criteria:**
- ✅ Base class DomainEvent criada
- ✅ Todos os eventos implementados
- ✅ Agregados rastreiam eventos
- ✅ Publisher permite subscrição

**Estimativa:** 2 pontos  
**Prioridade:** Média

---

### TASK-16: Adicionar Testes Unitários do Domínio
**Objetivo:** Cobrir 80%+ do código de domínio com testes  
**Descrição:**
- Criar projeto de testes: `tests/OrderHub.Domain.Tests/`
- Testes para agregados: Order, Product, Customer
- Testes para value objects: Money, Email, Address
- Testes para validações

**Arquivos a Criar:**
```
tests/OrderHub.Domain.Tests/
├── OrderHub.Domain.Tests.csproj
├── Aggregates/
│   ├── OrderAggregateTests.cs
│   └── ProductAggregateTests.cs
├── Entities/
│   └── CustomerEntityTests.cs
├── ValueObjects/
│   ├── MoneyTests.cs
│   ├── EmailTests.cs
│   └── AddressTests.cs
└── Events/
    └── DomainEventTests.cs
```

**Casos de Teste:**
```csharp
// OrderAggregateTests
[Theory]
public void CreateOrder_WithValidData_ShouldSucceed()
public void CreateOrder_WithoutItems_ShouldFail()
public void AddItem_ToOrder_ShouldUpdateTotal()
public void ChangeStatus_ToInvalidStatus_ShouldFail()

// MoneyTests
[Theory]
public void Money_Add_ShouldCalculateCorrectly()
public void Money_InvalidAmount_ShouldThrow()

// EmailTests
[Theory]
public void Email_InvalidFormat_ShouldThrow()
public void Email_Equality_ShouldWork()
```

**Acceptance Criteria:**
- ✅ 80%+ code coverage
- ✅ Testes passam
- ✅ Testes bem nomeados
- ✅ Usa xUnit e Moq
- ✅ 50+ testes implementados

**Estimativa:** 3 pontos  
**Prioridade:** Alta

---

## 📅 Cronograma Estimado

| TASK | Título | Pontos | Dias | Período |
|------|--------|--------|------|---------|
| 09 | Criar Projeto OrderHub.Domain | 1 | 1h | Dia 1 |
| 10 | Implementar Order Aggregate Root | 3 | 1-2h | Dia 1-2 |
| 11 | Implementar Order Item Entity | 2 | 1h | Dia 2 |
| 12 | Implementar Customer Entity | 2 | 1-2h | Dia 2-3 |
| 13 | Implementar Product Aggregate | 2 | 1-2h | Dia 3 |
| 14 | Implementar Domain Ports | 2 | 2h | Dia 3-4 |
| 15 | Implementar Domain Events | 2 | 1-2h | Dia 4 |
| 16 | Adicionar Testes Unitários | 3 | 2-3h | Dia 4-5 |
| **TOTAL** | | **17 pontos** | **10-14h** | **~1 semana** |

---

## 🛠️ Tecnologias e Padrões

### Padrões de Design
- ✅ Aggregate Root (DDD)
- ✅ Entity (DDD)
- ✅ Value Object (DDD)
- ✅ Domain Events
- ✅ Factory Methods
- ✅ Repository Pattern
- ✅ Unit of Work Pattern

### Validações
- ✅ Fluent Validation (opcional, pode adicionar depois)
- ✅ Data Annotations
- ✅ Custom Validations

### Testes
- ✅ xUnit
- ✅ Moq
- ✅ FluentAssertions

---

## 📝 Checklist de Qualidade

- [ ] Sem dependências externas no Domain
- [ ] 80%+ test coverage
- [ ] XML documentation completa
- [ ] Sem static methods (exceto factories)
- [ ] Sem null references
- [ ] Validações em construtores
- [ ] Value Objects imutáveis
- [ ] Métodos bem nomeados (Ubiquitous Language)

---

## 🔗 Dependências

- **FEAT-01:** ✅ Concluída (Setup inicial)
- **Nenhuma outra feature** precisa estar pronta antes

---

## 📚 Referências

- Domain Driven Design - Eric Evans
- Implementing Domain-Driven Design - Vaughn Vernon
- Hexagonal Architecture - Alistair Cockburn

---

## 👤 Responsável

- **Product Owner:** Alex Oliveira
- **Tech Lead:** Alex Oliveira
- **Desenvolvedor:** [A assinalar]

---

**Documento criado em:** 12 de Março de 2026  
**Próxima revisão:** Após implementação da FEAT-02

