# FEAT-02 | Arquitetura Domain Layer

**Data**: 12 de Março de 2026  
**Feature**: FEAT-02 | Domain Layer  
**Documentação de**: Arquitetura de Domínio  
**Baseado em**: Azure DevOps Issue 68 (6 tasks reais)  

---

## 📐 Arquitetura Geral da Solução

```
┌─────────────────────────────────────────────────────────────┐
│                    ORDENAÇÃO HEXAGONAL                      │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌──────────────────────────────────────────────────────┐   │
│  │              PRESENTATION LAYER (API)                │   │
│  │         HTTP Controllers / REST Endpoints            │   │
│  └──────────────────────────────────────────────────────┘   │
│                           ▲                                   │
│                           │ Adapters                          │
│                           ▼                                   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │          APPLICATION LAYER (Use Cases)               │   │
│  │     Services / CQRS / Command Handlers              │   │
│  └──────────────────────────────────────────────────────┘   │
│                           ▲                                   │
│                           │ Uses                              │
│                           ▼                                   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │          🎯 DOMAIN LAYER (FEAT-02) 🎯                │   │
│  │  ├─ Aggregates (Order, OrderItem)                    │   │
│  │  ├─ Value Objects (OrderAmount, OrderStatus, etc)   │   │
│  │  ├─ Entities (OrderItem)                             │   │
│  │  ├─ Ports (IOrderRepository, etc)                   │   │
│  │  ├─ Exceptions (DomainException)                     │   │
│  │  ├─ Domain Events                                    │   │
│  │  └─ Business Rules (Encapsulated)                    │   │
│  └──────────────────────────────────────────────────────┘   │
│                           ▲                                   │
│                           │ Implementations                   │
│                           ▼                                   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │       INFRASTRUCTURE LAYER (Database, APIs)          │   │
│  │     Repositories / External Services / DB Context    │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

---

## 🎯 Estrutura da Domain Layer / FEAT-02

```
src/OrderHub.Domain/
├── Aggregates/
│   └── Order/
│       ├── Order.cs                 # Aggregate Root
│       ├── OrderItem.cs             # Entity dentro do agregado
│       └── OrderStatus.cs           # Value Object - Estados
│
├── ValueObjects/
│   ├── OrderAmount.cs               # Dinheiro representando totais
│   ├── OrderId.cs                   # Identidade do pedido
│   ├── CustomerId.cs                # Referência a customer
│   └── Money.cs                     # Genérico para monetário
│
├── Entities/
│   └── (Entidades sem identidade própria)
│
├── Exceptions/
│   ├── DomainException.cs           # Exceção base do domínio
│   ├── InvalidOrderException.cs     # Ordem inválida
│   └── InvalidOrderAmountException.cs # Valor inválido
│
├── Ports/
│   ├── IOrderRepository.cs          # Contrato para persistência
│   └── IOrderNotificationPort.cs    # Contrato para notificações
│
├── Events/
│   ├── IDomainEvent.cs              # Interface base
│   ├── OrderCreatedEvent.cs
│   ├── OrderItemAddedEvent.cs
│   └── OrderStatusChangedEvent.cs
│
└── Constants/
    ├── OrderConstants.cs
    └── ValidationMessages.cs
```

---

## 📋 Padrões DDD Utilizados

### 1. **Aggregate Root: Order**

O agregado `Order` encapsula toda lógica relacionada a pedidos. Só pode ser acessado através da raiz.

**Responsabilidades**:
- Manter invariantes de negócio
- Validar regras antes de estado mudar
- Disparar domain events quando necessário
- Expor apenas métodos com intenção clara

```csharp
public class Order : AggregateRoot
{
    public OrderId OrderId { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public List<OrderItem> Items { get; private set; } = new();
    
    // Private construtor - usar factory method
    private Order(OrderId orderId, CustomerId customerId)
    {
        OrderId = orderId;
        CustomerId = customerId;
        OrderDate = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }
    
    // Factory method - Intenção clara
    public static Order CreateOrder(OrderId orderId, CustomerId customerId)
    {
        DomainValidator.ThrowIfNull(orderId, "OrderId não pode ser nulo");
        DomainValidator.ThrowIfNull(customerId, "CustomerId não pode ser nulo");
        
        return new Order(orderId, customerId);
    }
    
    // Método com regra embutida
    public void AddItem(OrderItem item)
    {
        if (Status == OrderStatus.Shipped)
            throw new InvalidOrderException("Não é possível adicionar itens a um pedido enviado");
        
        if (Items.Count >= 10)
            throw new InvalidOrderException("Número máximo de 10 itens atingido");
        
        Items.Add(item);
    }
}
```

### 2. **Value Objects: OrderAmount**

Representa valores monetários com total imutabilidade.

**Características**:
- Sem identidade própria (comparado por valor)
- Imutável (readonly em tudo)
- Auto-validação na construção
- Comportamento específico domínio

```csharp
public class OrderAmount : IEquatable<OrderAmount>
{
    public decimal Value { get; }
    public string Currency { get; } = "BRL";
    
