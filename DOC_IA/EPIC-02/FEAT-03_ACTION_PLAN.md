# FEAT-03 | Plano de Ação - Application Layer

**Data**: 12 de Março de 2026  
**Feature**: FEAT-03 | Application Layer  
**Status**: Em Planejamento  
**Baseado em**: Azure DevOps Issue 63 (FEAT-03 | Application Layer)  
**Total de Tasks**: 6  
**Sprint**: Sprint 1  
**Consultado via MCP**: ✅ GetWorkItem (ID 63)  

---

## 📋 Visão Geral

Implementação da camada de aplicação (Application Layer) seguindo os princípios de **Hexagonal Architecture** e **Clean Architecture**.

Esta camada atua como orquestrador, coordenando a lógica de negócios (Domain) com adaptadores externos e apresentação.

---

## 🎯 Objetivo

Criar uma camada de aplicação robusta que:
- **Coordena Use Cases** (Application Services)
- **Define Ports** (Interfaces para adaptadores)
- **Encapsula DTOs** para transferência de dados
- **Implementa Command/Query Pattern** para organização
- **Valida Entrada** antes de alcançar o domínio
- **Trata Exceções** de forma apropriada

---

## 📊 Tasks da FEAT-03

### TASK-15: Criar projeto OrderHub.Application

**ID Azure DevOps**: (será atribuído)  
**Título**: TASK-15 | Criar projeto OrderHub.Application  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 30 min  

**Descrição**:
Criar o projeto de biblioteca de classes .NET 8 que conterá toda a lógica de aplicação e orquestração.

**O que fazer**:
```bash
# Criar projeto class library
cd src/
dotnet new classlib -n OrderHub.Application -f net8.0

# Adicionar ao sln
cd ..
dotnet sln add src/OrderHub.Application/OrderHub.Application.csproj

# Adicionar referência ao projeto Domain
cd src/OrderHub.Application
dotnet add reference ../OrderHub.Domain/OrderHub.Domain.csproj
cd ../..

# Criar estrutura de pastas
mkdir src/OrderHub.Application/UseCases
mkdir src/OrderHub.Application/UseCases/Orders
mkdir src/OrderHub.Application/DTOs
mkdir src/OrderHub.Application/Ports
mkdir src/OrderHub.Application/Commands
mkdir src/OrderHub.Application/Queries
mkdir src/OrderHub.Application/Validators
mkdir src/OrderHub.Application/Exceptions
mkdir src/OrderHub.Application/Mappers
```

**Checklist**:
- [ ] Projeto OrderHub.Application criado com .NET 8
- [ ] Referenciado em OrderHub.sln
- [ ] Dependência em OrderHub.Domain adicionada
- [ ] Todas pastas criadas corretamente
- [ ] Arquivo .csproj configurado com `nullable` enabled
- [ ] Solução compila sem erros
- [ ] Commit: `feat: Create OrderHub.Application project structure`

**Critério de Aceitação**:
- ✅ Projeto OrderHub.Application criado e compilável
- ✅ Estrutura de pastas implementada
- ✅ Referência ao Domain project funciona
- ✅ Nenhum warning de compilação

---

### TASK-16: Criar DTOs (Data Transfer Objects)

**ID Azure DevOps**: (será atribuído)  
**Título**: TASK-16 | Criar DTOs para Orders  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1h  

**Descrição**:
Implementar DTOs para transferência de dados entre camadas, desacoplando a domain model da apresentação.

**Estrutura Esperada**:
```csharp
namespace OrderHub.Application.DTOs
{
    // Request DTOs
    public record CreateOrderRequest
    {
        public string CustomerId { get; init; }
        public List<OrderItemRequest> Items { get; init; }
    }

    public record OrderItemRequest
    {
        public string ProductId { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
    }

    // Response DTOs
    public record OrderResponse
    {
        public string OrderId { get; init; }
        public string CustomerId { get; init; }
        public DateTime OrderDate { get; init; }
        public string Status { get; init; }
        public List<OrderItemResponse> Items { get; init; }
        public decimal TotalAmount { get; init; }
    }

    public record OrderItemResponse
    {
        public string ProductId { get; init; }
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal SubTotal { get; init; }
    }
}
```

