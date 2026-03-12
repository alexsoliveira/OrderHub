# FEAT-02 | Plano de Ação - Domain Layer

**Data**: 12 de Março de 2026  
**Feature**: FEAT-02 | Domain Layer  
**Status**: Em Planejamento  
**Baseado em**: Azure DevOps Issue 68 (FEAT-02 | Domain Layer)  
**Total de Tasks**: 6  
**Sprint**: Sprint 1  
**Consultado via MCP**: ✅ GetWorkItem (ID 68) e GetRelatedWorkItems  

---

## 📋 Visão Geral

Implementação da camada de domínio (Domain Layer) seguindo os princípios de **Hexagonal Architecture** e **Domain-Driven Design (DDD)**.

Este documento detalha as **6 tasks reais** da FEAT-02 conforme definidas no Azure DevOps Issue 68.

---

## 🎯 Objetivo

Criar uma base sólida para o domínio de negócio da aplicação OrderHub, implementando:
- **Projeto Domain** estruturado e funcional
- **Agregados de Domínio** com comportamento encapsulado
- **Value Objects** imutáveis
- **Validações de Domínio** robustas
- **Regras de Negócio** enforced
- **Testes Unitários** com cobertura adequada

---

## 📊 Tasks da FEAT-02

### ✅ TASK-09: Criar projeto OrderHub.Domain

**ID Azure DevOps**: 92  
**Título**: TASK-09 | Criar projeto OrderHub.Domain  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Criar o projeto de biblioteca de classes .NET 8 que conterá toda a lógica de domínio da aplicação OrderHub.

**O que fazer**:
```bash
# Criar projeto class library
cd src/
dotnet new classlib -n OrderHub.Domain -f net8.0

# Adicionar ao sln
cd ..
dotnet sln add src/OrderHub.Domain/OrderHub.Domain.csproj

# Criar estrutura de pastas
mkdir src/OrderHub.Domain/Aggregates
mkdir src/OrderHub.Domain/ValueObjects
mkdir src/OrderHub.Domain/Entities
mkdir src/OrderHub.Domain/Exceptions
mkdir src/OrderHub.Domain/Ports
mkdir src/OrderHub.Domain/Events
mkdir src/OrderHub.Domain/Constants
```

**Checklist**:
- [ ] Projeto OrderHub.Domain criado com .NET 8
- [ ] Referenciado em OrderHub.sln
- [ ] Todas pastas criadas
- [ ] Arquivo .csproj configurado com `nullable` enabled
- [ ] Solução compila sem erros
- [ ] Commit: `feat: Create OrderHub.Domain project structure`

**Critério de Aceitação**:
- ✅ Projeto OrderHub.Domain criado e compilável
- ✅ Estrutura de pastas implementada
- ✅ Referenciado corretamente em OrderHub.sln
- ✅ Nenhum warning de compilação

---

### ✅ TASK-10: Criar entidade Order

**ID Azure DevOps**: 93  
**Título**: TASK-10 | Criar entidade Order  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Implementar a entidade `Order` como Aggregate Root, encapsulando lógica de pedidos.

**Estrutura Esperada**:
```csharp
namespace OrderHub.Domain.Aggregates.Order
{
    public class Order : AggregateRoot
    {
        public OrderId OrderId { get; private set; }
        public CustomerId CustomerId { get; private set; }
        public DateTime OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public List<OrderItem> Items { get; private set; } = new();
        
        // Factory method
        public static Order CreateOrder(OrderId orderId, CustomerId customerId)
        {
            // implementação
        }
        
        public void AddItem(OrderItem item)
        {
            // implementação com validações
        }
        
        public void RemoveItem(OrderItem item)
        {
            // implementação com validações
        }
        
        public bool CanAddItem(OrderItem item)
        {
            // validação de regras
        }
    }
}
```

**O que fazer**:
- [ ] Criar classe `Order` em `src/OrderHub.Domain/Aggregates/Order/Order.cs`
- [ ] Implementar propriedades: `OrderId`, `CustomerId`, `OrderDate`, `Status`, `Items`
- [ ] Criar `OrderId` como Value Object
- [ ] Criar `CustomerId` como Value Object
- [ ] Criar `OrderStatus` como enum ou Value Object
- [ ] Adicionar método `CreateOrder()` como factory method
- [ ] Implementar método `AddItem(OrderItem item)` com validações
- [ ] Implementar método `RemoveItem(OrderItem item)` com validações
- [ ] Implementar método `CanAddItem()` para verificar regras
- [ ] Implementar `IEquatable<Order>` para comparação
- [ ] Adicionar private setter para propriedades críticas
- [ ] Criar classe `OrderItem` em mesmo namespace
- [ ] Implementar validações de negócio
- [ ] Commit: `feat: Implement Order aggregate root`

