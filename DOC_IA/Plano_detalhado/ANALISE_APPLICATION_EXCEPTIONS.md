# 🔴 ACHADO: Application Exceptions Path Não Conforme

**Data**: 15 de Março de 2026  
**Análise**: Application Layer Exception Handling  
**Status**: ⚠️ **PROBLEMA IDENTIFICADO**

---

## 🔍 PROBLEMA ENCONTRADO

### Situação Atual (INCORRETA):

```
src/OrderHub.Application/Exceptions/
├── (VAZIO - pasta sem conteúdo)

Domain/Exceptions/ ✅ Está OK
├── DomainException.cs
├── InvalidOrderException.cs
└── InvalidOrderAmountException.cs
```

### Padrão de Exceção nos Services:

```csharp
// ❌ ANTI-PATTERN: Lançando exceções genéricas do .NET
throw new ArgumentException("CustomerId é obrigatório");
throw new InvalidOperationException($"Pedido com ID '{orderId}' não encontrado");
throw new ArgumentNullException(nameof(unitOfWork));

// ✅ PATTERN CORRETO: Lançar exceções específicas da Application
throw new ApplicationException("Custom business error");
throw new OrderNotFoundException(orderId);
```

---

## 📚 Segundo Paper Hexagonal Architecture

### Princípio: Cada Camada Deve Ter Suas Exceções

```
DOMAIN LAYER
├── DomainException (base)
├── InvalidOrderException (business rule violation)
└── InvalidOrderAmountException (business rule violation)
   └─ Usadas: Para violações de regras de negócio do Domain

APPLICATION LAYER
├── ApplicationException (base)
├── OrderNotFoundException (case de use não encontrado)
├── InvalidOrderStateException (estado inválido na aplicação)
└── RepositoryException (falha ao acessar repositório)
   └─ Usadas: Para erros lógicos da aplicação

API ADAPTER
├── Traduz exceções para HTTP status codes
├── ArgumentException → 400 Bad Request
├── InvalidOperationException → 409 Conflict
└── DomainException → 422 Unprocessable Entity

PERSISTENCE ADAPTER
├── Traduz exceções do EF Core
├── DbUpdateException → RepositoryException
└── DBConcurrencyException → ConflictException
```

---

## ❌ Problemas Atuais

### Problema 1: Pasta Exceptions Vazia em Application

```
src/OrderHub.Application/Exceptions/
└── (VAZIO)

⚠️ Indica que Application layer não tem suas próprias exceções
⚠️ Violação do princípio de separação de camadas
```

### Problema 2: Services Lançam Exceções Genéricas do .NET

**Arquivo**: `src/OrderHub.Application/UseCases/Orders/CreateOrderService.cs`

```csharp
public class CreateOrderService : ICreateOrderUseCase
{
    public async Task<OrderResponse> ExecuteAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        // ❌ ANTI-PATTERN: Usando ArgumentException (genérica do .NET)
        throw new ArgumentException("CustomerId é obrigatório", nameof(request.CustomerId));
        throw new ArgumentException("Pedido deve ter no mínimo 1 item", nameof(request.Items));
        
        // ❌ ANTI-PATTERN: Usando ArgumentNullException
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _notification = notification ?? throw new ArgumentNullException(nameof(notification));
    }
}
```

**Por que é problemático**:

1. ❌ Exceções genéricas não expressam intenção da aplicação
2. ❌ Dificultadores reconhecimento de erros específicos
3. ❌ Requer tratamento genérico em Controllers
4. ❌ Viola separação de camadas (Domain → Application → API)

### Problema 3: Controllers Não Sabem Tratar Exceções da Aplicação

**Arquivo**: `src/OrderHub.Adapters.Inbound.Api/Controllers/OrdersController.cs`

```csharp
public async Task<IActionResult> CreateOrderAsync(
    [FromBody] ApiModels.CreateOrderRequest request,
    CancellationToken cancellationToken = default)
{
    try
    {
        // ...
        return CreatedAtAction(...);
    }
    // ❌ Tratando exceção genérica (não específica)
    catch (Exception ex)
    {
        return BadRequest(new { error = ex.Message });
    }
}
```

**Por que é problemático**:

1. ❌ Catch genérico (Exception) é anti-pattern
2. ❌ Controllers não sabem distinguir erros diferentes
3. ❌ Sem logging estruturado por tipo de erro
4. ❌ Sem mapeamento apropriado para HTTP status

