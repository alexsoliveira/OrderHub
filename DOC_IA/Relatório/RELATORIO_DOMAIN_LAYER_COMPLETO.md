# 📊 Relatório Completo - Camada Domain do OrderHub

**Data**: Março 2026  
**Projeto**: OrderHub - Hexagonal Architecture com DDD  
**Framework**: .NET 8 (net10.0)  
**Local**: `src/OrderHub.Domain/`

---

## 📋 Sumário Executivo

A camada **Domain** do OrderHub é puramente responsável pela lógica de negócio, sem qualquer dependência externa. Implementa **Hexagonal Architecture** com **Domain-Driven Design (DDD)**, oferecendo:

✅ **Zero dependências externas** - Apenas SDK .NET nativo  
✅ **5 Agregados** - Order como raiz, OrderItem como entidade  
✅ **5 Value Objects** - OrderId, CustomerId, ProductId, OrderStatus, OrderAmount  
✅ **6 Output Ports** - Interfaces para integração com adapters  
✅ **3 Exceções de Domínio** - Hierarquia bem definida  
✅ **11 Métodos de Validação** - DomainValidator centralizado  
❌ **0 Domain Events** - Pasta vazia (pronto para implementação futura)

---

## 🏗️ 1. ESTRUTURA DE AGREGADOS

### 1.1 Agregado Raiz: `Order`

**Localização**: [`src/OrderHub.Domain/Aggregates/Order/Order.cs`](src/OrderHub.Domain/Aggregates/Order/Order.cs)

**Tipo**: Aggregate Root (herda de `AggregateRoot`)

**Responsabilidade**: Encapsula toda a lógica e regras de negócio relacionadas a pedidos

#### Propriedades Principais

| Propriedade | Tipo | Modificador | Descrição |
|-------------|------|------------|-----------|
| `OrderId` | `OrderId` | private set | Identificador único do pedido (Value Object) |
| `CustomerId` | `CustomerId` | private set | Identificador do cliente (Value Object) |
| `OrderDate` | `DateTime` | private set | Data de criação do pedido |
| `Status` | `OrderStatus` | private set | Estado atual do pedido (New, Pending, Processing, Shipped, Delivered, Cancelled) |
| `Items` | `IReadOnlyList<OrderItem>` | get-only | Coleção imutável de itens do pedido |
| `_items` | `List<OrderItem>` | private | Coleção interna para gerenciamento |

#### Constantes de Regras de Negócio

```csharp
private const int MaximumDistinctItems = 10;  // Máximo de itens distintos
private const int MinimumItems = 1;           // Mínimo de itens obrigatório
```

#### Métodos Públicos

```csharp
// Factory method para criação
public static Order CreateOrder(OrderId orderId, CustomerId customerId, DateTime? orderDate = null)
{
    // Valida orderId e customerId
    // Retorna nova instância com Status = New
}

// Gerenciamento de itens
public void AddItem(OrderItem item)
{
    // REGRA 1: Não permite adicionar itens a pedido Shipped
    // REGRA 5: Não permite mais de 10 itens distintos
    // Substitui item existente se já houver
}

public void RemoveItem(OrderItem item)
{
    // REGRA 3: Não permite remover último item
    // REGRA 4: Auto-marca como Cancelled se ficar vazio
}

// Validações para transição
public bool CanAddItem(OrderItem? item)    // Verifica se pode adicionar
public bool CanRemoveItem(OrderItem? item) // Verifica se pode remover
public bool CanTransitionTo(OrderStatus newStatus) // Valida transição de status

// Mudança de status
public void ChangeStatus(OrderStatus newStatus)
{
    // REGRA 6: Valida transição permitida
    // Transições: New → Pending → Processing → Shipped → Delivered
    // Cancelled acessível de qualquer estado
}

// Cálculos
public decimal GetTotal()        // Soma subtotais de todos os itens
public bool HasMinimumItems     // REGRA 2: Verifica 1 item mínimo
public bool HasItems            // Verifica se há itens
public int ItemCount            // Retorna quantidade de itens

// Validação completa
public void ValidateBusinessRules() // Valida todas as regras encapsuladas
```

#### 6 Regras de Negócio Implementadas

