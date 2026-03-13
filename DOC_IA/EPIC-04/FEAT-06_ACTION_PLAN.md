# FEAT-06 | Plano de Ação - API REST (Inbound Adapter)

**Data**: 13 de Março de 2026  
**Feature**: FEAT-06 | API REST (Inbound Adapter)  
**Status**: Em Planejamento  
**Baseado em**: Azure DevOps Issue 66 (FEAT-06)  
**Epic**: ID 70 - EPIC-04 | Implementação dos Adapters de Entrada  
**Total de Tasks**: 9  
**Sprint**: Sprint 1  
**Tempo Total Estimado**: 8-10 horas  
**Consultado via MCP**: ✅ GetWorkItem (ID 66), GetWorkItem (ID 70), GetRelatedWorkItems (ID 66)  

---

## 📋 Visão Geral

Implementação do adapter de entrada (Inbound Adapter) para a API REST seguindo os princípios de **Hexagonal Architecture** e **Clean Architecture**.

Esta camada atua como o portão de entrada da aplicação, expondo os casos de uso do domínio através de uma API REST bem estruturada e documentada.

---

## 🎯 Objetivo

Criar uma API REST robusta que:
- **Expõe Use Cases** através de endpoints REST padronizados
- **Implementa Controllers** para cada agregado de domínio
- **Mapeia DTOs** entre requisições HTTP e contratos de aplicação
- **Valida Entrada** antes de chegar aos use cases
- **Documenta API** com Swagger/OpenAPI
- **Integra com Use Cases** de forma desacoplada
- **Trata Erros** de forma apropriada com status HTTP corretos
- **Implementa Versionamento** de API (opcional)

---

## 📊 Tasks da FEAT-06

### TASK-31: Criar projeto OrderHub.Adapters.Inbound.Api

**ID Azure DevOps**: 114  
**Título**: TASK-31 | Criar projeto OrderHub.Adapters.Inbound.Api  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 45 min  

**Descrição**:
Criar o projeto ASP.NET Core 8 que conterá toda a implementação da API REST e controllers.

**O que fazer**:
```bash
# Criar projeto ASP.NET Core Web API
cd src/
dotnet new webapi -n OrderHub.Adapters.Inbound.Api -f net8.0

# Adicionar ao sln
cd ..
dotnet sln add src/OrderHub.Adapters.Inbound.Api/OrderHub.Adapters.Inbound.Api.csproj

# Adicionar dependências
cd src/OrderHub.Adapters.Inbound.Api
dotnet add reference ../OrderHub.Domain/OrderHub.Domain.csproj
dotnet add reference ../OrderHub.Application/OrderHub.Application.csproj
dotnet add reference ../OrderHub.Infrastructure/OrderHub.Infrastructure.csproj
dotnet add package Swashbuckle.AspNetCore --version 6.4.0
dotnet add package Microsoft.AspNetCore.Mvc.Versioning --version 5.1.0

# Criar estrutura de pastas
mkdir Controllers
mkdir Middleware
mkdir Mappers
mkdir Validators
mkdir Filters
mkdir Extensions
```

**Checklist**:
- [ ] Projeto OrderHub.Adapters.Inbound.Api criado com ASP.NET Core 8
- [ ] Referenciado em OrderHub.sln
- [ ] Dependências em Domain, Application e Infrastructure adicionadas
- [ ] Pacotes NuGet configurados (Swashbuckle, Versioning, etc)
- [ ] Todas pastas criadas corretamente
- [ ] Program.cs configurado com middleware padrão
- [ ] Solução compila sem erros
- [ ] Commit: `feat: Create OrderHub.Adapters.Inbound.Api project structure`

**Critério de Aceitação**:
- ✅ Projeto ASP.NET Core criado e compilável
- ✅ Estrutura de pastas implementada
- ✅ Referências aos outros projects funcionam
- ✅ Pacotes NuGet restaurados corretamente
- ✅ Nenhum warning de compilação