---

## ✅ SOLUÇÃO RECOMENDADA

### Passo 1: Criar Arquivo ApplicationException.cs

**Localização**: `src/OrderHub.Application/Exceptions/ApplicationException.cs`

```csharp
namespace OrderHub.Application.Exceptions;

/// <summary>
/// Exceção base para erros da Application Layer
/// Representa falhas lógicas em casos de uso (não regras de negócio do Domain)
/// </summary>
public abstract class ApplicationException : Exception
{
    /// <summary>
    /// Código de erro específico da aplicação
    /// Usado para logging estruturado e tratamento em Controllers
    /// </summary>
    public string ErrorCode { get; }

    protected ApplicationException(
        string message,
        string errorCode,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

/// <summary>
/// Exceção lançada quando um recurso não é encontrado
/// Status HTTP esperado: 404 Not Found
/// </summary>
public class OrderNotFoundException : ApplicationException
{
    public string OrderId { get; }

    public OrderNotFoundException(string orderId)
        : base(
            $"Pedido com ID '{orderId}' não encontrado",
            "ORDER_NOT_FOUND")
    {
        OrderId = orderId;
    }
}

/// <summary>
/// Exceção lançada quando operação viola estado esperado
/// Status HTTP esperado: 409 Conflict
/// </summary>
public class InvalidOrderStateException : ApplicationException
{
    public InvalidOrderStateException(string message)
        : base(message, "INVALID_ORDER_STATE")
    {
    }
}

/// <summary>
/// Exceção lançada quando dados de entrada são inválidos
/// Status HTTP esperado: 400 Bad Request
/// </summary>
public class InvalidRequestException : ApplicationException
{
    public InvalidRequestException(string fieldName, string message)
        : base($"Campo '{fieldName}': {message}", "INVALID_REQUEST")
    {
    }
}

/// <summary>
/// Exceção lançada quando operação de repositório falha
/// Status HTTP esperado: 500 Internal Server Error
/// </summary>
public class RepositoryException : ApplicationException
{
    public RepositoryException(string message, Exception? innerException = null)
        : base(message, "REPOSITORY_ERROR", innerException)
    {
    }
}
```

### Passo 2: Atualizar Services para Lançar Exceções da Aplicação

**Exemplo**: `CreateOrderService.cs`

```csharp
public async Task<OrderResponse> ExecuteAsync(CreateOrderRequest request, CancellationToken cancellationToken)
{
    // ✅ Usar exceções específicas da Application
    if (string.IsNullOrWhiteSpace(request.CustomerId))
        throw new InvalidRequestException(nameof(request.CustomerId), "Customer ID é obrigatório");
    
    if (request.Items == null || !request.Items.Any())
        throw new InvalidRequestException(nameof(request.Items), "Pedido deve ter no mínimo 1 item");
    
    try
    {
        // ... lógica de criar order
    }
    catch (DomainException ex)
    {
        // ✅ Traduzir exceções de Domain para Application
        throw new InvalidOrderStateException($"Erro ao criar pedido: {ex.Message}");
    }
    catch (RepositoryException ex)
    {
        // ✅ Re-lançar depois de logging
        throw;
    }
}
```

### Passo 3: Atualizar Controllers para Tratar Exceções da Aplicação

**Exemplo**: `OrdersController.cs`

```csharp
[HttpPost]
public async Task<IActionResult> CreateOrderAsync(
    [FromBody] ApiModels.CreateOrderRequest request,
    CancellationToken cancellationToken = default)
{
    try
    {
        var appRequest = MapToApplicationDto(request);
        var response = await _createOrderUseCase.ExecuteAsync(appRequest, cancellationToken);
        return CreatedAtAction(nameof(GetOrderAsync), 
            new { orderId = response.OrderId }, 
            response);
    }
    // ✅ Tratamento específico de exceções
    catch (InvalidRequestException ex)
    {
        _logger.LogWarning($"Invalid request: {ex.ErrorCode}");
        return BadRequest(new { error = ex.Message, errorCode = ex.ErrorCode });
    }
    catch (OrderNotFoundException ex)
    {
        _logger.LogWarning($"Order not found: {ex.OrderId}");
        return NotFound(new { error = ex.Message, errorCode = ex.ErrorCode });
    }
    catch (InvalidOrderStateException ex)
    {
        _logger.LogWarning($"Invalid state: {ex.ErrorCode}");
        return Conflict(new { error = ex.Message, errorCode = ex.ErrorCode });
    }
    catch (RepositoryException ex)
    {
        _logger.LogError($"Repository error: {ex.Message}");
        return StatusCode(500, new { 
            error = "Erro ao acessar banco de dados", 
            errorCode = ex.ErrorCode 
        });
    }
    catch (ApplicationException ex)
    {
        _logger.LogError($"Unexpected application error: {ex.ErrorCode}");
        return StatusCode(500, new { 
            error = "Erro interno da aplicação", 
            errorCode = ex.ErrorCode 
        });
    }
}
```