| # | Regra | Implementação | Método |
|---|-------|---------------|--------|
| 1 | Não pode adicionar itens a pedido Shipped | Status validation | `AddItem()` |
| 2 | Pedido deve ter no mínimo 1 item | Count validation | `HasMinimumItems` |
| 3 | Não pode remover último item | Count check | `RemoveItem()` |
| 4 | Auto-marca Cancelled se vazio | Auto-transition | `RemoveItem()` |
| 5 | Máximo 10 itens distintos | Limit check | `AddItem()` |
| 6 | Transições de status validadas | State machine | `ChangeStatus()` |

#### Code Snippet - Agregado Order

```csharp
public class Order : AggregateRoot, IEquatable<Order>
{
    private const int MaximumDistinctItems = 10;
    private const int MinimumItems = 1;

    public OrderId OrderId { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    private readonly List<OrderItem> _items = new();
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public static Order CreateOrder(OrderId orderId, CustomerId customerId, DateTime? orderDate = null)
    {
        if (orderId == null)
            throw new InvalidOrderException("OrderId não pode ser nulo");
        if (customerId == null)
            throw new InvalidOrderException("CustomerId não pode ser nulo");

        return new Order(orderId, customerId, orderDate ?? DateTime.UtcNow);
    }

    public void AddItem(OrderItem item)
    {
        if (item == null)
            throw new InvalidOrderException("Item não pode ser nulo");

        if (Status == OrderStatus.Shipped)
            throw new InvalidOrderException("Não é possível adicionar itens a um pedido que já foi enviado");

        if (_items.Count >= MaximumDistinctItems && !_items.Contains(item))
            throw new InvalidOrderException($"Não é possível adicionar mais de {MaximumDistinctItems} itens distintos no pedido");

        var existingItem = _items.FirstOrDefault(i => i.Equals(item));
        if (existingItem != null)
            _items.Remove(existingItem);

        _items.Add(item);
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        if (!CanTransitionTo(newStatus))
            throw new InvalidOrderException($"Transição de status inválida: não é possível passar de '{Status}' para '{newStatus}'");

        Status = newStatus;
    }

    public decimal GetTotal() => _items.Sum(item => item.GetSubtotal());
}
```

---

### 1.2 Entidade: `OrderItem`

**Localização**: [`src/OrderHub.Domain/Aggregates/Order/OrderItem.cs`](src/OrderHub.Domain/Aggregates/Order/OrderItem.cs)

**Tipo**: Entidade (não é Aggregate Root)

**Responsabilidade**: Representa um item dentro de um pedido com lógica encapsulada

#### Propriedades

| Propriedade | Tipo | Modificador | Descrição |
|-------------|------|------------|-----------|
| `Id` | `Guid` | private set | Identificador único da entidade |
| `ProductId` | `ProductId` | private set | Identificador do produto (Value Object) |
| `Quantity` | `int` | private set | Quantidade do produto |
| `Amount` | `OrderAmount` | private set | Preço unitário (Value Object) |

#### Métodos Principais

```csharp
// Construtor padrão (valida quantidade e null-checks)
public OrderItem(ProductId productId, int quantity, OrderAmount amount)

// Factory method com parâmetros simples
public static OrderItem Create(string productName, int quantity, decimal unitPrice)
{
    // Valida todos os parâmetros
    // Cria ProductId e OrderAmount internamente
}

// Cálculo
public decimal GetSubtotal() => Quantity * Amount.Value

// Igualdade por ID
public override bool Equals(object? obj)   // Implementa IEquatable<OrderItem>
public override int GetHashCode()          // Baseado em Id
```

#### Code Snippet - Entidade OrderItem

```csharp
public class OrderItem : IEquatable<OrderItem>
{
    public Guid Id { get; private set; }
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public OrderAmount Amount { get; private set; }

    public OrderItem(ProductId productId, int quantity, OrderAmount amount)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantity));

        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        Quantity = quantity;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Id = Guid.NewGuid();
    }

    public static OrderItem Create(string productName, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Nome do produto não pode estar vazio", nameof(productName));
        if (quantity <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantity));
        if (unitPrice <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(unitPrice));

        var productId = ProductId.Create(productName);
        var amount = OrderAmount.Create(unitPrice);
        return new OrderItem(productId, quantity, amount);
    }

    public decimal GetSubtotal() => Quantity * Amount.Value;
}
```

