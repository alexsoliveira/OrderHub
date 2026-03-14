# FEAT-08 | Plano de Ação - Repositório InMemory para Testes

**Data**: 13 de Março de 2026  
**Feature**: FEAT-08 | Repositório InMemory (para testes)  
**Status**: Em Planejamento  
**Baseado em**: Azure DevOps Issue 73 (FEAT-08)  
**Total de Tasks**: 4  
**Sprint**: Sprint 1  
**Consultado via MCP**: ✅ GetWorkItem (ID 73) e GetRelatedWorkItems  

---

## 📋 Visão Geral

Implementação de um repositório em memória que implementa a interface `IOrderRepository`, permitindo testes unitários e de integração da camada de aplicação **sem depender de banco de dados externo**.

Este documento detalha as **4 tasks reais** da FEAT-08 conforme definidas no Azure DevOps Issue 73, seguindo o padrão **Hexagonal Architecture** e **Dependency Inversion Principle**.

---

## 🎯 Objetivo

Criar uma implementação alternativa e leve de `IOrderRepository` que:
- Armazena dados em memória (Dictionary)
- Implementa o mesmo contrato da versão EF Core
- Funciona sem dependência externa
- Permite testes isolados e rápidos
- Mantém compatibilidade com Use Cases existentes

---

## 📊 Tasks da FEAT-08

### ✅ TASK-46: Criar InMemoryOrderRepository

**ID Azure DevOps**: 129  
**Título**: TASK-46 | Criar InMemoryOrderRepository  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Implementar a classe `InMemoryOrderRepository` que funciona como uma implementação alternativa de `IOrderRepository`, armazenando dados em memória através de um Dictionary.

**O que fazer**:
```bash
# Estrutura esperada
tests/OrderHub.Application.Tests/
├── Fixtures/
│   └── InMemoryOrderRepository.cs
```

**Implementação Esperada**:
```csharp
namespace OrderHub.Application.Tests.Fixtures
{
    public class InMemoryOrderRepository : IOrderRepository
    {
        private readonly Dictionary<string, Order> _orders = new();
        private readonly OrderMapper _mapper = new();

        // Implementar todos os métodos de IOrderRepository
        public async Task<OrderResponse?> GetByIdAsync(string orderId, CancellationToken cancellationToken = default)
        public async Task<List<OrderResponse>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
        public async Task SaveAsync(Order order, CancellationToken cancellationToken = default)
        public async Task DeleteAsync(string orderId, CancellationToken cancellationToken = default)
        public async Task<bool> ExistsAsync(string orderId, CancellationToken cancellationToken = default)
        
        // Métodos auxiliares
        public void Clear() { }
        public int Count { get; }
    }
}
```

**O que fazer**:
- [ ] Criar diretório `tests/OrderHub.Application.Tests/Fixtures/`
- [ ] Criar classe `InMemoryOrderRepository.cs`
- [ ] Implementar `IOrderRepository` completamente
- [ ] Usar `Dictionary<string, Order>` como storage
- [ ] Injetar `OrderMapper` para mapeamento Order → OrderResponse
- [ ] Implementar `GetByIdAsync()` com lookup por OrderId
- [ ] Implementar `GetByCustomerIdAsync()` com filtro por CustomerId
- [ ] Implementar `SaveAsync()` para adicionar/atualizar
- [ ] Implementar `DeleteAsync()` para remover
- [ ] Implementar `ExistsAsync()` para verificar existência
- [ ] Adicionar `Clear()` para resetar estado (útil em testes)
- [ ] Adicionar propriedade `Count` para contar itens
- [ ] Tratar strings null/empty adequadamente
- [ ] Suportar CancellationToken (mesmo que não seja usado)
- [ ] Adicionar documentação XML
- [ ] Compilar sem erros ou warnings
- [ ] Commit: `feat: Implement InMemoryOrderRepository for testing`

**Checklist**:
- [ ] Classe implementa IOrderRepository
- [ ] Todos os 5 métodos implementados
- [ ] Métodos auxiliares funcionais
- [ ] Dictionary storage operacional
- [ ] Mappers integrando corretamente
- [ ] Solução compila sem erros
- [ ] Sem warnings de compilação

**Critério de Aceitação**:
- ✅ InMemoryOrderRepository compila e implementa IOrderRepository
- ✅ Armazenamento em memória funciona
- ✅ Métodos CRUD funcionam corretamente
- ✅ Integración com OrderMapper validada
- ✅ Nenhum warning de compilação