---

### TASK-32: Configurar dependências e middleware

**ID Azure DevOps**: (a ser verificado)  
**Título**: TASK-32 | Configurar dependências e middleware  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1.5h  

**Descrição**:
Configurar o Program.cs com todas as dependências, middleware e extensões necessárias.

**O que fazer**:

1. **Criar ServiceCollectionExtensions**:
   - Arquivo: `src/OrderHub.Adapters.Inbound.Api/Extensions/ServiceCollectionExtensions.cs`
   - Registrar todos os serviços da Application e Infrastructure
   - Configurar CORS, versioning, autenticação

2. **Configurar Middleware**:
   - Global exception handler
   - Request/response logging
   - Validation error handling

3. **Program.cs Setup**:
   ```csharp
   builder.Services.AddApiServices();
   builder.Services.AddApplication();
   builder.Services.AddInfrastructure(builder.Configuration);
   
   app.UseMiddleware<ExceptionHandlingMiddleware>();
   app.UseSwagger();
   app.UseSwaggerUI();
   app.UseCors("DefaultPolicy");
   ```

**Checklist**:
- [ ] ServiceCollectionExtensions criado
- [ ] Todas dependências registradas
- [ ] Middleware global configurado
- [ ] CORS configurado
- [ ] Versionamento de API configurado
- [ ] Tratamento de exceções global implementado
- [ ] Build sem erros
- [ ] Commit: `feat: Configure API dependencies and middleware`

**Critério de Aceitação**:
- ✅ Todas dependências registradas
- ✅ Middleware funcionando
- ✅ Build sem erros
- ✅ API inicia sem erros

---

### TASK-33: Criar controller OrdersController

**ID Azure DevOps**: 116  
**Título**: TASK-33 | Criar controller OrdersController  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1.5h  

**Descrição**:
Implementar o controller que expõe os casos de uso de Orders através de endpoints REST.

**O que fazer**:

1. **Criar classe OrdersController**:
   - Arquivo: `src/OrderHub.Adapters.Inbound.Api/Controllers/OrdersController.cs`
   - Endpoints:
     ```csharp
     [ApiController]
     [Route("api/v1/[controller]")]
     public class OrdersController : ControllerBase
     {
         [HttpPost]
         public async Task<IActionResult> CreateOrderAsync(CreateOrderRequest request);
         
         [HttpGet("{orderId}")]
         public async Task<IActionResult> GetOrderAsync(string orderId);
         
         [HttpPut("{orderId}")]
         public async Task<IActionResult> UpdateOrderAsync(string orderId, UpdateOrderRequest request);
         
         [HttpDelete("{orderId}")]
         public async Task<IActionResult> DeleteOrderAsync(string orderId);
         
         [HttpGet]
         public async Task<IActionResult> GetAllOrdersAsync();
     }
     ```

2. **Mapear DTOs**:
   - DTOs da Application → Response models
   - Validar requisições

**Checklist**:
- [ ] Controller OrdersController criado
- [ ] Endpoints CRUD implementados
- [ ] Dependency Injection de UseCases funciona
- [ ] DTOs mapeados corretamente
- [ ] Validações implementadas
- [ ] Tratamento de erros implementado
- [ ] Testes de integração criados
- [ ] Build sem erros
- [ ] Commit: `feat: Implement OrdersController with CRUD endpoints`

**Critério de Aceitação**:
- ✅ Todos endpoints CRUD funcionam
- ✅ DTOs mapeados corretamente
- ✅ Validações funcionam
- ✅ Build sem erros

---

### TASK-34: Mapear DTOs para responses

**ID Azure DevOps**: (a ser verificado)  
**Título**: TASK-34 | Mapear DTOs para responses  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1h  

**Descrição**:
Criar mappers para converter DTOs de entrada/saída para modelos de resposta HTTP.