    private OrderAmount(decimal value, string currency = "BRL")
    {
        Value = value;
        Currency = currency;
    }
    
    public static OrderAmount Create(decimal value, string currency = "BRL")
    {
        if (value <= 0)
            throw new InvalidOrderAmountException("Valor deve ser maior que zero");
        
        return new OrderAmount(value, currency);
    }
    
    public bool Equals(OrderAmount other)
    {
        return other != null && 
               Value == other.Value && 
               Currency == other.Currency;
    }
    
    public override string ToString() => $"{Currency} {Value:N2}";
}
```

### 3. **Entidade: OrderItem**

Entidade dentro do agregado Order. Tem identidade apenas no contexto do aggregado.

```csharp
public class OrderItem
{
    public OrderItemId OrderItemId { get; private set; }
    public ProductId ProductId { get; private set; }
    public OrderAmount UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    
    private OrderItem() { }
    
    public OrderItem(OrderItemId id, ProductId productId, OrderAmount unitPrice, int quantity)
    {
        DomainValidator.ThrowIfNull(productId, "ProductId não pode ser nulo");
        DomainValidator.ThrowIfNegativeOrZero(quantity, "Quantidade deve ser > 0");
        
        OrderItemId = id;
        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
    
    public OrderAmount GetTotal() => 
        OrderAmount.Create(UnitPrice.Value * Quantity);
}
```

### 4. **Exceções de Domínio**

Exceções específicas do negócio, não técnicas.

```csharp
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class InvalidOrderException : DomainException
{
    public InvalidOrderException(string message) : base(message) { }
}

public class InvalidOrderAmountException : DomainException
{
    public InvalidOrderAmountException(string message) : base(message) { }
}
```

### 5. **Validador de Domínio**

Centraliza validações comuns.

```csharp
public static class DomainValidator
{
    public static void ThrowIfNull(object value, string message)
    {
        if (value == null)
            throw new DomainException(message);
    }
    
    public static void ThrowIfNegativeOrZero(decimal value, string message)
    {
        if (value <= 0)
            throw new DomainException(message);
    }
    