**O que fazer**:
- [ ] Criar classe `CreateOrderRequest.cs` em `src/OrderHub.Application/DTOs/`
- [ ] Criar classe `OrderItemRequest.cs`
- [ ] Criar classe `UpdateOrderRequest.cs`
- [ ] Criar classe `OrderResponse.cs`
- [ ] Criar classe `OrderItemResponse.cs`
- [ ] Implementar como `record` (C# 9+) para imutabilidade
- [ ] Adicionar validação básica (nullability, ranges)
- [ ] Implementar `ToString()` para logging
- [ ] Criar arquivo `Mappers/OrderMapper.cs` para converter entre Domain e DTOs
- [ ] Implementar método `ToEntity()` (DTO → Domain)
- [ ] Implementar método `ToResponse()` (Domain → DTO)
- [ ] Commit: `feat: Implement DTOs and mappers`

**Critério de Aceitação**:
- ✅ Todos DTOs criados e compiláveis
- ✅ Records implementados corretamente
- ✅ Mappers funcionam bidirecionalmente
- ✅ Sem exposição de domain internals

---

### TASK-17: Criar Ports (Interfaces de Adaptadores)

**ID Azure DevOps**: (será atribuído)  
**Título**: TASK-17 | Criar Ports (Interfaces)  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 45 min  

**Descrição**:
Definir as interfaces (Ports) que representam o contrato entre a Application Layer e os Adapters (persistência, APIs externas, etc).

**Estrutura Esperada**:
```csharp
namespace OrderHub.Application.Ports
{
    // Repository Port
    public interface IOrderRepository
    {
        Task<OrderResponse> GetByIdAsync(string orderId);
        Task<List<OrderResponse>> GetByCustomerIdAsync(string customerId);
        Task SaveAsync(Order order);
        Task DeleteAsync(string orderId);
    }

    // Unit of Work Port
    public interface IUnitOfWork
    {
        IOrderRepository Orders { get; }
        Task CommitAsync();
        Task RollbackAsync();
    }

    // External Service Ports
    public interface INotificationPort
    {
        Task SendOrderConfirmationAsync(string customerId, string orderId);
        Task SendShipmentNotificationAsync(string customerId, string orderId);
    }

    public interface IPaymentPort
    {
        Task<bool> ValidatePaymentAsync(string customerId, decimal amount);
        Task<string> ProcessPaymentAsync(string customerId, decimal amount);
    }
}
```

**O que fazer**:
- [ ] Criar interface `IOrderRepository.cs` em `src/OrderHub.Application/Ports/`
- [ ] Criar interface `IUnitOfWork.cs`
- [ ] Criar interface `INotificationPort.cs` (para envio de emails/notificações)
- [ ] Criar interface `IPaymentPort.cs` (para processamento de pagamentos)
- [ ] Criar interface `ILogger.cs` ou use `ILogger<T>` do framework
- [ ] Criar interface `IDateTimeProvider.cs` (para injetar Date/Time)
- [ ] Documentar cada método com XML comments
- [ ] Adicionar métodos async primariamente
- [ ] Commit: `feat: Define application ports (interfaces)`

**Critério de Aceitação**:
- ✅ Todas as interfaces criadas e bem definidas
- ✅ Contrato claro entre camadas
- ✅ Sem dependências circulares
- ✅ Métodos async/await padronizados

---

### TASK-18: Criar Application Services (Use Cases)

**ID Azure DevOps**: (será atribuído)  
**Título**: TASK-18 | Criar Application Services  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1.5h  

**Descrição**:
Implementar Application Services (orquestradores de use cases) que coordenam a lógica de aplicação.

**Estrutura Esperada**:
```csharp
namespace OrderHub.Application.UseCases.Orders
{
    public class CreateOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationPort _notification;
        private readonly ILogger<CreateOrderService> _logger;

        public CreateOrderService(
            IUnitOfWork unitOfWork,
            INotificationPort notification,
            ILogger<CreateOrderService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _notification = notification ?? throw new ArgumentNullException(nameof(notification));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OrderResponse> ExecuteAsync(CreateOrderRequest request)
        {
            // Validação
            ValidateRequest(request);

            // Criar agregado de domínio
            var order = Order.CreateOrder(
                OrderId.Create(),
                CustomerId.Create(request.CustomerId)
            );

            // Adicionar itens
            foreach (var item in request.Items)
            {
                order.AddItem(new OrderItem(/* ... */));
            }

            // Persistir
            await _unitOfWork.Orders.SaveAsync(order);
            await _unitOfWork.CommitAsync();

            // Notificar
            await _notification.SendOrderConfirmationAsync(
                request.CustomerId,
                order.OrderId.Value
            );

            return new OrderResponse(/* ... */);
        }
    }
}
```

**O que fazer**:
- [ ] Criar classe `CreateOrderService.cs` em `src/OrderHub.Application/UseCases/Orders/`
- [ ] Criar classe `GetOrderService.cs` para recuperar pedido
- [ ] Criar classe `UpdateOrderService.cs` para atualizar pedido
- [ ] Criar classe `CancelOrderService.cs` para cancelar pedido
- [ ] Implementar constructor injection de dependencies
- [ ] Adicionar validações de entrada
- [ ] Implementar logging apropriado
- [ ] Usar async/await em todas operações
- [ ] Tratar exceções de domínio corretamente
- [ ] Implementar transações via UnitOfWork
- [ ] Commit: `feat: Implement application services`

**Critério de Aceitação**:
- ✅ Serviços de aplicação funcionales e compiláveis
- ✅ Dependency injection implementado
- ✅ Logging presente em pontos críticos
- ✅ Transações gerenciadas corretamente

---

### TASK-19: Criar Validators (Validações de Entrada)

**ID Azure DevOps**: (será atribuído)  
**Título**: TASK-19 | Criar Validators  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1h  

**Descrição**:
Implementar validadores de entrada para garantir que os dados cheguem bem formados na Application Layer.

**Estrutura Esperada**:
```csharp
namespace OrderHub.Application.Validators
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId é obrigatório")
                .Length(1, 50).WithMessage("CustomerId deve ter entre 1 e 50 caracteres");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Pedido deve ter pelo menos 1 item")
                .Must(items => items.Count <= 10).WithMessage("Máximo 10 itens por pedido");

            RuleForEach(x => x.Items)
                .SetValidator(new OrderItemRequestValidator());
        }
    }

    public class OrderItemRequestValidator : AbstractValidator<OrderItemRequest>
    {
        public OrderItemRequestValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId é obrigatório");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantidade deve ser maior que 0");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Preço unitário deve ser maior que 0");
        }
    }
}
```

**O que fazer**:
- [ ] Instalar pacote `FluentValidation` via NuGet
- [ ] Criar classe `CreateOrderRequestValidator.cs` em `src/OrderHub.Application/Validators/`
- [ ] Criar classe `OrderItemRequestValidator.cs`
- [ ] Criar classe `UpdateOrderRequestValidator.cs`
- [ ] Implementar validações para cada DTO
- [ ] Usar mensagens de erro em português
- [ ] Adicionar validações de negócio (min/max items, etc)
- [ ] Criar extensão para validar automaticamente em services
- [ ] Commit: `feat: Implement input validators with FluentValidation`

**Critério de Aceitação**:
- ✅ Validadores implementados e testáveis
- ✅ Mensagens de erro clara em português
- ✅ Validações cobrem regras de negócio
- ✅ Sem re-validação desnecessária

---

### TASK-20: Criar Unit Tests para Application Layer

**ID Azure DevOps**: (será atribuído)  
**Título**: TASK-20 | Unit Tests Application Layer  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1.5h  

**Descrição**:
Implementar testes unitários abrangentes para toda a Application Layer com cobertura >80%.

**Estrutura Esperada**:
```csharp
namespace OrderHub.Application.Tests.UseCases.Orders
{
    public class CreateOrderServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<INotificationPort> _mockNotification;
        private readonly Mock<ILogger<CreateOrderService>> _mockLogger;
        private readonly CreateOrderService _service;

        public CreateOrderServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockNotification = new Mock<INotificationPort>();
            _mockLogger = new Mock<ILogger<CreateOrderService>>();
            _service = new CreateOrderService(
                _mockUnitOfWork.Object,
                _mockNotification.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task ExecuteAsync_WithValidRequest_CreatesOrderSuccessfully()
        {
            // Arrange
            var request = new CreateOrderRequest
            {
                CustomerId = "CUST-001",
                Items = new List<OrderItemRequest>
                {
                    new OrderItemRequest { ProductId = "PROD-001", Quantity = 2, UnitPrice = 100 }
                }
            };

            // Act
            var result = await _service.ExecuteAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("CUST-001", result.CustomerId);
            _mockUnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
            _mockNotification.Verify(x => x.SendOrderConfirmationAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
```

**O que fazer**:
- [ ] Criar projeto `tests/OrderHub.Application.Tests` (xUnit)
- [ ] Adicionar dependência em `Moq` para mocks
- [ ] Criar fixture `OrderRequestFixture.cs` para test data
- [ ] Criar testes para `CreateOrderService` (5 testes)
- [ ] Criar testes para `GetOrderService` (3 testes)
- [ ] Criar testes para `UpdateOrderService` (4 testes)
- [ ] Criar testes para Validators (5 testes)
- [ ] Criar testes para Mappers (3 testes)
- [ ] Implementar mocks de Ports correctamente
- [ ] Adicionar teste de integração simples (sem DB)
- [ ] Atingir >80% cobertura de código
- [ ] Commit: `feat: Implement application layer unit tests`

**Checklist**:
- [ ] Todos os testes executam com sucesso
- [ ] Cobertura >80% na Application Layer
- [ ] Mocks funcionam corretamente
- [ ] Testes são independentes e repetiveis
- [ ] Fixtures reutilizam dados
- [ ] Aspekt F.I.R.S.T. mantido (Fast, Independent, Repeatable, Self-checking, Timely)

**Critério de Aceitação**:
- ✅ Testes implementados e passando
- ✅ Cobertura >= 80%
- ✅ Testes cobrem happy path e error cases
- ✅ Execução rápida (<5 segundos)

---

## 📈 Progresso Esperado

| Task | Descrição | Tempo Est. | Status |
|------|-----------|-----------|--------|
| TASK-15 | Criar projeto Application | 30 min | ⏳ To Do |
| TASK-16 | Criar DTOs e Mappers | 1h | ⏳ To Do |
| TASK-17 | Criar Ports (Interfaces) | 45 min | ⏳ To Do |
| TASK-18 | Criar Application Services | 1.5h | ⏳ To Do |
| TASK-19 | Criar Validators | 1h | ⏳ To Do |
| TASK-20 | Unit Tests | 1.5h | ⏳ To Do |
| **TOTAL** | **6 Tasks** | **~6h** | ⏳ Em Planejamento |

---

## 🔗 Dependências

- ✅ **FEAT-02 completada** - Domain Layer implementada e testada
- 📦 **Pacotes NuGet necessários**:
  - `FluentValidation`
  - `Moq` (para testes)

---

## 🎓 Conceitos-Chave

### Application Layer
A Application Layer atua como orquestrador entre o Domain (lógica de negócio) e os Adapters (persistência, APIs externas).

**Responsabilidades**:
- Coordenar Use Cases
- Transformar DTOs ↔ Domain Entities
- Orquestrar transações
- Gerenciar notificações
- Validar entrada

**O que NÃO fazer aqui**:
- ❌ Lógica de negócio (vai no Domain)
- ❌ Detalhes de persistência (vai nos Adapters)
- ❌ Detalhes de apresentação (vai na API/UI)

---

## 📝 Notas de Implementação

1. **Async/Await**: Sempre usar async em operações de I/O
2. **Logging**: Registrar em pontos críticos (entrada, erro, saída)
3. **Validação Dupla**: Validar em Application + Domain
4. **Dependency Injection**: Usar constructor injection sempre
5. **Testes**: Mock todos os Ports, testar logic

---

## ✅ Checklist Final (FEAT-03)

- [ ] Todas as 6 Tasks completadas
- [ ] Solução compila sem erros/warnings
- [ ] Todos os testes passam
- [ ] Cobertura >= 80% application layer
- [ ] Commits atomicamente realizados
- [ ] README atualizado
- [ ] Completion Report gerado
- [ ] FEAT-03 marcada como Done no Azure DevOps