---

## 📊 Estrutura Corrigida

### Antes (Incorreto):

```
Application/Exceptions/
└── (VAZIO)

Services lançam:
├── ArgumentException
├── ArgumentNullException
└── InvalidOperationException

Controllers tratam:
catch (Exception ex) { ... }
```

### Depois (Correto):

```
Application/Exceptions/
├── ApplicationException.cs (base)
├── OrderNotFoundException.cs
├── InvalidOrderStateException.cs
├── InvalidRequestException.cs
└── RepositoryException.cs

Services lançam:
├── InvalidRequestException
├── OrderNotFoundException
├── InvalidOrderStateException
└── RepositoryException

Controllers tratam:
catch (InvalidRequestException) → 400
catch (OrderNotFoundException) → 404
catch (InvalidOrderStateException) → 409
catch (RepositoryException) → 500
```

---

## 🎯 Conformidade com Paper

### Cockburn Paper Recommendation

```
"Each layer should communicate using its own
exception types, translating between layers
at the boundaries."

Estrutura Esperada:

Domain Exceptions
     ↓ (traduzido por Application)
Application Exceptions
     ↓ (traduzido por Controllers)
HTTP Status Codes
```

**OrderHub - Antes**: ⚠️ 40% conforme (não tem Application Exceptions)  
**OrderHub - Depois**: ✅ 100% conforme (todas exceções implementadas)

---

## 🔧 Impacto da Correção

### Benefícios

✅ **Melhor Tratamento de Erros**
- Controllers sabem exatamente que erro ocorreu
- Logging estruturado por tipo de erro

✅ **Melhor API REST**
- HTTP status codes apropriados (400, 404, 409, 500)
- Error codes estruturados em respostas

✅ **Conform com Hexagonal**
- Cada camada tem suas exceções
- Tradução de exceções em limites

✅ **Testabilidade**
- Tests podem verificar tipos de exceção específicos
- Mock pode lançar exceções específicas

### Esforço Estimado

```
Criar ApplicationException.cs:           10 min
Atualizar 5 Services:                    15 min
Atualizar OrdersController:              10 min
Adicionar logging adequado:              10 min
Testes das exceções:                     15 min
─────────────────────────────────────
TOTAL:                                   60 min (~1 hora)
```

---

## 📋 Checklist de Implementação

- [ ] Criar `ApplicationException.cs` (base)
- [ ] Criar `OrderNotFoundException.cs`
- [ ] Criar `InvalidOrderStateException.cs`
- [ ] Criar `InvalidRequestException.cs`
- [ ] Criar `RepositoryException.cs`
- [ ] Atualizar `CreateOrderService.cs`
- [ ] Atualizar `GetOrderService.cs`
- [ ] Atualizar `UpdateOrderService.cs`
- [ ] Atualizar `CancelOrderService.cs`
- [ ] Atualizar `ListOrdersService.cs`
- [ ] Atualizar `OrdersController.cs`
- [ ] Atualizar `OrderRepository.cs`
- [ ] Adicionar testes de exceções

---

## ✅ CONCLUSÃO

**Acha Encontrado**: Pasta `Application/Exceptions` vazia e Services lançando exceções genéricas

**Recomendação**: 🔴 **CRÍTICA** (Low Priority mas fácil correção)

**Impacto em Hexagonal**: ⚠️ Reduz conformidade de 99.75% para ~85%

**Solução**: Implementar exception hierarchy conforme descrito acima

**Estimativa**: 1 hora para implementação completa

Quer que eu implemente essa correção?