**Critério de Aceitação**:
- ✅ Aggregate Root funciona corretamente
- ✅ Métodos implementados e validam regras
- ✅ Encapsulamento mantido (sem setters públicos inadequados)
- ✅ Compila sem warnings

---

### ✅ TASK-11: Criar ValueObject OrderAmount

**ID Azure DevOps**: 94  
**Título**: TASK-11 | Criar ValueObject OrderAmount  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Implementar Value Object `OrderAmount` para representar valores monetários com precisão.

**Estrutura Esperada**:
```csharp
namespace OrderHub.Domain.ValueObjects
{
    public class OrderAmount : IEquatable<OrderAmount>
    {
        public decimal Value { get; }
        public string Currency { get; }
        
        private OrderAmount(decimal value, string currency = "BRL")
        {
            Value = value;
            Currency = currency;
        }
        
        // Factory method
        public static OrderAmount Create(decimal value, string currency = "BRL")
        {
            // validação e criação
        }
    }
}
```

**O que fazer**:
- [ ] Criar classe `OrderAmount` em `src/OrderHub.Domain/ValueObjects/OrderAmount.cs`
- [ ] Adicionar propriedades: `Value` (decimal) e `Currency` (string)
- [ ] Implementar construtor privado
- [ ] Adicionar factory method `Create(decimal value, string currency = "BRL")`
- [ ] Adicionar validação: valor deve ser > 0
- [ ] Implementar `IEquatable<OrderAmount>` para comparação por valor
- [ ] Implementar operadores `==` e `!=`
- [ ] Adicionar método `ToString()` formatado: "R$ 100,00"
- [ ] Implementar `GetHashCode()` corretamente
- [ ] Fazer **completamente imutável** (readonly em tudo)
- [ ] Commit: `feat: Implement OrderAmount value object`

**Critério de Aceitação**:
- ✅ Value Object imutável e com negócio bem definido
- ✅ Comparação por valor funciona
- ✅ Validações impedem valores inválidos
- ✅ Sem qualquer variação de estado após criação

---

### ✅ TASK-12: Criar validações de domínio

**ID Azure DevOps**: 95  
**Título**: TASK-12 | Criar validações de domínio  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Implementar mecanismo robusto de validações específicas do domínio.

**Estrutura Esperada**:
```csharp
namespace OrderHub.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
    
    public class InvalidOrderException : DomainException
    {
        public InvalidOrderException(string message) : base(message) { }
    }
}

namespace OrderHub.Domain
{
    public static class DomainValidator
    {
        public static void ThrowIfNull(object value, string message)
        {
            if (value == null) throw new DomainException(message);
        }
        
        public static void ThrowIfNegativeOrZero(decimal value, string message)
        {
            if (value <= 0) throw new DomainException(message);
        }
    }
}
```

**O que fazer**:
- [ ] Criar classe `DomainException` em `src/OrderHub.Domain/Exceptions/DomainException.cs`
- [ ] Criar classe `InvalidOrderException` herdando `DomainException`
- [ ] Criar classe `InvalidOrderAmountException` herdando `DomainException`
- [ ] Criar classe `DomainValidator` com métodos estáticos:
  - `ThrowIfNull(object value, string message)`
  - `ThrowIfNegativeOrZero(decimal value, string message)`
  - `ThrowIfEmpty(string value, string message)`
  - `ThrowIfInvalidEmail(string email, string message)`
  - `ThrowIfNotInRange(decimal value, decimal min, decimal max, string message)`
- [ ] Integrar `DomainValidator` em construtores de Order e OrderAmount
- [ ] Adicionar mensagens de erro específicas **em português**
- [ ] Commit: `feat: Implement domain validation layer`

**Critério de Aceitação**:
- ✅ Validações rodam no momento da construção
- ✅ Mensagens em português e claras
- ✅ Impede estado inválido no domínio
- ✅ Utilizado em todas entidades/value objects

---

### ✅ TASK-13: Implementar regras de negócio básicas do pedido