---

### ✅ TASK-47: Implementar armazenamento em memória

**ID Azure DevOps**: 130  
**Título**: TASK-47 | Implementar armazenamento em memória  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Implementar a lógica de armazenamento em memória dentro do `InMemoryOrderRepository`, incluindo conversões de ValueObjects e mapeamento de entidades.

**O que fazer**:

**Armazenamento Principal**:
- [ ] Dictionary<string, Order> como storage
- [ ] Chave: OrderId.Value.ToString() (Guid formatado)
- [ ] Valor: Entidade Order completa

**Conversões de ValueObjects**:
- [ ] Conversão de OrderId para chave string
- [ ] Conversão de CustomerId para filtro de busca
- [ ] Conversão de ProductId em OrderItems
- [ ] Conversão de OrderAmount para decimal

**Mappers**:
- [ ] Injetar OrderMapper via construtor
- [ ] Mapear Order → OrderResponse em GetById
- [ ] Mapear Order → OrderResponse em GetByCustomerId
- [ ] Manter ValueObjects intactos durante mapeamento

**Operações CRUD**:

GetByIdAsync:
- [ ] Receber orderId como string
- [ ] Buscar no Dictionary (O(1) lookup)
- [ ] Converter para OrderResponse
- [ ] Retornar null se não encontrado
- [ ] Tratar string null/empty

GetByCustomerIdAsync:
- [ ] Receber customerId como string
- [ ] Filtrar Dictionary por CustomerId.Value.ToString()
- [ ] Mapear resultados para OrderResponse
- [ ] Retornar lista (vazia se nenhum encontrado)
- [ ] Tratar string null/empty

SaveAsync:
- [ ] Receber Order do agregado
- [ ] Validar se order é null (ArgumentNullException)
- [ ] Extrair OrderId.Value.ToString() como chave
- [ ] Armazenar no Dictionary (add ou update)
- [ ] Retornar Task.CompletedTask

DeleteAsync:
- [ ] Receber orderId como string
- [ ] Buscar no Dictionary
- [ ] Remover se encontrado
- [ ] Tratar strings null/empty

ExistsAsync:
- [ ] Receber orderId como string
- [ ] Verificar se chave existe no Dictionary
- [ ] Retornar bool
- [ ] Tratar strings null/empty

**Tratamento de Casos Especiais**:
- [ ] Null reference checks
- [ ] Empty string checks
- [ ] Entidades sem itens
- [ ] ValueObjects com valores zerados
- [ ] CancellationToken passar sem uso (consistent API)

**Documentação**:
- [ ] XML comments em todos métodos
- [ ] Exemplos de uso
- [ ] Descrição de parâmetros
- [ ] Descrição de retorno

**Checklist**:
- [ ] Dictionary storage implementado
- [ ] Conversões ValueObjects corretas
- [ ] Mapeamento Order → OrderResponse correto
- [ ] Todas operações CRUD funcionam
- [ ] Casos especiais tratados
- [ ] Compilação sem erros

**Critério de Aceitação**:
- ✅ Armazenamento persiste dados corretamente
- ✅ ValueObjects mapeados sem erros
- ✅ Operações CRUD funcionam end-to-end
- ✅ Nenhuma perda de dados entre operações
- ✅ Sem exceções não tratadas

---

### ✅ TASK-48: Integrar repositório InMemory nos testes

**ID Azure DevOps**: 131  
**Título**: TASK-48 | Integrar repositório InMemory nos testes  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Criar testes de integração que utilizam o `InMemoryOrderRepository` real em vez de mocks, validando que os Use Cases funcionam corretamente com persistência em memória.

**O que fazer**:

**Arquivo de Testes**:
```bash
tests/OrderHub.Application.Tests/UseCases/Orders/
└── CreateOrderServiceIntegrationTests.cs
```

**Setup do Teste**:
```csharp
public class CreateOrderServiceIntegrationTests
{
    private readonly InMemoryOrderRepository _repository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<INotificationPort> _mockNotification;
    private readonly CreateOrderService _service;

    public CreateOrderServiceIntegrationTests()
    {
        _repository = new InMemoryOrderRepository();  // Real, não mock
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockNotification = new Mock<INotificationPort>();
        
        // IUnitOfWork.Orders retorna repositório real
        _mockUnitOfWork.Setup(x => x.Orders).Returns(_repository);
        
        // Setup de mocks para transações
        _mockUnitOfWork
            .Setup(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        // ... mais setups
    }
}
```

