# FEAT-11 | Plano de Ação - Testes de Integração

**Data**: 14 de Março de 2026  
**Feature**: FEAT-11 | Testes de Integração  
**Status**: Em Planejamento  
**Baseado em**: Azure DevOps Issue 78 (FEAT-11 | Testes de Integração)  
**Total de Tasks**: 4  
**Sprint**: Sprint 2  
**Consultado via MCP**: ✅ GetWorkItem (ID 78) e GetChildWorkItems  

---

## 📋 Visão Geral

Implementação de **testes de integração** para validar a interação entre camadas da arquitetura, especialmente entre **API Layer** e **Application Layer**, garantindo que a API funcionará corretamente com as dependências externas (banco de dados, repositories).

Este documento detalha as **4 tasks reais** da FEAT-11 conforme definidas no Azure DevOps Issue 78, organizadas para testar a integração completa da API.

---

## 🎯 Objetivo

Criar uma suite de testes de integração que:
- **Testa endpoints HTTP** da API (POST, GET, PUT, DELETE)
- **Valida fluxo completo** entre API → Application → Domain
- **Integra com banco de dados real** ou em-memória
- **Valida respostas HTTP** (status codes, headers, body)
- **Testa cenários de erro** (bad requests, not found, etc)
- **Garante contrato da API** com clientes externos

---

## 📊 Tasks da FEAT-11

### ✅ TASK-60: Criar testes de integração da API

**ID Azure DevOps**: 143  
**Título**: TASK-60 | Criar testes de integração da API  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Criar o projeto e estrutura base para testes de integração da API, configurando WebApplicationFactory e ambiente de testes.

**O que fazer**:

1. **Criar pasta de testes de integração**:
   ```bash
   mkdir tests/OrderHub.Api.IntegrationTests
   mkdir tests/OrderHub.Api.IntegrationTests/Controllers
   mkdir tests/OrderHub.Api.IntegrationTests/Fixtures
   mkdir tests/OrderHub.Api.IntegrationTests/Helpers
   ```

2. **Configurar projeto xUnit para testes de integração**:
   ```bash
   cd tests/
   dotnet new xunit -n OrderHub.Api.IntegrationTests
   cd ..
   dotnet sln add tests/OrderHub.Api.IntegrationTests/OrderHub.Api.IntegrationTests.csproj
   ```

3. **Adicionar NuGet packages necessários**:
   ```bash
   cd tests/OrderHub.Api.IntegrationTests
   dotnet add package xunit
   dotnet add package xunit.runner.visualstudio
   dotnet add package Microsoft.AspNetCore.Mvc.Testing
   dotnet add package Microsoft.EntityFrameworkCore.InMemory
   dotnet add package FluentAssertions
   ```

4. **Criar WebApplicationFactory customizada**:
   - Arquivo: `Fixtures/OrderHubWebApplicationFactory.cs`
   - Herdar de `WebApplicationFactory<Program>`
   - Configurar banco de dados em memória
   - Sobrescrever `ConfigureWebHost()` para setup de teste

5. **Configurar referências de projeto**:
   ```bash
   dotnet add reference ../../src/OrderHub.Adapters.Inbound.Api/OrderHub.Adapters.Inbound.Api.csproj
   dotnet add reference ../../src/OrderHub.Domain/OrderHub.Domain.csproj
   dotnet add reference ../../src/OrderHub.Application/OrderHub.Application.csproj
   ```

**Checklist**:
- [ ] Projeto OrderHub.Api.IntegrationTests criado
- [ ] Estrutura de pastas implementada
- [ ] WebApplicationFactory criado
- [ ] Banco em-memória configurado
- [ ] Referências de projeto configuradas
- [ ] NuGet packages instalados
- [ ] Solução compila sem erros
- [ ] Commit: `test: Create API integration tests project`

