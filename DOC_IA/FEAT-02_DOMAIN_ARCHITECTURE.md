# 🏗️ Arquitetura da Camada de Domínio - FEAT-02

**Data:** 12 de Março de 2026  
**Feature:** FEAT-02 | Camada de Domínio  

---

## 📊 Arquitetura Geral

```
HEXAGONAL ARCHITECTURE
┌────────────────────────────────────────────────────┐
│                    Controllers                      │  ← Driving Adapters
│                  (REST API, gRPC)                   │
└──────────────────────┬─────────────────────────────┘
                       │ HTTP/REST
┌──────────────────────▼─────────────────────────────┐
│              Application Layer                      │  ← Use Cases
│          (Services, Commands, Queries)              │     & Business Logic
└──────────────────────┬─────────────────────────────┘
                       │ Calls
┌──────────────────────▼─────────────────────────────┐
│            DOMAIN LAYER (DDD)                       │  ← Domain Rules
│        (Entities, Aggregates, Value Objects)       │     & Invariants
│         Ports (Interfaces, Contracts)              │
└──────────────────────┬─────────────────────────────┘
                       │ Uses
┌──────────────────────▼─────────────────────────────┐
│           Infrastructure Layer                      │  ← Driven Adapters
│    (Database, APIs, File System, Events)           │
└────────────────────────────────────────────────────┘
```

---

## 🎯 Bounded Contexts

### Order Context (Contexto Principal)
Responsável por toda a lógica de pedidos

```
OrderAggregate
├── Order (Aggregate Root)
│   ├── OrderId (Value Object)
│   ├── OrderNumber (Value Object)
│   ├── CustomerId (Value Object)
│   ├── OrderStatus (Value Object)
│   ├── Items (Collection<OrderItem>)
│   ├── TotalAmount (Money)
│   └── Timestamps
├── OrderItem (Entity)
│   ├── OrderItemId
│   ├── ProductId
│   ├── Quantity
│   └── UnitPrice (Money)
└── Ports
    ├── IOrderRepository
    └── IOrderEventPublisher
```

### Product Context
Responsável pela gestão de produtos

```
ProductAggregate
├── Product (Aggregate Root)
│   ├── ProductId
│   ├── Sku (ProductSku - Value Object)
│   ├── Name
│   ├── Description
│   ├── Price (Money)
│   └── Stock (Quantity - Value Object)
└── Ports
    └── IProductRepository
```

### Customer Context
Responsável pela gestão de clientes

```
CustomerAggregate
├── Customer (Aggregate Root)
│   ├── CustomerId
│   ├── Name
│   ├── Email (Email - Value Object)
│   ├── PhoneNumber
│   └── Address (Address - Value Object)
└── Ports
    └── ICustomerRepository
```

---

## 🗂️ Estrutura de Pastas

```
src/OrderHub.Domain/
│
├── Aggregates/                          # Agregados
│   │
│   ├── Order/
│   │   ├── Order.cs                     # Aggregate Root
│   │   ├── OrderItem.cs                 # Entity
│   │   ├── OrderStatus.cs               # Value Object (Enum)
│   │   └── OrderId.cs                   # Value Object
│   │
│   ├── Product/
│   │   ├── Product.cs                   # Aggregate Root
│   │   ├── ProductSku.cs                # Value Object
│   │   ├── Stock.cs                     # Value Object
│   │   └── ProductId.cs                 # Value Object
│   │
│   └── Customer/
│       ├── Customer.cs                  # Aggregate Root
│       ├── CustomerId.cs                # Value Object
│       └── (outros value objects)
│
├── ValueObjects/                        # Value Objects Compartilhados
│   ├── Money.cs                         # Valor monetário
│   ├── Email.cs                         # Email com validação
│   ├── PhoneNumber.cs                   # Telefone com validação
│   └── Address.cs                       # Endereço
│
├── Entities/                            # Entities Base
│   ├── Entity.cs                        # Base para todas as entities
│   └── AggregateRoot.cs                 # Base para Aggregate Roots
│
├── Events/                              # Domain Events
│   ├── IDomainEvent.cs                  # Interface base
│   ├── DomainEventHandler.cs            # Handler base
│   │
│   └── Specific/
│       ├── OrderCreatedEvent.cs
│       ├── OrderStatusChangedEvent.cs
│       ├── OrderItemAddedEvent.cs
│       ├── CustomerCreatedEvent.cs
│       └── ProductStockUpdatedEvent.cs
│
├── Ports/                               # Puertos (Interfaces/Contratos)
│   │
│   ├── Repositories/                    # Data Access Contracts
│   │   ├── IRepository.cs               # Interface genérica
│   │   ├── IOrderRepository.cs
│   │   ├── IProductRepository.cs
│   │   ├── ICustomerRepository.cs
│   │   └── IUnitOfWork.cs               # Transações
│   │
│   ├── Services/                        # Application Services Contracts
│   │   ├── INotificationService.cs
│   │   ├── IEmailService.cs
│   │   └── ISmsService.cs
│   │
│   └── Outgoing/                        # Event Publishing Contracts
│       ├── IEventPublisher.cs
│       └── IOrderEventPublisher.cs
│
├── Exceptions/                          # Exceções de Domínio
│   ├── DomainException.cs               # Base
│   ├── OrderNotFoundException.cs
│   ├── InvalidOrderStatusException.cs
│   ├── InsufficientStockException.cs
│   └── InvalidEmailException.cs
│
├── Specifications/                      # DDD Specifications (Critério)
│   └── OrderSpecification.cs
│
└── Constants/                           # Constantes de Domínio
    ├── OrderConstants.cs
    ├── ProductConstants.cs
    └── ValidationConstants.cs
```

