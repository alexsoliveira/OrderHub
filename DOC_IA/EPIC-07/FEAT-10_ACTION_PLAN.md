# FEAT-10 | Plano de Ação - Testes Unitários

**Data**: 14 de Março de 2026  
**Feature**: FEAT-10 | Testes Unitários  
**Status**: Em Planejamento  
**Baseado em**: Azure DevOps Issue 77 (FEAT-10 | Testes Unitários)  
**Total de Tasks**: 4  
**Sprint**: Sprint 2  
**Consultado via MCP**: ✅ GetWorkItem (ID 77) e GetChildWorkItems  

---

## 📋 Visão Geral

Implementação de **testes unitários** para os componentes principais da aplicação OrderHub, seguindo as best practices de **Hexagonal Architecture** com foco na **validação da camada de Domínio** e **Aplicação**.

Este documento detalha as **4 tasks reais** da FEAT-10 conforme definidas no Azure DevOps Issue 77, organizadas para criar o projeto de testes e implementar cobertura de testes para entidades e use cases.

---

## 🎯 Objetivo

Criar uma suite de testes unitários que:
- **Valida entidades de domínio** (Order, OrderItem, etc.)
- **Testa casos de uso** (Application Services)
- **Garante isolamento de componentes** com mocks e stubs
- **Estabelece cobertura de testes** para código crítico
- **Documenta comportamento esperado** da aplicação através dos testes

---

## 📊 Tasks da FEAT-10

### ✅ TASK-55: Criar projeto OrderHub.UnitTests

**ID Azure DevOps**: 138  
**Título**: TASK-55 | Criar projeto OrderHub.UnitTests  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Criar o projeto de testes unitários para a aplicação OrderHub com estrutura de pastas alinhada aos projetos a serem testados.

**O que fazer**:

1. **Criar projeto xUnit test**:
   ```bash
   cd tests/
   dotnet new xunit -n OrderHub.UnitTests
   cd ..
   dotnet sln add tests/OrderHub.UnitTests/OrderHub.UnitTests.csproj
   ```

2. **Adicionar estrutura de pastas**:
   ```bash
   mkdir tests/OrderHub.UnitTests/Domain
   mkdir tests/OrderHub.UnitTests/Domain/Aggregates
   mkdir tests/OrderHub.UnitTests/Domain/ValueObjects
   mkdir tests/OrderHub.UnitTests/Application
   mkdir tests/OrderHub.UnitTests/Application/UseCases
   mkdir tests/OrderHub.UnitTests/Fixtures
   ```

3. **Configurar referências de projeto**:
   ```bash
   cd tests/OrderHub.UnitTests
   dotnet add reference ../../src/OrderHub.Domain/OrderHub.Domain.csproj
   dotnet add reference ../../src/OrderHub.Application/OrderHub.Application.csproj
   dotnet add reference ../../src/OrderHub.Adapters.Outbound.Persistence/OrderHub.Adapters.Outbound.Persistence.csproj
   ```

4. **Adicionar packages NuGet necessários**:
   ```bash
   dotnet add package xunit
   dotnet add package xunit.runner.visualstudio
   dotnet add package Moq
   dotnet add package Newtonsoft.Json
   ```

**Checklist**:
- [ ] Projeto OrderHub.UnitTests criado com framework xUnit
- [ ] Referenciado em OrderHub.sln
- [ ] Todas pastas criadas (Domain, Application, Fixtures)
- [ ] Referências de projeto configuradas para Domain e Application
- [ ] Packages NuGet instalados (xunit, Moq)
- [ ] Solução compila sem erros
- [ ] Commit: `test: Create OrderHub.UnitTests project structure`

**Critério de Aceitação**:
- ✅ Projeto OrderHub.UnitTests criado e compilável
- ✅ Estrutura de pastas implementada refletindo organização de código
- ✅ Referenciado corretamente em OrderHub.sln
- ✅ Packages de teste instalados (xunit, Moq, etc)
- ✅ Nenhum warning de compilação

---

### ✅ TASK-56: Criar testes para entidade Order

**ID Azure DevOps**: 139  
**Título**: TASK-56 | Criar testes para entidade Order  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Criar testes unitários para validar o comportamento da entidade agregada `Order` (DDD Aggregate Root) e suas operações críticas.

**Estrutura Esperada**:
```csharp
public class OrderTests
{
    [Fact]
    public void CreateOrder_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var customerId = new CustomerId(Guid.NewGuid());
        var items = new List<OrderItem> { /* ... */ };
        
        // Act
        var order = Order.Create(customerId, items);
        
        // Assert
        Assert.NotNull(order);
        Assert.Equal(customerId, order.CustomerId);
    }
    
    [Fact]
    public void CreateOrder_WithNullCustomerId_ShouldThrowException()
    {
        // Arrange & Act & Assert
        Assert.Throws<DomainException>(() => 
            Order.Create(null, new List<OrderItem>()));
    }
    
    [Fact]
    public void UpdateOrder_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange, Act, Assert...
    }
    
    [Fact]
    public void CancelOrder_WithValidReason_ShouldCancelSuccessfully()
    {
        // Arrange, Act, Assert...
    }
}
```