**Critério de Aceitação**:
- ✅ Projeto criado e compilável
- ✅ WebApplicationFactory funcional
- ✅ Banco de dados em memória operacional
- ✅ Estrutura pronta para testes de endpoint

---

### ✅ TASK-61: Testar endpoint POST /orders

**ID Azure DevOps**: 144  
**Título**: TASK-61 | Testar endpoint POST /orders  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Criar testes de integração para o endpoint POST /api/v1/orders que cria uma nova ordem.

**Estrutura Esperada**:
```csharp
public class OrdersControllerPostTests : IClassFixture<OrderHubWebApplicationFactory>
{
    private readonly OrderHubWebApplicationFactory _factory;
    private readonly HttpClient _client;
    
    public OrdersControllerPostTests(OrderHubWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task PostOrder_WithValidRequest_ShouldCreateAndReturn201()
    {
        // Arrange
        var request = new CreateOrderRequest 
        { 
            CustomerId = "cust-123",
            Items = new[] { new CreateOrderItemRequest { ... } }
        };
        var content = new StringContent(JsonConvert.SerializeObject(request), 
            Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync("/api/v1/orders", content);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var json = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<OrderResponse>(json);
        result.Should().NotBeNull();
    }
    
    [Fact]
    public async Task PostOrder_WithInvalidData_ShouldReturn400()
    {
        // Arrange, Act, Assert...
    }
    
    [Fact]
    public async Task PostOrder_WithEmptyItems_ShouldReturn400()
    {
        // Arrange, Act, Assert...
    }
}
```

**O que fazer**:
- [ ] Criar classe `OrdersControllerPostTests` em `Controllers/OrdersControllerPostTests.cs`
- [ ] Implementar teste para POST com dados válidos (esperado: 201 Created)
- [ ] Implementar teste para POST com dados inválidos (esperado: 400 Bad Request)
- [ ] Implementar teste para POST com itens vazios (esperado: 400 Bad Request)
- [ ] Implementar teste para resposta contém Order ID válido
- [ ] Implementar teste para recursos criados persistem no banco
- [ ] Usar FluentAssertions para assertions claras
- [ ] Documentar cada teste
- [ ] Arquivo em `tests/OrderHub.Api.IntegrationTests/Controllers/OrdersControllerPostTests.cs`
- [ ] Commit: `test: Add integration tests for POST /orders endpoint`

**Critério de Aceitação**:
- ✅ Endpoint POST testado completamente
- ✅ Todos cenários cobertos (sucesso, erro)
- ✅ HTTP status codes validados (201, 400)
- ✅ Response body verificado
- ✅ Testes passam com sucesso
- ✅ Integração com banco em memória funciona

---

### ✅ TASK-62: Testar endpoint GET /orders

**ID Azure DevOps**: 145  
**Título**: TASK-62 | Testar endpoint GET /orders  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Criar testes de integração para o endpoint GET /api/v1/orders/{id} que recupera uma ordem.

**Estrutura Esperada**:
```csharp
public class OrdersControllerGetTests : IClassFixture<OrderHubWebApplicationFactory>
{
    private readonly OrderHubWebApplicationFactory _factory;
    private readonly HttpClient _client;
    
    public OrdersControllerGetTests(OrderHubWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task GetOrder_WithValidId_ShouldReturn200AndOrder()
    {
        // Arrange - Criar ordem via POST primeiro
        var createRequest = new CreateOrderRequest { ... };
        var createContent = new StringContent(JsonConvert.SerializeObject(createRequest), 
            Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/v1/orders", createContent);
        var createdOrder = JsonConvert.DeserializeObject<OrderResponse>(
            await createResponse.Content.ReadAsStringAsync());
        
        // Act
        var response = await _client.GetAsync($"/api/v1/orders/{createdOrder.Id}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var order = JsonConvert.DeserializeObject<OrderResponse>(json);
        order.Id.Should().Be(createdOrder.Id);
    }
    
    [Fact]
    public async Task GetOrder_WithInvalidId_ShouldReturn404()
    {
        // Arrange, Act, Assert...
    }
}
```