---

## 💎 2. VALUE OBJECTS

Value Objects são imutáveis, implementam igualdade por **valor** (não por referência) e encapsulam lógica de validação do domínio.

### 2.1 `OrderId` - Identificador de Pedido

**Localização**: [`src/OrderHub.Domain/ValueObjects/OrderId.cs`](src/OrderHub.Domain/ValueObjects/OrderId.cs)

```csharp
public class OrderId : IEquatable<OrderId>
{
    public Guid Value { get; }

    private OrderId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("OrderId não pode ser vazio", nameof(value));
        Value = value;
    }

    // Factory methods
    public static OrderId Create(Guid? value = null)
    {
        var id = value ?? Guid.NewGuid();
        return new OrderId(id);
    }

    public static OrderId Parse(string value)
    {
        if (!Guid.TryParse(value, out var id))
            throw new ArgumentException("Formato de OrderId inválido", nameof(value));
        return new OrderId(id);
    }

    // Igualdade por VALOR (não por referência)
    public bool Equals(OrderId? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((OrderId)obj);
    }

    public override int GetHashCode() => Value.GetHashCode();
}
```

**Características**:
- ✅ Imutável (Value é readonly)
- ✅ Igualdade por **valor** (baseado em Guid.Value)
- ✅ Validação: Não permite Guid.Empty
- ✅ Factory methods para criar ou parsear

---

### 2.2 `CustomerId` - Identificador de Cliente

**Localização**: [`src/OrderHub.Domain/ValueObjects/CustomerId.cs`](src/OrderHub.Domain/ValueObjects/CustomerId.cs)

Estrutura idêntica a `OrderId`:

```csharp
public class CustomerId : IEquatable<CustomerId>
{
    public Guid Value { get; }  // Imutável

    private CustomerId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("CustomerId não pode ser vazio", nameof(value));
        Value = value;
    }

    public static CustomerId Create(Guid? value = null) => new(value ?? Guid.NewGuid());
    public static CustomerId Parse(string value) => /* ... */;

    // Igualdade por valor
    public bool Equals(CustomerId? other) => other?.Value.Equals(Value) ?? false;
}
```

---

### 2.3 `ProductId` - Identificador de Produto

**Localização**: [`src/OrderHub.Domain/ValueObjects/ProductId.cs`](src/OrderHub.Domain/ValueObjects/ProductId.cs)

```csharp
public class ProductId : IEquatable<ProductId>
{
    public string Value { get; }  // Imutável

    private ProductId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("ProductId não pode ser vazio", nameof(value));
        Value = value.Trim();
    }

    public static ProductId Create(string value) => new(value);

    public static ProductId CreateNew()
        => new($"PROD-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}");

    public bool Equals(ProductId? other) => other != null && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();
}
```

**Diferença**: Usa `string` como valor base, não `Guid`

---

### 2.4 `OrderStatus` - Enum/Value Object de Status

**Localização**: [`src/OrderHub.Domain/ValueObjects/OrderStatus.cs`](src/OrderHub.Domain/ValueObjects/OrderStatus.cs)

```csharp
public enum OrderStatus
{
    New = 0,           // Estado inicial
    Pending = 1,       // Aguardando processamento
    Processing = 2,    // Em processamento
    Shipped = 3,       // Enviado
    Delivered = 4,     // Entregue
    Cancelled = 5      // Cancelado
}
```

**Transições Permitidas** (implementadas em `Order.CanTransitionTo()`):
```
New → Pending → Processing → Shipped → Delivered
              ↓
            Cancelled (acessível de qualquer estado)
```

---

### 2.5 `OrderAmount` - Valor Monetário Imutável

**Localização**: [`src/OrderHub.Domain/ValueObjects/OrderAmount.cs`](src/OrderHub.Domain/ValueObjects/OrderAmount.cs)