**O que fazer**:

1. **Criar Response Models**:
   - Arquivo: `src/OrderHub.Adapters.Inbound.Api/Models/CreateOrderResponse.cs`
   - Arquivo: `src/OrderHub.Adapters.Inbound.Api/Models/GetOrderResponse.cs`
   - Response genérica com metadata

2. **Criar Mappers**:
   - Arquivo: `src/OrderHub.Adapters.Inbound.Api/Mappers/OrderResponseMapper.cs`
   - Usar AutoMapper ou manual mapping
   - Validar conversões

**Checklist**:
- [ ] Response models criados
- [ ] Mappers implementados
- [ ] Conversões testadas
- [ ] Build sem erros
- [ ] Commit: `feat: Implement DTO to response mapping`

**Critério de Aceitação**:
- ✅ Response models criados
- ✅ Mappers funcionam corretamente
- ✅ Build sem erros

---

### TASK-35: Implementar endpoint POST /orders

**ID Azure DevOps**: 118  
**Título**: TASK-35 | Implementar endpoint POST /orders  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1.5h  

**Descrição**:
Implementar em detalhe o endpoint POST para criação de pedidos com validações e tratamento de erro.

**O que fazer**:

1. **Implementar CreateOrder Action**:
   - Validar CreateOrderRequest (FluentValidation)
   - Chamar ICreateOrderUseCase
   - Mapear resposta para CreateOrderResponse
   - Retornar 201 (Created) com Location header

2. **Tratamento de Erros**:
   - DomainException → 400 Bad Request
   - ValidationException → 400 Bad Request
   - Exception → 500 Internal Server Error

3. **Response modelo**:
   ```csharp
   {
     "id": "order-uuid",
     "customerId": "customer-id",
     "status": "PENDING",
     "totalAmount": 299.99,
     "items": [...],
     "createdAt": "2026-03-13T10:30:00Z"
   }
   ```

**Checklist**:
- [ ] Endpoint POST implementado
- [ ] Validações funcionam
- [ ] Tratamento de erros implementado
- [ ] Status HTTP corretos (201, 400, 500)
- [ ] Response mapper funciona
- [ ] Testes unitários criados
- [ ] Build sem erros
- [ ] Commit: `feat: Implement POST /orders endpoint`

**Critério de Aceitação**:
- ✅ Endpoint cria pedido com sucesso
- ✅ Validações funcionam
- ✅ Status HTTP corretos
- ✅ Build sem erros

---

### TASK-36: Implementar endpoints GET e UPDATE

**ID Azure DevOps**: (a ser verificado)  
**Título**: TASK-36 | Implementar endpoints GET e UPDATE  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1.5h  

**Descrição**:
Implementar os endpoints de leitura e atualização de pedidos.

**O que fazer**:

1. **GET /orders/{orderId}**:
   - Chamar IGetOrderUseCase
   - Retornar 200 com pedido
   - Retornar 404 se não existir

2. **PUT /orders/{orderId}**:
   - Chamar IUpdateOrderUseCase
   - Validar UpdateOrderRequest
   - Retornar 200 com pedido atualizado
   - Retornar 404 se não existir

3. **GET /orders**:
   - Listar todos os pedidos
   - Retornar 200 com lista paginada

**Checklist**:
- [ ] Endpoints GET e PUT implementados
- [ ] Validações funcionam
- [ ] Tratamento de 404 implementado
- [ ] Paginação implementada
- [ ] Testes de integração criados
- [ ] Build sem erros
- [ ] Commit: `feat: Implement GET and PUT /orders endpoints`

**Critério de Aceitação**:
- ✅ Endpoints funcionam corretamente
- ✅ Status HTTP apropriados
- ✅ Build sem erros

---

### TASK-37: Integrar controllers com UseCases

**ID Azure DevOps**: 120  
**Título**: TASK-37 | Integrar controllers com UseCases  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1.5h  