**Testes de Integração**:
- [ ] `CreateOrderService_ShouldPersistOrderInRepository()` 
  - Cria order via CreateOrderService
  - Verifica se foi persistida no repositório
  - Testa GetByIdAsync()
  
- [ ] `CreateOrderService_MultipleOrders_ShouldRetrieveByCustomerId()`
  - Cria múltiplos pedidos do mesmo cliente
  - Verifica se podem ser recuperados via GetByCustomerIdAsync
  - Valida integridade dos dados

- [ ] `CreateOrderService_ShouldVerifyOrderExists()`
  - Cria order
  - Testa ExistsAsync()
  - Valida retorno boolean

- [ ] `CreateOrderService_RepositoryCanBeCleared()`
  - Cria order
  - Limpa repositório via Clear()
  - Verifica reset do estado

**Características**:
- [ ] Usar repositório real em memória
- [ ] Mock apenas IUnitOfWork e INotificationPort
- [ ] Validar persistência end-to-end
- [ ] Validar mapeamento Order → OrderResponse
- [ ] Validar cada método do repositório
- [ ] Testes isolados (independentes um do outro)
- [ ] Setup/Teardown apropriado

**Cobertura**:
- [ ] CreateOrderService com repositório real
- [ ] Persistência de dados
- [ ] Recuperação de dados
- [ ] Múltiplos pedidos
- [ ] Transações (Begin/Commit/Rollback)
- [ ] Notificações (mocked)

**Checklist**:
- [ ] Testes compilam
- [ ] Testes passam
- [ ] 4 testes implementados
- [ ] Cobertura de CreateOrderService adequada
- [ ] Sem testes redundantes com mocks anteriores
- [ ] Arquivo bem organizado

**Critério de Aceitação**:
- ✅ Testes de integração compilam
- ✅ CreateOrderService com repositório real validado
- ✅ Persistência em memória funciona
- ✅ Mapeamentos corretos
- ✅ Todos os testes passam

---

### ✅ TASK-49: Validar funcionamento do UseCase sem banco

**ID Azure DevOps**: 132  
**Título**: TASK-49 | Validar funcionamento do UseCase sem banco  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Implementar testes de validação completa dos Use Cases (Create, Get, Update, Cancel) utilizando o `InMemoryOrderRepository`, confirmando que toda a lógica de aplicação funciona sem dependência de banco de dados externo.

**O que fazer**:

**Arquivo de Testes**:
```bash
tests/OrderHub.Application.Tests/UseCases/Orders/
└── OrderUseCasesValidationTests.cs
```

**Validações de Workflows Completos**:
- [ ] Teste: `CompleteOrderWorkflow_CreateAndRetrieveOrder_ShouldWork()`
  - Create Order → Get Order
  - Valida fluxo completo
  - Verifica todas as propriedades

- [ ] Teste: `CompleteOrderWorkflow_CreateAndUpdateOrder_ShouldWork()`
  - Create Order → Update Order → Get Order
  - Valida mutabilidade
  - Verifica quantidade de itens

- [ ] Teste: `CompleteOrderWorkflow_CreateAndCancelOrder_ShouldWork()`
  - Create Order → Cancel Order → Get Order
  - Valida transição de estado
  - Verifica que order foi cancelada

**Validações de Repositório**:
- [ ] Teste: `Repository_ShouldPersistMultipleOrdersAndRetrieveByCustomer()`
  - Cria múltiplos pedidos
  - Recupera por cliente
  - Valida integridade de lista

- [ ] Teste: `Repository_ShouldReturnNullForNonExistentOrder()`
  - Edge case: order não existe
  - Verifica retorno null
  - Valida tratamento de erro

- [ ] Teste: `Repository_ShouldReturnEmptyListForNonExistentCustomer()`
  - Edge case: cliente sem pedidos
  - Verifica retorno lista vazia
  - Valida tratamento de caso vazio

- [ ] Teste: `Repository_ClearFunctionality_ShouldResetInMemoryStorage()`
  - Cria order
  - Limpa repositório
  - Valida reset de estado