```csharp
public class OrderAmount : IEquatable<OrderAmount>
{
    // Campos readonly - Imutabilidade completa
    public readonly decimal Value;
    public readonly string Currency;

    private OrderAmount(decimal value, string currency)
    {
        Value = value;
        Currency = currency;
    }

    public static OrderAmount Create(decimal value, string currency = "BRL")
    {
        if (value <= 0)
            throw new ArgumentException("Valor deve ser maior que zero", nameof(value));
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Moeda não pode estar vazia", nameof(currency));
        if (currency.Length != 3)
            throw new ArgumentException("Código de moeda deve ter 3 caracteres", nameof(currency));

        var roundedValue = Math.Round(value, 2);
        return new OrderAmount(roundedValue, currency.ToUpperInvariant());
    }

    // Operações retornam NOVO OrderAmount (imutável)
    public OrderAmount Add(OrderAmount other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));
        if (other.Currency != Currency)
            throw new InvalidOperationException(
                $"Não é possível adicionar valores em moedas diferentes: {Currency} e {other.Currency}");

        return Create(Value + other.Value, Currency);
    }

    public OrderAmount Subtract(OrderAmount other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));
        if (other.Currency != Currency)
            throw new InvalidOperationException(
                $"Não é possível subtrair valores em moedas diferentes");

        var result = Value - other.Value;
        if (result <= 0)
            throw new InvalidOperationException("Resultado não pode ser menor ou igual a zero");

        return Create(result, Currency);
    }

    // Igualdade por valor
    public bool Equals(OrderAmount? other)
        => other != null && Value == other.Value && Currency == other.Currency;
}
```

**Características Especiais**:
- 🔒 Imutabilidade completa (campos readonly)
- 💱 Gerencia moeda (validação de código 3-letra)
- ➕ Operações retornam novo OrderAmount (padrão imutável)
- ✋ Impede operações com moedas diferentes
- 📊 Arredondamento automático para 2 casas decimais

---

## 🚪 3. OUTPUT PORTS (INTERFACES)

Ports são interfaces que definem contratos para adapters externos. O Domain **não conhece** as implementações, apenas define o que precisa.

### 3.1 `IOrderRepository` - Persistência de Pedidos

**Localização**: [`src/OrderHub.Domain/Ports/IOrderRepository.cs`](src/OrderHub.Domain/Ports/IOrderRepository.cs)

```csharp
public interface IOrderRepository
{
    /// <summary>
    /// Recupera um agregado Order pelo seu identificador
    /// </summary>
    Task<Order?> GetByIdAsync(OrderId orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupera todos os pedidos de um cliente
    /// </summary>
    Task<List<Order>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persiste um novo pedido ou atualiza um existente
    /// </summary>
    Task SaveAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove um pedido pelo seu identificador
    /// </summary>
    Task DeleteAsync(OrderId orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um pedido existe
    /// </summary>
    Task<bool> ExistsAsync(OrderId orderId, CancellationToken cancellationToken = default);
}
```

**Padrão**: Trabalha com **agregados completos** (Order), não com SQL ou queries

---

### 3.2 `IRepository<TEntity, TId>` - Repositório Genérico

**Localização**: [`src/OrderHub.Domain/Ports/IRepository.cs`](src/OrderHub.Domain/Ports/IRepository.cs)

```csharp
public interface IRepository<TEntity, TId> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(TId id, CancellationToken cancellationToken = default);
}
```

**Uso**: Oferece interface genérica reutilizável para outras entidades

---

### 3.3 `IUnitOfWork` - Gerência de Transações

**Localização**: [`src/OrderHub.Domain/Ports/IUnitOfWork.cs`](src/OrderHub.Domain/Ports/IUnitOfWork.cs)

```csharp
public interface IUnitOfWork : IAsyncDisposable
{
    IOrderRepository Orders { get; }

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
    bool HasActiveTransaction { get; }
}
```

**Padrão**: Coordena múltiplos repositórios em uma transação

---

### 3.4 `INotificationPort` - Notificações Externas

**Localização**: [`src/OrderHub.Domain/Ports/INotificationPort.cs`](src/OrderHub.Domain/Ports/INotificationPort.cs)

```csharp
public interface INotificationPort
{
    Task SendOrderConfirmationAsync(string customerId, string orderId, 
                                   CancellationToken cancellationToken = default);
    
    Task SendOrderApprovedAsync(string customerId, string orderId, 
                               CancellationToken cancellationToken = default);
    
    Task SendOrderShippedAsync(string customerId, string orderId, 
                              CancellationToken cancellationToken = default);
    
    Task SendOrderDeliveredAsync(string customerId, string orderId, 
                                CancellationToken cancellationToken = default);
}
```