**Descrição**:
Integrar todos os controllers com os UseCases da Application Layer de forma desacoplada.

**O que fazer**:

1. **Dependency Injection nos Controllers**:
   - Injetar ICreateOrderUseCase
   - Injetar IGetOrderUseCase
   - Injetar IUpdateOrderUseCase (se existir)

2. **Chamadas dos UseCases**:
   - Controllers → UseCases
   - Garantir que a Application Layer não conhece HTTP
   - Mapear exceções da Application para respostas HTTP

3. **Testes de Integração**:
   - Controller test com mock de UseCases
   - E2E test com verdadeira integração

**Checklist**:
- [ ] Controllers injetam UseCases
- [ ] Chamadas aos UseCases funcionam
- [ ] Exceções mapeadas para HTTP status
- [ ] Testes de integração criados
- [ ] Build sem erros
- [ ] Commit: `feat: Integrate controllers with use cases`

**Critério de Aceitação**:
- ✅ Controllers chamam UseCases corretamente
- ✅ Desacoplamento mantido
- ✅ Build sem erros

---

### TASK-38: Configurar Swagger/OpenAPI

**ID Azure DevOps**: 121  
**Título**: TASK-38 | Configurar Swagger/OpenAPI  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1.5h  

**Descrição**:
Configurar Swagger/OpenAPI para documentação automática da API.

**O que fazer**:

1. **Configurar Swashbuckle**:
   - Arquivo: `src/OrderHub.Adapters.Inbound.Api/Extensions/SwaggerServiceCollectionExtensions.cs`
   - Configurar documentação da API
   - Adicionar informações de versão

2. **XML Comments**:
   - Adicionar comentários XML em controllers
   - Adicionar comentários em modelos
   - Gerar documentação automática

3. **Cors**:
   - Configurar CORS para Swagger UI
   - Permitir requisições cruzadas

4. **Exemplo de configuração**:
   ```csharp
   services.AddSwaggerGen(c =>
   {
       c.SwaggerDoc("v1", new OpenApiInfo 
       { 
           Title = "OrderHub API", 
           Version = "v1" 
       });
       c.IncludeXmlComments(xmlPath);
   });
   
   app.UseSwagger();
   app.UseSwaggerUI();
   ```

**Checklist**:
- [ ] Swashbuckle configurado
- [ ] Swagger disponível em /swagger/index.html
- [ ] XML comments documentam endpoints
- [ ] Modelos documentados
- [ ] CORS configurado para Swagger
- [ ] Build sem erros
- [ ] Commit: `feat: Configure Swagger/OpenAPI documentation`

**Critério de Aceitação**:
- ✅ Swagger UI funciona
- ✅ Todos endpoints documentados
- ✅ Modelos aparecem corretamente
- ✅ Build sem erros

---

### TASK-39: Implementar validações em controllers

**ID Azure DevOps**: (a ser verificado)  
**Título**: TASK-39 | Implementar validações em controllers  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1h  

**Descrição**:
Implementar validações de entrada usando FluentValidation para garantir integridade dos dados.

**O que fazer**:

1. **Criar Validators**:
   - Arquivo: `src/OrderHub.Adapters.Inbound.Api/Validators/CreateOrderRequestValidator.cs`
   - Usar FluentValidation para regras de validação
   - Validar tipos, tamanho, formato

2. **Registrar Validators**:
   - Registrar em ServiceCollectionExtensions
   - Usar middleware para aplicar automaticamente

3. **Resposta de Erro**:
   ```json
   {
     "errors": {
       "customerId": ["Customer ID is required"],
       "items": ["At least one item is required"]
     }
   }
   ```

**Checklist**:
- [ ] Validators criados com FluentValidation
- [ ] Registrados no DI
- [ ] Middleware de validação implementado
- [ ] Respostas de erro formatadas
- [ ] Build sem erros
- [ ] Commit: `feat: Implement request validation in controllers`

