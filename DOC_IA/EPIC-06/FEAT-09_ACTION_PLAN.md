# FEAT-09 | Plano de Ação - Configuração da Infraestrutura

**Data**: 14 de Março de 2026  
**Feature**: FEAT-09 | Configuração da Infraestrutura  
**Status**: Em Planejamento  
**Baseado em**: Azure DevOps Issue 75 (FEAT-09 | Configuração da Infraestrutura)  
**Total de Tasks**: 5  
**Sprint**: Sprint 2  
**Consultado via MCP**: ✅ GetWorkItem (ID 75) e GetChildWorkItems  

---

## 📋 Visão Geral

Implementação da infraestrutura de **Dependency Injection** e configuração de projeto seguindo os princípios de **Hexagonal Architecture**.

Este documento detalha as **5 tasks reais** da FEAT-09 conforme definidas no Azure DevOps Issue 75, organizadas para criar o novo projeto `OrderHub.Infrastructure` responsável pela orquestração de dependências.

---

## 🎯 Objetivo

Criar uma camada de infraestrutura (Infrastructure Layer) que:
- **Centraliza Dependency Injection** para toda aplicação
- **Registra Use Cases** no container de DI
- **Registra Repositories** no container de DI
- **Configura carregamento de configurações** (appsettings)
- **Orquestra serviços** da aplicação de forma desacoplada

---

## 📊 Tasks da FEAT-09

### ✅ TASK-50: Criar projeto OrderHub.Infrastructure

**ID Azure DevOps**: 133  
**Título**: TASK-50 | Criar projeto OrderHub.Infrastructure  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Criar o projeto de biblioteca de classes que conterá a configuração centralizada de Dependency Injection e setup de infraestrutura da aplicação.

**O que fazer**:

1. **Criar projeto class library**:
   ```bash
   cd src/
   dotnet new classlib -n OrderHub.Infrastructure -f net8.0
   cd ..
   dotnet sln add src/OrderHub.Infrastructure/OrderHub.Infrastructure.csproj
   ```

2. **Adicionar estrutura de pastas**:
   ```bash
   mkdir src/OrderHub.Infrastructure/DependencyInjection
   mkdir src/OrderHub.Infrastructure/Configuration
   mkdir src/OrderHub.Infrastructure/Extensions
   ```

3. **Configurar referências de projeto**:
   ```bash
   cd src/OrderHub.Infrastructure
   dotnet add reference ../OrderHub.Domain/OrderHub.Domain.csproj
   dotnet add reference ../OrderHub.Application/OrderHub.Application.csproj
   dotnet add reference ../OrderHub.Adapters.Outbound.Persistence/OrderHub.Adapters.Outbound.Persistence.csproj
   ```

4. **Adicionar packages NuGet se necessário**:
   ```bash
   dotnet add package Microsoft.Extensions.DependencyInjection
   dotnet add package Microsoft.Extensions.Configuration
   ```

**Checklist**:
- [ ] Projeto OrderHub.Infrastructure criado com .NET 8
- [ ] Referenciado em OrderHub.sln
- [ ] Todas pastas criadas (DependencyInjection, Configuration, Extensions)
- [ ] Referências de projeto configuradas corretamente
- [ ] Arquivo .csproj configurado com `nullable` enabled
- [ ] Solução compila sem erros
- [ ] Commit: `feat: Create OrderHub.Infrastructure project structure`

**Critério de Aceitação**:
- ✅ Projeto OrderHub.Infrastructure criado e compilável
- ✅ Estrutura de pastas implementada
- ✅ Referenciado corretamente em OrderHub.sln
- ✅ Referências para Domain, Application e Persistence corretas
- ✅ Nenhum warning de compilação

---

### ✅ TASK-51: Implementar configuração de Dependency Injection

**ID Azure DevOps**: 134  
**Título**: TASK-51 | Implementar configuração de Dependency Injection  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Implementar a configuração centralizada de Dependency Injection como extension method para facilitar registro de serviços.

**Estrutura Esperada**:
```csharp
namespace OrderHub.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Registrar serviços de aplicação
            services.AddApplicationServices();
            
            // Registrar repositórios
            services.AddRepositories();
            
            // Configurar banco de dados
            services.AddPersistence(configuration);
            
            return services;
        }
    }
}
```