    public static void ThrowIfEmpty(string value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(message);
    }
}
```

---

## 🎭 Regras de Negócio Codificadas

As seguintes **regras de negócio** serão enforced no código e **impossíveis de violar**:

### R1: Não adicionar itens a pedido enviado
```csharp
public void AddItem(OrderItem item)
{
    if (Status == OrderStatus.Shipped)
        throw new InvalidOrderException(
            "Não é possível adicionar itens a um pedido enviado");
}
```

### R2: Mínimo 1 item no pedido
```csharp
public static Order CreateOrder(OrderId orderId, CustomerId customerId)
{
    // Validação na factory
    // Items.Count >= 1 garantido após criação
}
```

### R3: Não remover único item
```csharp
public void RemoveItem(OrderItem item)
{
    if (Items.Count == 1)
        throw new InvalidOrderException(
            "Não é possível remover o último item do pedido");
}
```

### R4: Remover único item → Cancela pedido
```csharp
public void RemoveItem(OrderItem item)
{
    var itemToRemove = Items.FirstOrDefault(x => x.OrderItemId == item.OrderItemId);
    
    if (Items.Count == 1 && itemToRemove != null)
    {
        Items.Remove(itemToRemove);
        Status = OrderStatus.Cancelled;
        RaiseDomainEvent(new OrderCancelledEvent(OrderId));
    }
    else if (itemToRemove != null)
    {
        Items.Remove(itemToRemove);
    }
}
```

### R5: Máximo 10 itens distintos
```csharp
public void AddItem(OrderItem item)
{
    if (Items.Count >= 10)
        throw new InvalidOrderException(
            "Número máximo de 10 itens atingido");
}
```

### R6: Transições válidas de Status
```csharp
public bool CanTransitionTo(OrderStatus newStatus)
{
    // Nueva → Pending → Processing → Shipped → Delivered
    // Cancelled pode vir de qualquer estado
    return newStatus switch
    {
        OrderStatus.Pending => Status == OrderStatus.Pending,
        OrderStatus.Processing => Status == OrderStatus.Pending,
        OrderStatus.Shipped => Status == OrderStatus.Processing,
        OrderStatus.Delivered => Status == OrderStatus.Shipped,
        OrderStatus.Cancelled => true,
        _ => false
    };
}

public void ChangeStatus(OrderStatus newStatus)
{
    if (!CanTransitionTo(newStatus))
        throw new InvalidOrderException(
            $"Transição de {Status} para {newStatus} não permitida");
    
    Status = newStatus;
    RaiseDomainEvent(new OrderStatusChangedEvent(OrderId, newStatus));
}
```

---

## 🧪 Estratégia de Testes

### Pirâmide de Testes
```
        ▲
       / \
      /   \ - E2E (API/Integração)  ~5%
     /-----\
    /       \ - Integration Tests    ~15%
   /         \
  /-----------\ - Unit Tests (Domínio) ~80%
 /             \
```

### Testes Unitários do Domínio
- **Sem dependências externas** (sem banco, APIs, etc)
- **Rápidos** (nanosegundos a milisegundos)
- **Determinísticos** (sempre mesmo resultado)
- **Fáceis de entender** (nome descreve cenário)

Exemplo:
```csharp
[Fact]
public void AddItem_ToShippedOrder_ThrowsException()
{
    // Arrange
    var order = Order.CreateOrder(new OrderId(Guid.NewGuid()), new CustomerId(Guid.NewGuid()));
    order.ChangeStatus(OrderStatus.Shipped);
    var item = new OrderItem(...);
    
    // Act & Assert
    Assert.Throws<InvalidOrderException>(() => order.AddItem(item));
}
```

---

## 📦 Dependências Externas

**Domain Layer NÃO deve ter**:
- ❌ Referências a `Infrastructure`
- ❌ Referências a `Application`
- ❌ Referências a `Presentation`
- ❌ Dependências externas (NuGet packages)

**Pode ter**:
- ✅ `System.*` (core .NET)

**Por quê?**:
- Domínio é core, não deve depender de camadas externas
- Fácil testar sem mocks complexos
- Reusável em diferentes contextos

---

## 🔌 Ports (Interfaces)

Contracts que serão implementados por Infrastructure.

```csharp
// Namespace: OrderHub.Domain.Ports

public interface IOrderRepository
{
    Task<Order> GetByIdAsync(OrderId id, CancellationToken cancellationToken);
    Task SaveAsync(Order order, CancellationToken cancellationToken);
    Task DeleteAsync(OrderId id, CancellationToken cancellationToken);
}

public interface IOrderNotificationPort
{
    Task NotifyOrderStatusChangedAsync(Order order, OrderStatus newStatus);
    Task NotifyOrderCreatedAsync(Order order);
}
```

---

## 📡 Domain Events

Eventos disparados quando eventos importantes ocorrem.

```csharp
public interface IDomainEvent
{
    OrderId OrderId { get; }
    DateTime OccurredAt { get; }
}

public class OrderCreatedEvent : IDomainEvent
{
    public OrderId OrderId { get; }
    public CustomerId CustomerId { get; }
    public DateTime OccurredAt { get; }
    
    public OrderCreatedEvent(OrderId orderId, CustomerId customerId)
    {
        OrderId = orderId;
        CustomerId = customerId;
        OccurredAt = DateTime.UtcNow;
    }
}
```

---

## 🏗️ Ao Completar FEAT-02

Após completar todas as 6 tasks, a Domain Layer será:

✅ **Completa**: Todos agregados, VOs e ports criados  
✅ **Testada**: 80%+ cobertura de testes  
✅ **Documentada**: Código auto-explicativo e comentado  
✅ **Pronta para Integração**: Application Layer pode usar  
✅ **Sem Dependências Externas**: Isolada do resto  

---

## 🚀 Próximo Valor

Uma vez FEAT-02 completado, FEAT-03 (Application Layer) pode:
- Usar agregados e value objects
- Implementar use cases
- Ler eventos de domínio
- Coordenar com Infrastructure