**Critério de Aceitação**:
- ✅ Validações funcionam
- ✅ Respostas de erro apropriadas
- ✅ Build sem erros

---

### TASK-40: Testes de integração da API

**ID Azure DevOps**: (a ser verificado)  
**Título**: TASK-40 | Testes de integração da API  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 1  
**Tempo Estimado**: 2h  

**Descrição**:
Criar testes de integração para validar os endpoints da API.

**O que fazer**:

1. **WebApplicationFactory**:
   - Criar TestWebApplicationFactory
   - Configurar base de dados em memória
   - Setup de dados de teste

2. **Testes dos Endpoints**:
   - Teste POST /orders com sucesso (201)
   - Teste POST /orders com dados inválidos (400)
   - Teste GET /orders/{orderId} com sucesso (200)
   - Teste GET /orders/{orderId} inexistente (404)
   - Teste PUT /orders com sucesso
   - Teste DELETE /orders

3. **xUnit + Fluent Assertions**:
   ```csharp
   public class OrdersControllerTests
   {
       [Fact]
       public async Task CreateOrder_WithValidData_Returns201()
       {
           // Arrange
           var request = new CreateOrderRequest { ... };
           
           // Act
           var response = await _httpClient.PostAsync("/api/v1/orders", ...);
           
           // Assert
           response.StatusCode.Should().Be(HttpStatusCode.Created);
       }
   }
   ```

**Checklist**:
- [ ] WebApplicationFactory criada
- [ ] Testes para cada endpoint
- [ ] Dados de teste configurados
- [ ] Testes passando
- [ ] Build sem erros
- [ ] Commit: `test: Add API integration tests`

**Critério de Aceitação**:
- ✅ Todos endpoints testados
- ✅ Cobertura de 80%+
- ✅ Build sem erros

---

## 📋 Resumo das Entregas

| Task ID | Tarefa | Tipo | Estimado | Status |
|---------|--------|------|----------|--------|
| 114 | Criar projeto Adapters.Inbound.Api | Setup | 45 min | To Do |
| - | Configurar dependências e middleware | Configuration | 1.5h | To Do |
| 116 | Criar controller OrdersController | Development | 1.5h | To Do |
| - | Mapear DTOs para responses | Development | 1h | To Do |
| 118 | Implementar endpoint POST /orders | Development | 1.5h | To Do |
| - | Implementar endpoints GET e UPDATE | Development | 1.5h | To Do |
| 120 | Integrar controllers com UseCases | Integration | 1.5h | To Do |
| 121 | Configurar Swagger/OpenAPI | Documentation | 1.5h | To Do |
| - | Implementar validações em controllers | Validation | 1h | To Do |
| - | Testes de integração da API | Testing | 2h | To Do |
| **TOTAL** | | | **8-10h** | |

---

## ✅ Critérios de Conclusão

- ✅ Todos os 9 tasks completados
- ✅ Projeto ASP.NET Core compilável
- ✅ Todos endpoints CRUD funcionando
- ✅ Swagger/OpenAPI documentando API
- ✅ Validações de entrada implementadas
- ✅ Testes de integração passando
- ✅ Build pipeline verde
- ✅ Zero erros de compilação
- ✅ Code review aprovado
- ✅ Mergeado para develop branch

---

## 🔗 Referências

- **EPIC-04**: ID 70 - Implementação dos Adapters de Entrada
- **FEAT-06**: ID 66 - API REST (Inbound Adapter)
- **Padrão de Arquitetura**: Hexagonal Architecture + Clean Architecture
- **Framework**: ASP.NET Core 8.0
- **Documentação**: OpenAPI/Swagger 3.0
- **Validação**: FluentValidation 11.x
- **Testes**: xUnit + Fluent Assertions

---

**Próximo passo**: Após conclusão de FEAT-06, avaliar FEAT-07 ou adapters de saída/persistência.