---

## 🏛️ Diagrama de Classes - Order Aggregate

```
┌──────────────────────────────────────────────┐
│              AggregateRoot                   │
│  (Base abstrata para raízes de agregados)    │
├──────────────────────────────────────────────┤
│ + Id: Guid                                   │
│ + Version: int                               │
│ + GetUncommittedEvents(): List<DomainEvent> │
│ + ClearUncommittedEvents(): void            │
└──────────────────────┬───────────────────────┘
                       △
                       │ Herda
                       │
┌──────────────────────▼───────────────────────┐
│              <<Aggregate Root>>              │
│                    Order                     │
├──────────────────────────────────────────────┤
│ - id: OrderId (Value Object)                │
│ - orderNumber: string                       │
│ - customerId: CustomerId (Value Object)     │
│ - status: OrderStatus (Value Object)        │
│ - items: List<OrderItem>                    │
│ - totalAmount: Money (Value Object)         │
│ - createdAt: DateTime                       │
│ - updatedAt: DateTime?                      │
├──────────────────────────────────────────────┤
│ + CreateOrder(customerId, items): Order     │
│ + AddItem(product, quantity): void          │
│ + RemoveItem(itemId): void                  │
│ + ChangeStatus(newStatus): void             │
│ + CalculateTotalAmount(): Money             │
│ + Validate(): Result                        │
├──────────────────────────────────────────────┤
│ Events:                                      │
│ - OrderCreatedEvent                         │
│ - OrderStatusChangedEvent                   │
│ - OrderItemAddedEvent                       │
│ - OrderItemRemovedEvent                     │
└──────────────────────────────────────────────┘
         △              △              △
         │ Contains     │ Contains      │ References
         │              │              │
    ┌────┴──┐    ┌─────┴────┐    ┌────┴──────────┐
    │        │    │          │    │               │
┌───▼──┐  ┌─┴────▼──┐  ┌────▼─────┤ ┌──────────┐ │
│Order │  │OrderItem│  │OrderStatus│ │CustomerId│ │
│Item  │  │   (E)   │  │    (VO)    │ │   (VO)   │ │
│ (E)  │  │         │  │ - Pending  │ └──────────┘ │
│      │  │ - Id    │  │ - Confirmed│              │
│ - Id │  │ - Qty   │  │ - Shipped  │   ┌────────┐ │
│ - Qty│  │ - Price │  │ - Delivered│   │ Money  │ │
│      │  │  (Money)│  │ - Cancelled│   │  (VO)  │ │
└──────┘  │ - Amount│  └────────────┘   └────────┘ │
          │  (Money)│
          └─────────┘
```

---

## 💾 Padrão Repository

```csharp
// Porta (Interface) - No Domain
public interface IOrderRepository
{
    Task<Order> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Order?> GetByNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

// Adaptador (Implementação) - Na Infraestrutura
public class OrderRepository : IOrderRepository
{
    private readonly OrderHubDbContext _context;
    
    public async Task<Order> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Implementação usando Entity Framework
        return await _context.Orders.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
    }
    
    // ... outras implementações
}
```

---

## 🎭 Exemplo de Aggregate Root