**O que fazer**:
- [ ] Criar classe `ServiceCollectionExtensions` em `DependencyInjection/ServiceCollectionExtensions.cs`
- [ ] Implementar método `AddInfrastructure()` como extension de `IServiceCollection`
- [ ] Método deve aceitar `IConfiguration` para carregar configurações
- [ ] Definir sub-methods para cada categoria (Application, Repositories, Persistence)
- [ ] Documentar com XML comments o propósito de cada método
- [ ] Garantir que seja responsabilidade dele orquestrar AddApplicationServices(), AddRepositories() e AddPersistence()
- [ ] Arquivo em `src/OrderHub.Infrastructure/DependencyInjection/ServiceCollectionExtensions.cs`
- [ ] Commit: `feat: Implement central DependencyInjection configuration`

**Critério de Aceitação**:
- ✅ Extension method criado e funcional
- ✅ Compila sem erros ou warnings
- ✅ Pode ser chamado de Program.cs facilmente
- ✅ Estrutura clara e bem organizada

---

### ✅ TASK-52: Registrar UseCases no container

**ID Azure DevOps**: 135  
**Título**: TASK-52 | Registrar UseCases no container  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Criar método para registrar todos os Use Cases (Application Layer) no container de Dependency Injection.

**Estrutura Esperada**:
```csharp
namespace OrderHub.Infrastructure.DependencyInjection
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registrar Use Cases
            services.AddScoped<ICreateOrderUseCase, CreateOrderUseCase>();
            services.AddScoped<IGetOrderUseCase, GetOrderUseCase>();
            services.AddScoped<IUpdateOrderUseCase, UpdateOrderUseCase>();
            services.AddScoped<IDeleteOrderUseCase, DeleteOrderUseCase>();
            // ... outros use cases
            
            return services;
        }
    }
}
```

**O que fazer**:
- [ ] Criar classe `ApplicationServiceExtensions` em `DependencyInjection/ApplicationServiceExtensions.cs`
- [ ] Implementar método `AddApplicationServices()` como extension de `IServiceCollection`
- [ ] Registrar todos Use Cases com `AddScoped<IInterface, Implementation>()`
- [ ] Incluir todos os Use Cases de criar, obter, atualizar e deletar ordem
- [ ] Documentar com XML comments
- [ ] Usar pattern de `IServiceCollection.AddScoped<T, TImpl>()`
- [ ] Arquivo em `src/OrderHub.Infrastructure/DependencyInjection/ApplicationServiceExtensions.cs`
- [ ] Commit: `feat: Register all application use cases in DI container`

**Critério de Aceitação**:
- ✅ Todos Use Cases registrados no container
- ✅ Usa pattern Scoped para cada requisição ter instância
- ✅ Compila sem erros
- ✅ Interfaces e implementações bate

---

### ✅ TASK-53: Registrar Repositories no container

**ID Azure DevOps**: 136  
**Título**: TASK-53 | Registrar Repositories no container  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Criar método para registrar todos os Repositories (Output Ports) no container de Dependency Injection.

**Estrutura Esperada**:
```csharp
namespace OrderHub.Infrastructure.DependencyInjection
{
    public static class RepositoryServiceExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Registrar Repositories
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            // ... outros repositories
            
            return services;
        }
    }
}
```

**O que fazer**:
- [ ] Criar classe `RepositoryServiceExtensions` em `DependencyInjection/RepositoryServiceExtensions.cs`
- [ ] Implementar método `AddRepositories()` como extension de `IServiceCollection`
- [ ] Registrar todos Repositories com `AddScoped<IInterface, Implementation>()`
- [ ] Incluir repositories da camada Persistence (OrderRepository, OrderItemRepository, etc)
- [ ] Documentar com XML comments
- [ ] Usar pattern de `IServiceCollection.AddScoped<T, TImpl>()`
- [ ] Arquivo em `src/OrderHub.Infrastructure/DependencyInjection/RepositoryServiceExtensions.cs`
- [ ] Commit: `feat: Register all repositories in DI container`