**Implementações Possíveis**: Email, SMS, Push Notifications, Webhooks

---

### 3.5 `IPaymentPort` - Processamento de Pagamentos

**Localização**: [`src/OrderHub.Domain/Ports/IPaymentPort.cs`](src/OrderHub.Domain/Ports/IPaymentPort.cs)

```csharp
public interface IPaymentPort
{
    Task<bool> ValidatePaymentAsync(string customerId, decimal amount, 
                                   string currency = "BRL", 
                                   CancellationToken cancellationToken = default);
    
    Task<string> ProcessPaymentAsync(string customerId, decimal amount, 
                                    string currency = "BRL", 
                                    CancellationToken cancellationToken = default);
    
    Task<bool> RefundPaymentAsync(string paymentTransactionId, decimal amount, 
                                 CancellationToken cancellationToken = default);
    
    Task<string> GetPaymentStatusAsync(string paymentTransactionId, 
                                      CancellationToken cancellationToken = default);
}
```

**Integração**: Gateways de pagamento (Stripe, PayPal, PagSeguro, etc)

---

### 3.6 `ILoggingService` - Logging Centralizado

**Localização**: [`src/OrderHub.Domain/Ports/ILoggingService.cs`](src/OrderHub.Domain/Ports/ILoggingService.cs)

```csharp
public interface ILoggingService
{
    void LogDebug(string message, params object[] args);
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(string message, params object[] args);
    void LogCritical(string message, params object[] args);
}
```

**Desacoplamento**: Domain não depende de Serilog, NLog ou qualquer biblioteca específica

---

## ⚠️ 4. EXCEÇÕES DE DOMÍNIO

### 4.1 Hierarquia de Exceções

```
Exception (Framework)
└── DomainException (Base para o domínio)
    ├── InvalidOrderException
    └── InvalidOrderAmountException
```

### 4.2 `DomainException` - Exceção Base

**Localização**: [`src/OrderHub.Domain/Exceptions/DomainException.cs`](src/OrderHub.Domain/Exceptions/DomainException.cs)

```csharp
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception? innerException) 
        : base(message, innerException) { }
}
```

**Uso**: Base para todas as exceções de regras de negócio

---

### 4.3 `InvalidOrderException` - Violação de Regra da Order

**Localização**: [`src/OrderHub.Domain/Exceptions/InvalidOrderException.cs`](src/OrderHub.Domain/Exceptions/InvalidOrderException.cs)

```csharp
public class InvalidOrderException : DomainException
{
    public InvalidOrderException(string message) : base(message) { }
    public InvalidOrderException(string message, Exception? innerException) 
        : base(message, innerException) { }
}
```

**Exemplos de Lançamento**:
```csharp
throw new InvalidOrderException("Não é possível adicionar itens a um pedido que já foi enviado");
throw new InvalidOrderException("Não é possível remover o último item do pedido");
throw new InvalidOrderException("Transição de status inválida");
```

---

### 4.4 `InvalidOrderAmountException` - Valor Monetário Inválido

**Localização**: [`src/OrderHub.Domain/Exceptions/InvalidOrderAmountException.cs`](src/OrderHub.Domain/Exceptions/InvalidOrderAmountException.cs)

```csharp
public class InvalidOrderAmountException : DomainException
{
    public InvalidOrderAmountException(string message) : base(message) { }
    public InvalidOrderAmountException(string message, Exception? innerException) 
        : base(message, innerException) { }
}
```

---

## 🔍 5. DOMAIN VALIDATOR - Validação Centralizada

**Localização**: [`src/OrderHub.Domain/DomainValidator.cs`](src/OrderHub.Domain/DomainValidator.cs)

Classe estática que fornece **11 métodos de validação** reutilizáveis em todo o domínio:

### 5.1 Métodos de Validação