**Validação Final**:
- [ ] Teste: `UseCase_ShouldWorkWithoutExternalDependencies()`
  - Confirma que Use Cases funcionam completamente sem BD
  - Testa fluxo realista completo
  - Valida cálculos de totais

**Características**:
- [ ] Todos os Use Cases testados: Create, Get, Update, Cancel
- [ ] Workflows completos end-to-end
- [ ] Edge cases cobertos
- [ ] Sem dependência de SQL Server
- [ ] Mocks apenas para serviços externos (Notification)
- [ ] Repositório real em memória
- [ ] Validações de estado e dados

**Cobertura Esperada**:
- ✅ CreateOrderService
- ✅ GetOrderService
- ✅ UpdateOrderService
- ✅ CancelOrderService
- ✅ InMemoryOrderRepository CRUD
- ✅ OrderMapper
- ✅ Transações
- ✅ Notificações (mocked)

**Checklist**:
- [ ] 7+ testes implementados
- [ ] Todos os testes compilam
- [ ] Todos os testes passam
- [ ] Workflows completos validados
- [ ] Edge cases cobertos
- [ ] Sem BD externo
- [ ] Fast feedback (< 100ms total)

**Critério de Aceitação**:
- ✅ Todos os Use Cases validados sem BD
- ✅ Workflows completos funcionam
- ✅ Edge cases tratados corretamente
- ✅ Testes rápidos (sem I/O externo)
- ✅ Cobertura de casos principais atingida
- ✅ Nenhuma dependência de SQL Server

---

## 📦 Estrutura Esperada de Arquivos

Após conclusão das 4 tasks:

```
src/OrderHub.Application/
├── Ports/
│   └── IOrderRepository.cs  (já exists)
├── Mappers/
│   └── OrderMapper.cs       (já exists)
└── UseCases/Orders/
    ├── CreateOrderService.cs
    ├── GetOrderService.cs
    ├── UpdateOrderService.cs
    └── CancelOrderService.cs

tests/OrderHub.Application.Tests/
├── Fixtures/
│   └── InMemoryOrderRepository.cs  (TASK-46/47)
└── UseCases/Orders/
    ├── CreateOrderServiceIntegrationTests.cs  (TASK-48)
    └── OrderUseCasesValidationTests.cs         (TASK-49)
```

---

## 🔄 Relacionamento com FEAT-07

**FEAT-07** implementa `IOrderRepository` com EF Core e SQL Server  
**FEAT-08** implementa `IOrderRepository` com Dictionary e memória

Ambas implementam o **mesmo contrato**, permitindo:
- ✅ Injetar qual implementação usar em runtime
- ✅ Usar EF Core em produção
- ✅ Usar InMemory em testes
- ✅ Sem modificar código de Use Cases
- ✅ Coexistem sem conflitos

---

## ✅ Validações a Realizar

### Durante Implementação
- [ ] Cada método compila sem errors
- [ ] Nenhum warning de compiler
- [ ] Código segue padrões C# e .NET
- [ ] XML comments presentes
- [ ] Nomes de variáveis descritivos

### Após Conclusão
- [ ] Solução compila: `dotnet build`
- [ ] Testes rodam: `dotnet test`
- [ ] Build status: ✅ Success
- [ ] Tempo build: < 3 segundos
- [ ] Testes: ✅ Todos passando

### Qualidade
- [ ] Sem compiler warnings
- [ ] Respeitam SOLID principles
- [ ] Seguem Hexagonal Architecture
- [ ] Integram com DI container
- [ ] Documentação XML completa

---

## 📊 Estimativas

| Task | Estimativa | Finalizado |
|------|-----------|-----------|
| TASK-46 | 30 min | - |
| TASK-47 | 30 min | - |
| TASK-48 | 30 min | - |
| TASK-49 | 30 min | - |
| **Total** | **2 horas** | - |

---

## 🎯 Próximos Passos

1. Iniciar com TASK-46: Criar InMemoryOrderRepository
2. Validar armazenamento em memória (TASK-47)
3. Integrar em testes (TASK-48)
4. Validar workflows completos (TASK-49)
5. Marcar todas as tasks como Done
6. Marcar FEAT-08 como Done
7. Marcar EPIC-05 como Done (ambas Features concluídas)

---

**Desenvolvido por**: AI Assistant  
**Data de Criação**: 2026-03-13  
**Status**: Pronto para Implementação