**ID Azure DevOps**: 96  
**Título**: TASK-13 | Implementar regras de negócio básicas do pedido  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Codificar regras de negócio específicas para a entidade Order.

**Regras a Implementar**:

1. **Regra 1**: Não pode adicionar itens a pedido que já foi enviado (Status = Shipped)
   - Validar no método `AddItem()`
   - Lançar `InvalidOrderException` se Status == Shipped

2. **Regra 2**: Total do pedido deve ter no mínimo 1 item
   - Validar na criação de Order
   - Garantir que Order sempre tem pelo menos 1 item

3. **Regra 3**: Não pode remover último item (impedindo pedido vazio)
   - Validar no método `RemoveItem()`
   - Impedir remoção se Items.Count == 1

4. **Regra 4**: Remover o último item automaticamente marca pedido como Cancelled
   - Se `Items.Count == 1` e `RemoveItem()` é chamado
   - Status muda para `OrderStatus.Cancelled`

5. **Regra 5**: Não pode adicionar mais de 10 itens distintos no pedido
   - Validar no método `AddItem()`
   - Permitir até 10 itens diferentes

6. **Regra 6**: Implementar método `IsValidForStatus(OrderStatus newStatus)`
   - Valida transições de status permitidas
   - Novo → Pending → Processing → Shipped → Delivered
   - Cancelled pode ser atingido de qualquer estado

**O que fazer**:
- [ ] Implementar cada regra em método apropriado
- [ ] Adicionar método `CanAddItem()` retornando bool
- [ ] Adicionar método `CanRemoveItem()` retornando bool
- [ ] Adicionar método `CanTransitionTo(OrderStatus newStatus)` retornando bool
- [ ] Adicionar método `ChangeStatus(OrderStatus newStatus)` com validação
- [ ] Adicionar propriedade `DomainEvents` (List<IDomainEvent>) para futuro uso
- [ ] Codificar regras em português nas mensagens de erro
- [ ] Commit: `feat: Implement Order domain business rules`

**Critério de Aceitação**:
- ✅ Todas 6 regras implementadas e enforced
- ✅ Impossível violar regra via API pública
- ✅ Métodos com nomes descritivos e intencionais
- ✅ Mensagens de erro claras em português

---

### ✅ TASK-14: Criar testes unitários para entidade Order

**ID Azure DevOps**: 97  
**Título**: TASK-14 | Criar testes unitários para entidade Order  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  

**Descrição**:
Implementar suite de testes unitários para cobrir comportamento da entidade Order.

**O que fazer**:

1. **Criar Projeto de Testes**:
   ```bash
   cd tests/
   dotnet new xunit -n OrderHub.Domain.Tests
   cd ..
   dotnet sln add tests/OrderHub.Domain.Tests/OrderHub.Domain.Tests.csproj
   dotnet add tests/OrderHub.Domain.Tests/reference src/OrderHub.Domain/OrderHub.Domain.csproj
   ```

2. **Testes de Criação Order**:
   - [ ] `CreateOrder_WithValidData_ReturnsOrderEntity`
   - [ ] `CreateOrder_WithNullOrderId_ThrowsDomainException`
   - [ ] `CreateOrder_WithNullCustomerId_ThrowsDomainException`

3. **Testes de Adição de Itens**:
   - [ ] `AddItem_WithValidItem_ItemAddedSuccessfully`
   - [ ] `AddItem_ToShippedOrder_ThrowsInvalidOrderException`
   - [ ] `AddItem_Exceeding10Items_ThrowsInvalidOrderException`
   - [ ] `AddItem_WithValidItem_UpdatesOrderTotal`

4. **Testes de Remoção de Itens**:
   - [ ] `RemoveItem_WithValidItem_ItemRemovedSuccessfully`
   - [ ] `RemoveItem_LastItem_OrderCancelledAutomatically`
   - [ ] `CanRemoveItem_WithSingleItem_ReturnsFalse`
   - [ ] `RemoveItem_UpdatesOrderTotal`

5. **Testes de OrderAmount**:
   - [ ] `OrderAmount_Created_WithValidValue_Success`
   - [ ] `OrderAmount_Created_WithInvalidValue_ThrowsDomainException`
   - [ ] `OrderAmount_Comparison_ReturnsTrueForEqualValues`
   - [ ] `OrderAmount_GetHashCode_SameForEqualValues`
   - [ ] `OrderAmount_ToString_FormattedCorrectly`