| # | Método | Assinatura | Descrição |
|---|--------|-----------|-----------|
| 1 | `ThrowIfNull<T>` | `ThrowIfNull<T>(T? value, string message)` | Valida se não é nulo |
| 2 | `ThrowIfNegativeOrZero` | `ThrowIfNegativeOrZero(decimal value, string message)` | Valida se > 0 |
| 3 | `ThrowIfNegative` | `ThrowIfNegative(decimal value, string message)` | Valida se >= 0 |
| 4 | `ThrowIfEmpty` (string) | `ThrowIfEmpty(string? value, string message)` | Valida string não vazia |
| 5 | `ThrowIfExceedsLength` | `ThrowIfExceedsLength(string? value, int maxLength, string message)` | Valida tamanho máximo |
| 6 | `ThrowIfInvalidEmail` | `ThrowIfInvalidEmail(string? email, string message)` | Valida formato email (regex) |
| 7 | `ThrowIfNotInRange` | `ThrowIfNotInRange(decimal value, decimal min, decimal max, string message)` | Valida intervalo |
| 8 | `ThrowIfEmpty` (Guid) | `ThrowIfEmpty(Guid value, string message)` | Valida Guid não vazio |
| 9 | `ThrowIf` | `ThrowIf(bool condition, string message)` | Lança se condição true |
| 10 | `ThrowIfNot` | `ThrowIfNot(bool condition, string message)` | Lança se condição false |
| 11 | `ThrowIfInvalid<T>` | `ThrowIfInvalid<T>(T value, Func<T, bool> isValid, string message)` | Lança se validação falhar |

### 5.2 Code Snippet - DomainValidator

```csharp
public static class DomainValidator
{
    public static void ThrowIfNull<T>(T? value, string message) where T : class
    {
        if (value == null)
            throw new DomainException(message);
    }

    public static void ThrowIfNegativeOrZero(decimal value, string message)
    {
        if (value <= 0)
            throw new DomainException(message);
    }

    public static void ThrowIfEmpty(string? value, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(message);
    }

    public static void ThrowIfExceedsLength(string? value, int maxLength, string message)
    {
        if (value == null)
            return;
        if (value.Length > maxLength)
            throw new DomainException(message);
    }

    public static void ThrowIfInvalidEmail(string? email, string message)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Email não pode estar vazio");

        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (!Regex.IsMatch(email, emailPattern))
            throw new DomainException(message);
    }

    public static void ThrowIfNotInRange(decimal value, decimal min, decimal max, string message)
    {
        if (value < min || value > max)
            throw new DomainException(message);
    }

    public static void ThrowIfEmpty(Guid value, string message)
    {
        if (value == Guid.Empty)
            throw new DomainException(message);
    }

    public static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new DomainException(message);
    }

    public static void ThrowIfNot(bool condition, string message)
    {
        if (!condition)
            throw new DomainException(message);
    }

    public static void ThrowIfInvalid<T>(T value, Func<T, bool> isValid, string message)
    {
        if (!isValid(value))
            throw new DomainException(message);
    }
}
```

### 5.3 Exemplos de Uso

```csharp
// Em um Value Object
DomainValidator.ThrowIfEmpty(email, "Email não pode estar vazio");
DomainValidator.ThrowIfInvalidEmail(email, "Formato de email inválido");

// Em um Agregado
DomainValidator.ThrowIfNegativeOrZero(quantity, "Quantidade deve ser maior que zero");
DomainValidator.ThrowIfNull(customerId, "Cliente não pode ser nulo");

// Com condição customizada
DomainValidator.ThrowIfInvalid(
    orderId,
    id => id != Guid.Empty,
    "OrderId não pode ser vazio"
);
```

---

## 🎯 6. DOMAIN EVENTS (Estado Atual)

**Localização**: `src/OrderHub.Domain/Events/` (VAZIA)

### Análise Atual

❌ **Nenhum domain event foi implementado**

### Infraestrutura Preparada

A classe base `AggregateRoot` já possui suporte completo para eventos:

```csharp
public abstract class AggregateRoot
{
    private readonly List<object> _domainEvents = new();
    
    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();
    
    protected void RaiseDomainEvent(object @event)
    {
        _domainEvents.Add(@event);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

### Pronto para Expansão

Quando implementados, eventos seriam assim:

```csharp
// Exemplo futuro - Não implementado ainda
public class OrderCreatedEvent
{
    public OrderId OrderId { get; set; }
    public CustomerId CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Uso no agregado
public static Order CreateOrder(OrderId orderId, CustomerId customerId, DateTime? orderDate = null)
{
    var order = new Order(orderId, customerId, orderDate ?? DateTime.UtcNow);
    order.RaiseDomainEvent(new OrderCreatedEvent 
    { 
        OrderId = orderId, 
        CustomerId = customerId, 
        CreatedAt = order.OrderDate 
    });
    return order;
}
```

---

## 📦 7. DEPENDÊNCIAS DO PROJETO

### 7.1 Arquivo `.csproj`

**Localização**: [`src/OrderHub.Domain/OrderHub.Domain.csproj`](src/OrderHub.Domain/OrderHub.Domain.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

### 7.2 Análise de Dependências

| Dependência | Presente | Motivo |
|-------------|----------|--------|
| Entity Framework Core | ❌ NÃO | Correto! EF é adapter (camada Persistence) |
| FluentValidation | ❌ NÃO | Correto! Validação é application concern |
| ASP.NET Core | ❌ NÃO | Correto! Controllers são adapters |
| AutoMapper | ❌ NÃO | Correto! Mapeamento é application concern |
| Logging Library | ❌ NÃO | Correto! Abstrato via ILoggingService |
| SQLite/SQL Server | ❌ NÃO | Correto! Database é adapter |

### 7.3 Conclusão sobre Dependências

✅ **ZERO dependências externas** - Apenas SDK .NET nativo

✅ **Arquitetura Hexagonal pura** - Nenhum acoplamento com camadas externas

✅ **DDD bem implementado** - Domain layer isolado e testável

---

## 📊 8. RESUMO QUANTITATIVO

### Entidades e Agregados

| Item | Quantidade | Tipo |
|------|-----------|------|
| Agregados Raiz | 1 | Order |
| Entidades | 1 | OrderItem |
| Value Objects | 5 | OrderId, CustomerId, ProductId, OrderStatus, OrderAmount |
| Enums | 1 | OrderStatus |

### Interfaces (Ports)

| Item | Quantidade | Categoria |
|------|-----------|-----------|
| Output Ports | 6 | IOrderRepository, IRepository<T,TId>, IUnitOfWork, INotificationPort, IPaymentPort, ILoggingService |
| Input Ports | 0 | (Implementadas na camada Application) |

### Exceções

| Item | Quantidade |
|------|-----------|
| Base Exception | 1 |
| Custom Exceptions | 2 |

### Validação

| Item | Quantidade |
|------|-----------|
| Métodos no DomainValidator | 11 |
| Métodos de Validação no Order | 6 |

### Domain Events

| Item | Quantidade |
|------|-----------|
| Events Implementados | 0 |
| Infraestrutura Preparada | ✅ Sim |

---

## 🎯 9. ANÁLISE DE CONFORMIDADE COM HEXAGONAL ARCHITECTURE

### Critérios Hexagonal Architecture

| Critério | Status | Evidência |
|----------|--------|-----------|
| Zero dependências externas | ✅ PASS | Nenhuma NuGet além do SDK nativo |
| Interfaces para adapters | ✅ PASS | 6 Output Ports definidas |
| Lógica de negócio isolada | ✅ PASS | Toda regra está em Aggregates/ValueObjects |
| Imutabilidade de ValueObjects | ✅ PASS | Todos implementam imutabilidade |
| Igualdade por valor | ✅ PASS | IEquatable<T> implementado |
| Validação de domínio | ✅ PASS | DomainValidator centralizado |
| Factory methods | ✅ PASS | ValueObjects e Aggregates usam factories |
| Aggregates bem definidos | ✅ PASS | Order como raiz, OrderItem como entidade |

### Critérios Domain-Driven Design

| Critério | Status | Evidência |
|----------|--------|-----------|
| Ubiquitous Language | ✅ PASS | Nomes em português alinhados com negócio |
| Aggregate Root | ✅ PASS | Order herda AggregateRoot |
| Bounded Context | ✅ PASS | OrderHub é um bounded context |
| Value Objects | ✅ PASS | 5 Value Objects identificados |
| Repositories | ✅ PASS | IOrderRepository interface |
| Domain Services | ⚠️ PARTIAL | DomainValidator como static class |
| Domain Events | ❌ NÃO | Infraestrutura sim, implementação não |
| Specifications | ❌ NÃO | Não implementado |

---

## 🔐 10. PRINCÍPIOS SOLID IMPLEMENTADOS

| Princípio | Implementação |
|-----------|--------------|
| **S** - Single Responsibility | Cada classe tem uma responsabilidade clara (Order, OrderItem, ValueObjects) |
| **O** - Open/Closed | Extensível via novos ValueObjects e Exceptions sem modificar existentes |
| **L** - Liskov Substitution | Order substitui AggregateRoot, Exceptions substituem base |
| **I** - Interface Segregation | Ports específicas (IOrderRepository, IPaymentPort, etc) |
| **D** - Dependency Inversion | Domain não depende de implementations, apenas interfaces |

---

## 📝 11. RECOMENDAÇÕES E PRÓXIMOS PASSOS

### 1. Implementar Domain Events
```csharp
// Criar events para:
- OrderCreatedEvent
- OrderItemAddedEvent
- OrderItemRemovedEvent
- OrderStatusChangedEvent
- OrderCancelledEvent
```

### 2. Considerar Domain Services
```csharp
// Se necessário, criar serviços para:
- OrderCalculationService (cálculos complexos)
- OrderTransitionService (transições de status)
- OrderValidationService (validações complexas)
```

### 3. Adicionar Specifications Pattern
```csharp
// Para queries mais complexas no futuro:
- OrdersByStatusSpecification
- OrdersByDateRangeSpecification
```

### 4. Melhorias de Testes
```
- Testar todas as regras de negócio
- Testar transições de status
- Testar constraints de quantidade
```

### 5. Documentação XML
- Adicionar `<example>` tags nos métodos públicos
- Documentar exceções com `<exception>`
- Adicionar `<returns>` mais descritivos

---

## 📚 12. ARQUIVOS E ESTRUTURA FINAL

```
src/OrderHub.Domain/
├── Aggregates/
│   ├── AggregateRoot.cs                 # Base para todos os agregados
│   └── Order/
│       ├── Order.cs                     # Aggregate Root principal
│       └── OrderItem.cs                 # Entidade aninhada
│
├── ValueObjects/
│   ├── OrderId.cs                       # Identificador de pedido
│   ├── CustomerId.cs                    # Identificador de cliente
│   ├── ProductId.cs                     # Identificador de produto
│   ├── OrderStatus.cs                   # Enum de status
│   └── OrderAmount.cs                   # Valor monetário
│
├── Ports/
│   ├── IOrderRepository.cs              # Persistência de pedidos
│   ├── IRepository.cs                   # Repositório genérico
│   ├── IUnitOfWork.cs                   # Gerência de transações
│   ├── INotificationPort.cs             # Notificações
│   ├── IPaymentPort.cs                  # Pagamentos
│   └── ILoggingService.cs               # Logging
│
├── Exceptions/
│   ├── DomainException.cs               # Base
│   ├── InvalidOrderException.cs         # Order
│   └── InvalidOrderAmountException.cs   # Amount
│
├── Events/
│   └── (VAZIO - pronto para eventos)
│
├── DomainValidator.cs                   # 11 métodos de validação
└── OrderHub.Domain.csproj               # Zero dependências externas
```

---

## ✅ CONCLUSÃO

A camada **Domain** do OrderHub está **muito bem estruturada**:

1. ✅ **Zero acoplamento** - Nenhuma dependência externa
2. ✅ **Regras encapsuladas** - 6 regras de negócio na Order
3. ✅ **Value Objects puros** - Imutáveis e com igualdade por valor
4. ✅ **Ports bem definidas** - 6 interfaces para adapters
5. ✅ **Validação centralizada** - 11 métodos reutilizáveis
6. ✅ **Exceções específicas** - Hierarquia clara
7. ⚠️ **Domain Events** - Infraestrutura pronta, implementação pendente

O projeto é um **exemplar educacional excelente** de Hexagonal Architecture com DDD.

---

**Relatório Completo - Fim**  
*Gerado em: Março 2026*