**O que fazer**:
- [ ] Criar classe `OrderTests` em `Domain/Aggregates/OrderTests.cs`
- [ ] Implementar teste para criação de Order com dados válidos
- [ ] Implementar teste para exceção com CustomerId null
- [ ] Implementar teste para atualização de Order
- [ ] Implementar teste para cancelamento de Order
- [ ] Implementar teste para validações de business rules
- [ ] Documentar cada teste com arrange-act-assert pattern
- [ ] Usar `xUnit` com assertions do `Assert` ou FluentAssertions
- [ ] Arquivo em `tests/OrderHub.UnitTests/Domain/Aggregates/OrderTests.cs`
- [ ] Commit: `test: Add unit tests for Order aggregate`

**Critério de Aceitação**:
- ✅ Testes cobrem operações principais de Order
- ✅ Validação de business rules funciona
- ✅ Exceções de domínio são lançadas corretamente
- ✅ Testes passam com sucesso
- ✅ Cobertura de pelo menos 80% da entidade Order

---

### ✅ TASK-57: Criar testes para UseCase CreateOrder

**ID Azure DevOps**: 140  
**Título**: TASK-57 | Criar testes para UseCase CreateOrder  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Criar testes unitários para validar o comportamento do use case `CreateOrder`, incluindo integração com repositório mock e validações de application layer.

**Estrutura Esperada**:
```csharp
public class CreateOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly CreateOrderUseCase _useCase;
    
    public CreateOrderUseCaseTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _useCase = new CreateOrderUseCase(_mockRepository.Object);
    }
    
    [Fact]
    public async Task Execute_WithValidRequest_ShouldCreateOrderSuccessfully()
    {
        // Arrange
        var request = new CreateOrderDto { /* ... */ };
        
        // Act
        var result = await _useCase.ExecuteAsync(request, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        _mockRepository.Verify(r => r.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task Execute_WithInvalidCustomerId_ShouldThrowException()
    {
        // Arrange, Act, Assert...
    }
    
    [Fact]
    public async Task Execute_WithEmptyItems_ShouldThrowException()
    {
        // Arrange, Act, Assert...
    }
}
```

**O que fazer**:
- [ ] Criar classe `CreateOrderUseCaseTests` em `Application/UseCases/CreateOrderUseCaseTests.cs`
- [ ] Configurar setup com Mock<IOrderRepository>
- [ ] Implementar teste para criação com dados válidos
- [ ] Implementar teste para exceção com CustomerId inválido
- [ ] Implementar teste para exceção com items vazios
- [ ] Verificar chamadas do repository com Moq.Verify()
- [ ] Usar padrão AAA (Arrange-Act-Assert)
- [ ] Documentar cada teste com comentários
- [ ] Arquivo em `tests/OrderHub.UnitTests/Application/UseCases/CreateOrderUseCaseTests.cs`
- [ ] Commit: `test: Add unit tests for CreateOrder use case`

**Critério de Aceitação**:
- ✅ Use case é testado com sucesso
- ✅ Repositório é mockado corretamente
- ✅ Validações de negócio são testadas
- ✅ Chamadas ao repositório são verificadas
- ✅ Testes passam com sucesso
- ✅ Cobertura de pelo menos 80% do use case

---

### ✅ TASK-58: Criar testes para UseCase GetOrder

**ID Azure DevOps**: 141  
**Título**: TASK-58 | Criar testes para UseCase GetOrder  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Criar testes unitários para validar o comportamento do use case `GetOrder`, incluindo cenários de ordem encontrada e não encontrada.

**Estrutura Esperada**:
```csharp
public class GetOrderUseCaseTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly GetOrderUseCase _useCase;
    
    public GetOrderUseCaseTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _useCase = new GetOrderUseCase(_mockRepository.Object);
    }
    
    [Fact]
    public async Task Execute_WithValidOrderId_ShouldReturnOrderSuccessfully()
    {
        // Arrange
        var orderId = new OrderId(Guid.NewGuid());
        var order = Order.Create(/* ... */);
        _mockRepository.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        
        // Act
        var result = await _useCase.ExecuteAsync(new GetOrderDto { Id = orderId.Value }, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(orderId.Value, result.Id);
    }
    
    [Fact]
    public async Task Execute_WithNonExistentOrderId_ShouldThrowException()
    {
        // Arrange, Act, Assert...
    }
    
    [Fact]
    public async Task Execute_RepositoryIsCalledOnce()
    {
        // Arrange, Act, Assert...
    }
}
```