**O que fazer**:
- [ ] Criar classe `OrdersControllerGetTests` em `Controllers/OrdersControllerGetTests.cs`
- [ ] Implementar teste para GET com ID válido (esperado: 200 OK)
- [ ] Implementar teste para GET com ID inválido (esperado: 404 Not Found)
- [ ] Implementar teste para GET com ID inexistente (esperado: 404 Not Found)
- [ ] Implementar teste para dados retornados correspondem ao que foi criado
- [ ] Usar factory para criar dados de teste
- [ ] Validar structure da response
- [ ] Usar FluentAssertions
- [ ] Arquivo em `tests/OrderHub.Api.IntegrationTests/Controllers/OrdersControllerGetTests.cs`
- [ ] Commit: `test: Add integration tests for GET /orders endpoint`

**Critério de Aceitação**:
- ✅ Endpoint GET testado completamente
- ✅ Cenários sucesso e erro cobertos
- ✅ HTTP status codes 200 e 404 validados
- ✅ Response body verificado
- ✅ Testes passam com sucesso
- ✅ Cross-test data isolation funciona

---

### ✅ TASK-63: Testar integração com banco

**ID Azure DevOps**: 146  
**Título**: TASK-63 | Testar integração com banco  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Criar testes de integração que validam a persistência de dados no banco de dados, garantindo que modificações via API são realmente salvadas.

**Estrutura Esperada**:
```csharp
public class DatabaseIntegrationTests : IClassFixture<OrderHubWebApplicationFactory>
{
    private readonly OrderHubWebApplicationFactory _factory;
    private readonly HttpClient _client;
    
    public DatabaseIntegrationTests(OrderHubWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateOrder_ShouldPersistToDatabase()
    {
        // Arrange
        var request = new CreateOrderRequest { ... };
        var content = new StringContent(JsonConvert.SerializeObject(request), 
            Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync("/api/v1/orders", content);
        var createdOrder = JsonConvert.DeserializeObject<OrderResponse>(
            await response.Content.ReadAsStringAsync());
        
        // Assert - Verificar no banco via novo client
        using var dbContext = _factory.CreateDbContext();
        var savedOrder = await dbContext.Orders.FindAsync(createdOrder.Id);
        savedOrder.Should().NotBeNull();
        savedOrder.CustomerId.Should().Be(request.CustomerId);
    }
    
    [Fact]
    public async Task UpdateOrder_ShouldPersistChanges()
    {
        // Arrange, Act, Assert...
    }
    
    [Fact]
    public async Task DeleteOrder_ShouldRemoveFromDatabase()
    {
        // Arrange, Act, Assert...
    }
}
```

**O que fazer**:
- [ ] Criar classe `DatabaseIntegrationTests` em `Database/DatabaseIntegrationTests.cs`
- [ ] Implementar teste para POST → verificar persistência de criação
- [ ] Implementar teste para PUT → verificar persistência de updates
- [ ] Implementar teste para DELETE → verificar remoção do banco
- [ ] Implementar teste para relacionamentos (Orders com OrderItems)
- [ ] Acessar DbContext diretamente para assertions no banco
- [ ] Validar transações (commit/rollback)
- [ ] Usar seed data se necessário
- [ ] Arquivo em `tests/OrderHub.Api.IntegrationTests/Database/DatabaseIntegrationTests.cs`
- [ ] Commit: `test: Add database integration tests`

**Critério de Aceitação**:
- ✅ Persistência de dados validada
- ✅ Updates refletidos no banco
- ✅ Soft/hard deletes funcionam
- ✅ Relacionamentos mantidos
- ✅ Testes passam com sucesso
- ✅ Transações funcionam corretamente

---

## 🔗 Relacionamentos

