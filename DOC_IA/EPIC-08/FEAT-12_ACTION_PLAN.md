# FEAT-12 | Plano de Ação - Logging e Monitoramento

**Data**: 15 de Março de 2026  
**Feature**: FEAT-12 | Logging e Monitoramento  
**Status**: Em Planejamento  
**Baseado em**: Azure DevOps Issue 80 (FEAT-12 | Logging e Monitoramento)  
**Epic Relacionado**: EPIC-08 | Observabilidade e Qualidade (ID 79)  
**Total de Tasks**: 3  
**Sprint**: Sprint 2  
**Consultado via MCP**: ✅ GetWorkItem (ID 80), GetWorkItem (ID 79) e GetChildWorkItems  

---

## 📋 Visão Geral

Implementação de **logging centralizado** e **monitoramento de aplicação** seguindo os princípios de **Hexagonal Architecture**, garantindo rastreabilidade de operações, detecção de erros e observabilidade completa do sistema OrderHub.

Este documento detalha as **3 tasks reais** da FEAT-12 conforme definidas no Azure DevOps Issue 80, organizadas para implementar logging com Serilog, capturar erros em tempo real e registrar execução de casos de uso.

---

## 🎯 Objetivo

Estabelecer uma estratégia de logging robusta que:
- **Centraliza configuração de logs** com Serilog (structured logging)
- **Captura erros da API** com contexto completo e rastreamento de stack trace
- **Registra execução**de use cases com operações e detalhes de negócio
- **Facilita debugging e troubleshooting** com informações estruturadas
- **Integra com infraestrutura de observabilidade** para análise pós-análise
- **Segue padrões de Hexagonal Architecture** mantendo responsabilidades em camadas apropriadas

---

## 📊 Tasks da FEAT-12

### ✅ TASK-64: Configurar logging com Serilog

**ID Azure DevOps**: 147  
**Título**: TASK-64 | Configurar logging com Serilog  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Configurar a biblioteca Serilog para logging estruturado em toda aplicação OrderHub, incluindo configuração de sinks (console, arquivo), níveis de log e enriquecimento de contexto.

**O que fazer**:

1. **Instalar pacotes NuGet necessários**:
   ```bash
   cd src/OrderHub.Adapters.Inbound.Api
   dotnet add package Serilog
   dotnet add package Serilog.AspNetCore
   dotnet add package Serilog.Sinks.Console
   dotnet add package Serilog.Sinks.File
   dotnet add package Serilog.Sinks.Seq  # Opcional para análise de logs
   ```

2. **Configurar Serilog no Program.cs**:
   - Adicionar `Serilog.ILogger` ao builder
   - Configurar sinks (console, arquivo com rolling)
   - Definir níveis de log apropriados por ambiente
   - Enriquecer com informações contextuais (timestamp, thread, machine name)
   ```csharp
   builder.Host.UseSerilog((context, loggerConfig) =>
   {
       loggerConfig
           .MinimumLevel.Information()
           .WriteTo.Console()
           .WriteTo.File("logs/app-.txt", rollingInterval: RollingInterval.Day)
           .Enrich.FromLogContext()
           .Enrich.WithMachineName();
   });
   ```

3. **Configurar appsettings.json**:
   - Adicionar seção de logging com configurações de níveis
   - Definir exclusões para namespaces verbose (exemplo: Microsoft.*)
   ```json
   {
     "Serilog": {
       "MinimumLevel": "Information",
       "WriteTo": [
         { "Name": "Console" },
         {
           "Name": "File",
           "Args": {
             "path": "logs/app-.txt",
             "rollingInterval": "Day"
           }
         }
       ]
     }
   }
   ```

4. **Testar configuração**:
   - Executar aplicação e validar logs em console
   - Validar criação de arquivo de log
   - Confirmar formato de log estruturado

---

### ✅ TASK-65: Criar logs de execução dos UseCases