**O que fazer**:
- [ ] Criar classe `GetOrderUseCaseTests` em `Application/UseCases/GetOrderUseCaseTests.cs`
- [ ] Configurar setup com Mock<IOrderRepository>
- [ ] Implementar teste para busca de ordem existente
- [ ] Implementar teste para exceção com ordem não encontrada
- [ ] Implementar teste para verificar chamada única ao repositório
- [ ] Usar Setup/Returns do Moq para simular dados
- [ ] Verificar chamadas com Verify()
- [ ] Usar padrão AAA (Arrange-Act-Assert)
- [ ] Comparar resultado retornado com dados mockados
- [ ] Arquivo em `tests/OrderHub.UnitTests/Application/UseCases/GetOrderUseCaseTests.cs`
- [ ] Commit: `test: Add unit tests for GetOrder use case`

**Critério de Aceitação**:
- ✅ Use case GetOrder é testado com sucesso
- ✅ Cenários de ordem encontrada funcionam
- ✅ Cenários de ordem não encontrada tratam exceção
- ✅ Repository é mockado e verificado
- ✅ Testes passam com sucesso
- ✅ Cobertura de pelo menos 80% do use case

---

## 🔗 Relacionamentos

**Epic**: [EPIC-07 | Testes da Arquitetura](../EPIC-07/) (ID 76)  
**Feature**: FEAT-10 | Testes Unitários  
**Parent**: [EPIC-07 | Testes da Arquitetura](../EPIC-07/)

---

## 📅 Timeline Estimada

| Milestone | Duração Estimada | Descrição |
|-----------|-----------------|-----------|
| TASK-55 | 1h | Criar projeto e estrutura de testes |
| TASK-56 | 2.5h | Testes para entidade Order |
| TASK-57 | 2.5h | Testes para CreateOrder UseCase |
| TASK-58 | 2.5h | Testes para GetOrder UseCase |
| **TOTAL** | **8.5 horas** | Tempo total estimado |

---

## 🎓 Padrões e Conceitos Aplicados

### Test Pyramid Pattern
- **Unit Tests**: Testes de unidades isoladas (entidades, use cases)
- **Mocks & Stubs**: Simulação de dependências externas
- **Fixtures**: Dados compartilhados entre testes

### xUnit Framework Features
- **[Fact]**: Testes de caso único
- **[Theory]**: Testes parametrizados (para expansão futura)
- **Assert**: Validações com xUnit assertions
- **Async/Await**: Testes assíncronos para use cases

### Moq Mocking Framework
- **Mock<T>**: Criação de mocks de interfaces
- **Setup()**: Configuração de comportamento
- **Returns/ReturnsAsync**: Definição de valores de retorno
- **Verify()**: Verificação de chamadas

### DDD Testing Approach
- **Domain Tests**: Validam invariantes de negócio
- **Application Tests**: Validam orquestração de domínio
- **Isolation**: Cada camada testada isoladamente
- **Mocks para Output Ports**: Repositories são mockados

### Test Organization
- **AAA Pattern**: Arrange (preparar), Act (executar), Assert (validar)
- **Naming Convention**: {MethodName}_{Scenario}_{ExpectedResult}
- **Fixture Sharing**: Use de constructores para setup comum
- **Test Data**: Builders ou factory methods para dados complexos

---

## ✅ Checklist Final de Implementação

- [ ] Pasta EPIC-07 criada em DOC_IA
- [ ] Arquivo FEAT-10_ACTION_PLAN.md criado
- [ ] Projeto OrderHub.UnitTests criado
- [ ] Estrutura de pastas implementada
- [ ] Testes para entidade Order implementados
- [ ] Testes para CreateOrder UseCase implementados
- [ ] Testes para GetOrder UseCase implementados
- [ ] Todos os testes passam (green lights)
- [ ] Cobertura de código validada
- [ ] Solução compila sem erros
- [ ] Merge em `develop` completado

---

## 🎯 Expansão Futura

Após conclusão da FEAT-10, próximas features podem incluir:

### Testes Adicionais
- [ ] **FEAT-11**: Testes para UpdateOrder e CancelOrder UseCases
- [ ] **FEAT-12**: Testes para OrderItem aggregate
- [ ] **FEAT-13**: Testes de integração (Integration Tests)
- [ ] **FEAT-14**: Testes de API (API/E2E Tests)

### Qualidade de Código
- [ ] Code Coverage reporting
- [ ] SonarQube integration
- [ ] Performance benchmarks
- [ ] Mutation testing

### CI/CD
- [ ] Testes rodam automaticamente em PRs
- [ ] Coverage reports gerados
- [ ] Failed tests bloqueiam merge
- [ ] Test trends em dashboard

---

**Próximas etapas**: Após conclusão da FEAT-10, proceder com FEAT-11 (Testes para mais UseCases) ou outras features do EPIC-07.