**Epic**: [EPIC-07 | Testes da Arquitetura](../EPIC-07/) (ID 76)  
**Feature**: FEAT-11 | Testes de Integração  
**Parent**: [EPIC-07 | Testes da Arquitetura](../EPIC-07/)  
**Predecessor**: [FEAT-10 | Testes Unitários](../FEAT-10_ACTION_PLAN.md)

---

## 📅 Timeline Estimada

| Milestone | Duração Estimada | Descrição |
|-----------|-----------------|-----------|
| TASK-60 | 1.5h | Projeto e WebApplicationFactory |
| TASK-61 | 2h | Testes endpoint POST |
| TASK-62 | 2h | Testes endpoint GET |
| TASK-63 | 2h | Testes de persistência |
| **TOTAL** | **7.5 horas** | Tempo total estimado |

---

## 🎓 Padrões e Conceitos Aplicados

### WebApplicationFactory Pattern
- **In-Memory Database**: EF Core InMemory para testes isolados
- **Test Server**: ASP.NET Core test server para HTTP testing
- **Factory Reuse**: WebApplicationFactory compartilhado entre testes

### Integration Testing Best Practices
- **Test Isolation**: Cada teste limpa dados antes de executar
- **Happy Path + Error Cases**: Testa sucesso e falha
- **HTTP Assertions**: Valida status codes, headers, body
- **Database Verification**: Confirma persistência real

### Test Data Management
- **Builder Pattern**: Facilitador fluido para criar test data
- **Fixtures**: Factory methods para setup comum
- **Seed Data**: Pré-carrega dados necessários
- **Cleanup**: Limpa estado após cada teste

### API Testing Approach
- **Client Simulation**: HttpClient como cliente real
- **JSON Serialization**: Newtonsoft.Json para conversão
- **Response Validation**: FluentAssertions para assertions
- **Status Code Verification**: Testa códigos HTTP corretos

---

## 🏗️ Arquitetura de Testes

### Test Pyramid Completo
```
        △ E2E Tests
       △ △ Integration Tests (FEAT-11)
      △ △ △ Unit Tests (FEAT-10)
```

### Camadas Testadas
- **API Controller Layer**: Endpoints HTTP
- **Application Layer**: Use Cases
- **Domain Layer**: Agregados (via Application)
- **Persistence Layer**: EF Core em-memória

---

## ✅ Checklist Final de Implementação

- [ ] Pasta EPIC-07 criada em DOC_IA
- [ ] Arquivo FEAT-11_ACTION_PLAN.md criado
- [ ] Projeto OrderHub.Api.IntegrationTests criado
- [ ] WebApplicationFactory configurada
- [ ] Testes POST /orders implementados
- [ ] Testes GET /orders implementados
- [ ] Testes de persistência implementados
- [ ] Todos os testes passam (green lights)
- [ ] Solução compila sem erros
- [ ] Merge em `develop` completado

---

## 🎯 Expansão Futura

### Testes Adicionais de Endpoints
- [ ] PUT /orders/{id} - Update order
- [ ] DELETE /orders/{id} - Delete order
- [ ] GET /customers/{id}/orders - List customer orders
- [ ] POST /orders/{id}/items - Add item to order

### Testes Avançados
- [ ] Testes de concorrência
- [ ] Testes de performance
- [ ] Testes de validação de headers
- [ ] Testes de autenticação/autorização

### CI/CD Integration
- [ ] Testes rodam em pipeline
- [ ] Coverage reports gerados
- [ ] Failed tests bloqueiam merge
- [ ] Performance trends monitorados

---

## 📚 Recursos e Referências

- **Microsoft.AspNetCore.Mvc.Testing**: [Documentação oficial](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests)
- **EF Core InMemory**: [Documentação oficial](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/)
- **FluentAssertions**: [Documentação oficial](https://fluentassertions.com/)
- **xUnit**: [Documentação oficial](https://xunit.net/)

---

**Próximas etapas**: Após conclusão da FEAT-11, proceder com testes de API contracts ou performance tests.