**Critério de Aceitação**:
- ✅ Todos Repositories registrados no container
- ✅ Usa pattern Scoped para cada requisição
- ✅ Compila sem erros
- ✅ Interfaces vêm de Domain.Ports, implementações de Persistence

---

### ✅ TASK-54: Configurar carregamento de configurações

**ID Azure DevOps**: 137  
**Título**: TASK-54 | Configurar carregamento de configurações  
**Status**: To Do  
**Sprint**: Sprint 2  
**Prioridade**: 2  

**Descrição**:
Implementar método para configurar o carregamento de configurações (appsettings.json) e suas variações por ambiente.

**Estrutura Esperada**:
```csharp
namespace OrderHub.Infrastructure.Configuration
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddCustomConfiguration(
            this IConfigurationBuilder builder, 
            string basePath)
        {
            return builder
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", 
                    optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }
}
```

**O que fazer**:
- [ ] Criar classe `ConfigurationExtensions` em `Configuration/ConfigurationExtensions.cs`
- [ ] Implementar método `AddCustomConfiguration()` como extension de `IConfigurationBuilder`
- [ ] Carregar `appsettings.json` como obrigatório
- [ ] Carregar `appsettings.{Environment}.json` como opcional
- [ ] Suportar variáveis de ambiente
- [ ] Documentar com XML comments
- [ ] Permitir reloadOnChange em desenvolvimento
- [ ] Arquivo em `src/OrderHub.Infrastructure/Configuration/ConfigurationExtensions.cs`
- [ ] Commit: `feat: Implement configuration loading extensions`

**Critério de Aceitação**:
- ✅ Carrega appsettings.json
- ✅ Carrega appsettings por ambiente
- ✅ Suporta variáveis de ambiente
- ✅ Compila sem erros
- ✅ Usável em Program.cs facilmente

---

## 🔗 Relacionamentos

**Epic **EPIC-06**: [Configuração de Dependency Injection](../EPIC-06/) (ID 74)  
**Feature**: FEAT-09 | Configuração da Infraestrutura  
**Parent**: [EPIC-06 | Configuração de Dependency Injection](../EPIC-06/)

---

## 📅 Timeline Estimada

| Milestone | Duração Estimada | Descrição |
|-----------|-----------------|-----------|
| TASK-50 | 0.5h | Criar projeto e estrutura |
| TASK-51 | 1.5h | Implementar DI centralizado |
| TASK-52 | 2h | Registrar Use Cases |
| TASK-53 | 1.5h | Registrar Repositories |
| TASK-54 | 1h | Configurações de arquivo |
| **TOTAL** | **6.5 horas** | Tempo total estimado |

---

## 🎓 Padrões e Conceitos Aplicados

### Dependency Injection Pattern
- **Extension Methods**: Facilitam composição de serviços
- **Factory Pattern**: Cada serviço é instanciado adequadamente
- **Inversion of Control**: Serviços desacoplados do cliente

### Configuration Pattern
- **Environment-specific configs**: Diferente setup por ambiente
- **Centralized configuration**: Todas configs em um lugar
- **Secret Management Ready**: Suporta user secrets em dev

### Hexagonal Architecture
- **Adapter Independence**: Infraestrutura é um adapter
- **Loose Coupling**: Interfaces separam configuração de uso
- **Easy Testing**: DI facilita mock em testes

---

## ✅ Checklist Final de Implementação

- [ ] Pasta EPIC-06 criada em DOC_IA
- [ ] Arquivo FEAT-09_ACTION_PLAN.md criado
- [ ] Projeto OrderHub.Infrastructure criado
- [ ] All extension methods implementados
- [ ] All Use Cases registrados
- [ ] All Repositories registrados
- [ ] Configuration extensions implementadas
- [ ] Program.cs atualizado para usar nova infraestrutura
- [ ] Solução compila sem erros
- [ ] Testes de DI funcionam
- [ ] Merge em `develop` completado

---

**Próximas etapas**: Após conclusão da FEAT-09, proceder com FEAT-10 (Testes de Integração) ou outras features do EPIC-06.