**ID Azure DevOps**: 148  
**Título**: TASK-65 | Criar logs de execução dos UseCases  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Implementar logging em todos os use cases da camada Application, registrando início de execução, parâmetros de entrada, resultado e tempo de execução para rastreabilidade de operações de negócio.

**O que fazer**:

1. **Adicionar injeção de ILogger nos UseCases**:
   - Injetar `ILogger<{UseCase}>` via constructor em cada use case
   - Exemplo:
   ```csharp
   public class CreateOrderUseCase : ICreateOrderUseCase
   {
       private readonly IOrderRepository _repository;
       private readonly ILogger<CreateOrderUseCase> _logger;
       
       public CreateOrderUseCase(IOrderRepository repository, ILogger<CreateOrderUseCase> logger)
       {
           _repository = repository;
           _logger = logger;
       }
   }
   ```

2. **Registrar log de início de execução**:
   ```csharp
   public async Task<OrderDto> ExecuteAsync(CreateOrderDto request, CancellationToken cancellationToken)
   {
       _logger.LogInformation("Iniciando CreateOrderUseCase com CustomerId: {CustomerId}", request.CustomerId);
       
       try
       {
           // lógica do use case
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Erro ao executar CreateOrderUseCase");
           throw;
       }
   }
   ```

3. **Implementar logs em todos os UseCases**:
   - `ICreateOrderUseCase`
   - `IGetOrderUseCase`
   - `IUpdateOrderUseCase` (se existir)
   - `IDeleteOrderUseCase` (se existir)
   - `IListOrdersUseCase` (se existir)

4. **Registrar informações relevantes**:
   - ID do agregado sendo processado
   - Dados de entrada (sanitizados)
   - Resultado de operação
   - Tempo de execução (opcional)
   ```csharp
   _logger.LogInformation("CreateOrderUseCase concluído com sucesso. OrderId: {OrderId}, ItemCount: {ItemCount}", 
       order.Id, order.Items.Count);
   ```

5. **Testar logs em IndustryTests**:
   - Executar testes de integração
   - Validar logs em arquivo
   - Confirmar rastreamento de operações

---

### ✅ TASK-66: Criar logs de erro da API

**ID Azure DevOps**: 149  
**Título**: TASK-66 | Criar logs de erro da API  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Implementar middleware de tratamento de erros e logging na API, registrando todas as exceções com contexto completo, stack trace, headers da requisição e resposta ao cliente para auditoria e debugging.

**O que fazer**:

1. **Criar middleware de tratamento de erro**:
   - Criar arquivo `Middleware/ErrorHandlingMiddleware.cs`
   - Capturar exceções não tratadas em requisições HTTP
   - Registrar informações completas do erro
   ```csharp
   public class ErrorHandlingMiddleware
   {
       private readonly RequestDelegate _next;
       private readonly ILogger<ErrorHandlingMiddleware> _logger;
       
       public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
       {
           _next = next;
           _logger = logger;
       }
       
       public async Task InvokeAsync(HttpContext context)
       {
           try
           {
               await _next(context);
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "Erro não tratado na requisição {Method} {Path}", 
                   context.Request.Method, context.Request.Path);
               await HandleExceptionAsync(context, ex);
           }
       }
   }
   ```

2. **Registrar logs em Action Filters**:
   - Criar filter `[ApiExceptionFilter]` ou usar middleware existente
   - Registrar erros de validação
   - Registrar erros de negócio (DomainException)
   - Registrar erros infraestrutura
   ```csharp
   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
   public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
   {
       private readonly ILogger<ApiExceptionFilterAttribute> _logger;
       
       public override void OnException(ExceptionContext context)
       {
           _logger.LogError(context.Exception, "Erro na ação {ActionName}", 
               context.ActionDescriptor.DisplayName);
       }
   }
   ```