6. **Testes de Validações**:
   - [ ] `CreateOrder_WithNullCustomerId_ThrowsException`
   - [ ] `CreateOrder_WithNullOrderAmount_ThrowsException`
   - [ ] `AddItem_WithNullItem_ThrowsException`

7. **Testes de Regras de Negócio**:
   - [ ] `CanTransitionTo_ValidTransition_ReturnsTrue`
   - [ ] `CanTransitionTo_InvalidTransition_ReturnsFalse`
   - [ ] `ChangeStatus_ToValidStatus_UpdatesStatus`

**Estrutura de Pastas**:
```
tests/OrderHub.Domain.Tests/
├── Aggregates/
│   └── OrderTests.cs
├── ValueObjects/
│   └── OrderAmountTests.cs
├── Validators/
│   └── DomainValidatorTests.cs
└── Fixtures/
    └── OrderTestFixture.cs
```

**Checklist**:
- [ ] Projeto OrderHub.Domain.Tests criado
- [ ] Todas classes de teste criadas
- [ ] Mínimo 30 testes implementados
- [ ] Cobertura >= 80% do código de domínio
- [ ] Fixtures para reutilização de dados
- [ ] Todos testes passando localmente
- [ ] Suite executa em < 2 segundos
- [ ] Commit: `test: Add unit tests for Order aggregate`

**Critério de Aceitação**:
- ✅ Cobertura mínima 80% do código de domínio
- ✅ Todos testes passam em CI/CD
- ✅ Azure Pipelines executa testes automaticamente
- ✅ Testes rápidos e determinísticos

---

## 📈 Sequência Recomendada de Execução

| # | Task | Objetivo | Estimativa | Dependências |
|---|------|----------|-----------|--------------|
| 1 | TASK-09 | Criar estrutura base | 30 min | Nenhuma |
| 2 | TASK-10 | Implementar Order | 1.5h | TASK-09 |
| 3 | TASK-11 | Criar OrderAmount VO | 1h | TASK-09 |
| 4 | TASK-12 | Validações Domínio | 1h | TASK-09 |
| 5 | TASK-13 | Regras Negócio | 1h | TASK-10, TASK-12 |
| 6 | TASK-14 | Testes Unitários | 1.5h | TASK-10, TASK-11 |
| | **TOTAL** | | **~6.5h** | |

---

## ✅ Checklist Geral FEAT-02

### Código
- [ ] Compila sem erros
- [ ] Nenhum warning de compilação
- [ ] Nomenclatura padrão .NET (PascalCase)
- [ ] Encapsulamento adequado (private setters)
- [ ] Value Objects imutáveis
- [ ] Validações em construtores

### Testes
- [ ] Cobertura >= 80%
- [ ] Todos testes PASSING
- [ ] Azure Pipelines executa com sucesso
- [ ] Testes rápidos (< 2s suite completa)

### Git & DevOps
- [ ] Commits seguem BRANCH_POLICY.md
- [ ] Feature branch criado: `feature/FEAT-02/domain-layer`
- [ ] Todos commits pusheados
- [ ] Build Pipeline PASS

### Documentação
- [ ] README.md atualizado
- [ ] Código comentado (métodos públicos)
- [ ] Diagrama UML atualizado (se aplicável)

---

## 🔗 Sincronização com Azure DevOps

Para cada task completada, atualizar o status no Azure DevOps:

```powershell
# Exemplo: Marcar TASK-09 como Done
mcp_azure-devops-_UpdateWorkItem -id 92 -state "Done"

# Marcação em lote (ao finalizar full FEAT-02)
for ($id in 92, 93, 94, 95, 96, 97) {
    mcp_azure-devops-_UpdateWorkItem -id $id -state "Done"
}

# Marcar Issue FEAT-02 como Done
mcp_azure-devops-_UpdateWorkItem -id 68 -state "Done"
```

---

## 📝 Notas Importantes

1. **Fonte de Verdade**: Este plano é **100% baseado em Azure DevOps Issue 68**
2. **Sem Especulação**: Cada task corresponde exatamente ao que foi definido no MCP
3. **Rastreabilidade**: Cada task tem ID Azure DevOps referenciado
4. **Commits Automatizados**: Marcar tasks Done após push bem-sucedido
5. **Review**: Antes de marcar Done, validar aceitação criteria completado