```csharp
public class Order : AggregateRoot
{
    // Identidade
    public OrderId Id { get; }
    public string OrderNumber { get; private set; }
    
    // Referências
    public CustomerId CustomerId { get; private set; }
    
    // Status e Dados
    public OrderStatus Status { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();
    public Money TotalAmount { get; private set; }
    
    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Invariantes (Regras de Negócio)
    private const int MinimumItems = 1;
    private const int MaximumItems = 100;
    
    // Construtor privado (Factory)
    private Order() { }
    
    // Factory Method
    public static Result<Order> Create(CustomerId customerId, List<OrderItem> items)
    {
        // Validações
        if (items == null || items.Count < MinimumItems)
            return Result.Failure<Order>("Ordem deve ter pelo menos um item");
        
        if (items.Count > MaximumItems)
            return Result.Failure<Order>("Ordem não pode ter mais de 100 itens");
        
        // Criação
        var order = new Order
        {
            Id = OrderId.Create(Guid.NewGuid()),
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId,
            Status = OrderStatus.Pending,
            Items = items,
            CreatedAt = DateTime.UtcNow,
        };
        
        // Domain Event
        order.RaiseDomainEvent(new OrderCreatedEvent(
            order.Id, 
            order.CustomerId, 
            order.CreatedAt
        ));
        
        return Result.Success(order);
    }
    
    // Métodos de Negócio
    public Result AddItem(OrderItem item)
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure("Não é possível adicionar itens a uma ordem não-pendente");
        
        if (Items.Count >= MaximumItems)
            return Result.Failure("Ordem atingiu limite de itens");
        
        Items.Add(item);
        
        // Atualizar total
        RecalculateTotalAmount();
        
        // Domain Event
        RaiseDomainEvent(new OrderItemAddedEvent(Id, item.Id, item.UnitPrice));
        
        return Result.Success();
    }
    
    public Result ChangeStatus(OrderStatus newStatus)
    {
        // Validar transição de estado
        if (!IsValidStatusTransition(Status, newStatus))
            return Result.Failure($"Transição inválida de {Status} para {newStatus}");
        
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        
        // Domain Event
        RaiseDomainEvent(new OrderStatusChangedEvent(Id, Status, DateTime.UtcNow));
        
        return Result.Success();
    }
    
    private void RecalculateTotalAmount()
    {
        var total = Items
            .Select(i => i.UnitPrice * i.Quantity)
            .Aggregate(Money.Zero(), (acc, amt) => acc + amt);
        
        TotalAmount = total;
    }
    
    private static string GenerateOrderNumber()
        => $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    
    private static bool IsValidStatusTransition(OrderStatus current, OrderStatus next)
        => (current, next) switch
        {
            (OrderStatus.Pending, OrderStatus.Confirmed) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Confirmed, OrderStatus.Shipped) => true,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
            (OrderStatus.Shipped, OrderStatus.Delivered) => true,
            _ => false
        };
}
```

---

## 💡 Value Objects - Exemplos

### Money (Valor Monetário)

```csharp
public class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; } = "BRL";
    
    private Money() { }
    
    public Money(decimal amount)
    {
        if (amount < 0)
            throw new InvalidOperationException("Valor não pode ser negativo");
        
        Amount = amount;
    }
    
    public static Money Zero() => new(0);
    
    public static Money operator +(Money a, Money b) => new(a.Amount + b.Amount);
    public static Money operator -(Money a, Money b) => new(a.Amount - b.Amount);
    public static Money operator *(Money money, int quantity) => new(money.Amount * quantity);
    
    public override bool Equals(object obj) => obj is Money money && Money.Amount == money.Amount;
    public override int GetHashCode() => Amount.GetHashCode();
}
```

### Email (Email com Validação)

```csharp
public class Email : ValueObject
{
    public string Value { get; }
    
    private Email() { }
    
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !IsValidEmail(value))
            throw new InvalidOperationException("Email inválido");
        
        Value = value.ToLower();
    }
    
    private static bool IsValidEmail(string email)
        => Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    
    public override bool Equals(object obj) => obj is Email email && Value == email.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
```

---

## 🔄 Domain Events

```csharp
// Evento Base
public abstract class DomainEvent
{
    public Guid AggregateId { get; protected set; }
    public DateTime OccurredAt { get; protected set; } = DateTime.UtcNow;
    public int Version { get; protected set; } = 1;
}

// Evento Específico
public class OrderCreatedEvent : DomainEvent
{
    public OrderId OrderId { get; }
    public CustomerId CustomerId { get; }
    public DateTime CreatedAt { get; }
    
    public OrderCreatedEvent(OrderId orderId, CustomerId customerId, DateTime createdAt)
    {
        OrderId = orderId;
        CustomerId = customerId;
        CreatedAt = createdAt;
        AggregateId = orderId.Value;
    }
}
```

---

## 📋 Fluxo de Desenvolvimento

```
1. Criar estrutura de pastas
   └─ mkdir -p OrderHub.Domain/...

2. Implementar Value Objects
   ├─ Money
   ├─ Email
   ├─ PhoneNumber
   └─ Address

3. Implementar Entities
   ├─ Order
   ├─ OrderItem
   ├─ Customer
   └─ Product

4. Implementar Aggregates Roots
   ├─ Order (com Order Items)
   ├─ Customer
   └─ Product

5. Definir Ports (Interfaces)
   ├─ IOrderRepository
   ├─ ICustomerRepository
   ├─ IProductRepository
   └─ IEventPublisher

6. Implementar Domain Events
   ├─ OrderCreatedEvent
   ├─ OrderStatusChangedEvent
   └─ Outros...

7. Adicionar Testes Unitários
   ├─ OrderAggregateTests
   ├─ MoneyTests
   ├─ EmailTests
   └─ Outros...

8. Code Review & Refactoring
   └─ Qualidade de Código
```

---

## ✅ Critérios de Aceitação da Feature

- [ ] Projeto OrderHub.Domain criado com sucesso
- [ ] Todos os Aggregates implementados
- [ ] Todas as Value Objects criadas
- [ ] Domain Events implementados
- [ ] Ports (Interfaces) definidas
- [ ] 80%+ test coverage
- [ ] Sem dependências não-desejadas
- [ ] Documentação completa
- [ ] Code Review aprovado
- [ ] Mergeado em develop
- [ ] Build pipeline passando

---

**Documento criado em:** 12 de Março de 2026  
**Status:** 📝 Planejamento para Implementação