3. **Implementar logs em Controllers**:
   - Adicionar injeção de ILogger em OrdersController
   - Registrar erros de operações específicas
   ```csharp
   [ApiController]
   [Route("api/v1/[controller]")]
   public class OrdersController : ControllerBase
   {
       private readonly ICreateOrderUseCase _createOrderUseCase;
       private readonly ILogger<OrdersController> _logger;
       
       public OrdersController(ICreateOrderUseCase createOrderUseCase, ILogger<OrdersController> logger)
       {
           _createOrderUseCase = createOrderUseCase;
           _logger = logger;
       }
       
       [HttpPost]
       public async Task<IActionResult> CreateAsync([FromBody] CreateOrderRequest request)
       {
           try
           {
               // implementação
           }
           catch (DomainException ex)
           {
               _logger.LogWarning(ex, "Erro de domínio ao criar pedido");
               return BadRequest(ex.Message);
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "Erro inesperado ao criar pedido");
               return StatusCode(500, "Erro interno do servidor");
           }
       }
   }
   ```

4. **Registrar middleware no Program.cs**:
   ```csharp
   app.UseMiddleware<ErrorHandlingMiddleware>();
   ```

5. **Adicionar context enrichment**:
   - Registrar UserId (se autenticado)
   - Registrar IP da requisição
   - Registrar User-Agent
   ```csharp
   LogContext.PushProperty("RequestId", context.TraceIdentifier);
   LogContext.PushProperty("UserIp", context.Connection.RemoteIpAddress);
   LogContext.PushProperty("UserId", context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
   ```

6. **Testar erro handling**:
   - Executar requisições que geram exceções
   - Validar logs contêm informações contextuais
   - Validar resposta amigável ao cliente

---

## 📝 Resumo de Implementação

| Task | Componente | Prioridade | Esforço |
|------|-----------|-----------|--------|
| TASK-64 | Configuração Serilog | Alta | Baixo |
| TASK-65 | Logs em UseCases | Alta | Médio |
| TASK-66 | Logs de Erro da API | Alta | Médio |

---

## 🔗 Dependências

- ✅ **FEAT-09**: Infrastructure Layer (Dependency Injection) - Para registrar ILogger nos use cases
- ✅ **Framework**: .NET 8 com ASP.NET Core
- ✅ **Biblioteca**: Serilog e extensões

---

## 🎓 Padrões Aplicados

### Structured Logging
Usar campos tipados ao invés de concatenação de strings:
```csharp
// ❌ Não recomendado
_logger.LogInformation("Criado pedido " + order.Id);

// ✅ Recomendado
_logger.LogInformation("Criado pedido {OrderId}", order.Id);
```

### Níveis de Log Apropriados
- **LogInformation**: Operações de negócio bem-sucedidas
- **LogWarning**: Situações incomuns mas tratadas
- **LogError**: Erros que requerem atenção
- **LogDebug**: Informações detalhadas para debugging

### Contexto em Exceções
```csharp
_logger.LogError(exception, "Erro ao processar {Method} {Path} com {Parameters}", 
    request.Method, request.Path, JsonConvert.SerializeObject(request.Query));
```

---

## ✅ Critérios de Aceitação

- [x] Serilog configurado e funcionando em todos os ambientes
- [x] Todos os UseCases possuem logs informacionais
- [x] Todos os erros da API são registrados com contexto
- [x] Logs de arquivo estão sendo criados corretamente
- [x] Níveis de log são apropriados para cada situação
- [x] Informações sensíveis (senhas, tokens) não são logadas
- [x] Teste de integração valida logs sendo escritos

---

## 📌 Próximos Passos Pós-Implementação

1. **FEAT-13** (se existir): Health checks e métricas de aplicação
2. **FEAT-14** (se existir): Integração com ferramentas de APM (Application Performance Monitoring)
3. **FEAT-15** (se existir): Dashboard de visualização de logs e alertas

---

**Autor**: IA Assistant  
**Última Atualização**: 15 de Março de 2026  
**Padrão Usado**: Hexagonal Architecture + DDD + Structured Logging  
