# FEAT-04 | Plano de Ação - Application Layer (Complementação)

**Data**: 12 de Março de 2026  
**Feature**: FEAT-04 | Application Layer  
**Status**: Em Planejamento  
**Baseado em**: Azure DevOps Issue 64 (FEAT-04)  
**Total de Tasks**: 4  
**Sprint**: Sprint 1  
**Tempo Total Estimado**: 6-8 horas  
**Consultado via MCP**: ✅ GetWorkItem (IDs 106, 107, 108, 109)  

---

## 📋 Visão Geral

Continuação da implementação da camada de aplicação (Application Layer):
- **Use Cases/Application Services**: Interfaces (Ports) de casos de uso bem definidos
- **Contratos de Requisição/Resposta**: DTOs consolidados e validados
- **Documentação técnica**: Especificação completa de todos os casos de uso

Esta fase consolida a lógica de orquestração, assegurando que todos os casos de uso sejam bem definidos, documentados e prontos para implementação de infraestrutura.

---

## 🎯 Objetivo

Completar a camada de aplicação com:
- **Define Use Cases** como interfaces (Ports) claras e bem contratadas
- **Implementa/Valida Contratos** (DTOs) padronizados e mapeadores
- **Documenta Casos de Uso** com especificações técnicas detalhadas
- **Garante Consistência** entre serialização (DTOs) e model de domínio

---

## 📊 Tasks da FEAT-04

### TASK-23: Criar interface ICreateOrderUseCase

**ID Azure DevOps**: 106  
**Título**: TASK-23 | Criar interface ICreateOrderUseCase  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1.5h  

**Descrição**:
Criar a interface (Port) para o caso de uso de criação de pedidos, definindo o contrato que a aplicação espera para executar a lógica de criar uma nova ordem.

**O que fazer**:

1. **Criar interface ICreateOrderUseCase**:
   - Arquivo: `src/OrderHub.Application/UseCases/ICreateOrderUseCase.cs`
   - Definir assinatura:
     ```csharp
     public interface ICreateOrderUseCase
     {
         Task<CreateOrderResponse> ExecuteAsync(CreateOrderRequest request, CancellationToken cancellation = default);
     }
     ```
   - Retornar `CreateOrderResponse` com campos:
     - OrderId (string/Guid)
     - CustomerId (string)
     - Status (enum)
     - CreatedAt (DateTime)
     - Items (List<OrderItemResponse>)
     - TotalAmount (decimal)

2. **Refatorar CreateOrderService**:
   - Implementar interface `ICreateOrderUseCase`
   - Garantir signature do método bate com inteface
   - Adicionar XML comments

3. **Integração**:
   - Assegurar consistência com Domain Model
   - Documentar exceções: `ValidationException`, `ArgumentNullException`

**Checklist**:
- [ ] Interface `ICreateOrderUseCase` criada
- [ ] Método `ExecuteAsync` com tipagem correta
- [ ] `CreateOrderResponse` DTO validado
- [ ] Service `CreateOrderService` implementa interface
- [ ] Build sem erros
- [ ] Commit: `feat: Create ICreateOrderUseCase interface`

**Critério de Aceitação**:
- ✅ Interface bem definida
- ✅ Contrato com tipos específicos
- ✅ Build sem erros

---

### TASK-24: Criar interface IGetOrderUseCase

**ID Azure DevOps**: 107  
**Título**: TASK-24 | Criar interface IGetOrderUseCase  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1.5h  

**Descrição**:
Criar a interface (Port) para recuperação de pedidos, definindo o contrato para buscar ordens por ID ou por cliente.

**O que fazer**:

1. **Criar interface IGetOrderUseCase**:
   - Arquivo: `src/OrderHub.Application/UseCases/IGetOrderUseCase.cs`
   - Definir métodos:
     ```csharp
     public interface IGetOrderUseCase
     {
         Task<OrderResponse?> GetByIdAsync(string orderId, CancellationToken cancellation = default);
         Task<IEnumerable<OrderResponse>> GetByCustomerIdAsync(string customerId, CancellationToken cancellation = default);
     }
     ```

2. **Refatorar GetOrderService**:
   - Implementar interface `IGetOrderUseCase`
   - Validar métodos existentes batem com interface
   - Adicionar logging

3. **Validações**:
   - Método ById retorna null se não encontrado
   - Método ByCustomer retorna lista vazia se nenhum pedido
   - Sem lançar exceções

**Checklist**:
- [ ] Interface `IGetOrderUseCase` criada
- [ ] Dois métodos definidos
- [ ] Service implementa interface
- [ ] Build sem erros
- [ ] Commit: `feat: Create IGetOrderUseCase interface`

**Critério de Aceitação**:
- ✅ Interface bem estruturada
- ✅ Contratos tipados corretamente
- ✅ Build sem erros

---

### TASK-25: Criar contratos request/response

**ID Azure DevOps**: 108  
**Título**: TASK-25 | Criar contratos de request/response  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1.5h  

**Descrição**:
Criar/consolidar os DTOs que definem os contratos de requisição e resposta, garantindo alinhamento com Domain Model.

**O que fazer**:

1. **DTOs de Requisição**:
   - `CreateOrderRequest`: CustomerId, Items, Description
   - `OrderItemRequest`: ProductId, Quantity, UnitPrice
   - `UpdateOrderRequest`: OrderId, Items

2. **DTOs de Resposta**:
   - `OrderResponse`: OrderId, CustomerId, OrderDate, Status, Items, TotalAmount
   - `OrderItemResponse`: ProductId, Quantity, UnitPrice, SubTotal

3. **Criar/Validar Mapper**:
   - Arquivo: `src/OrderHub.Application/Mappers/OrderMapper.cs`
   - Métodos: `ToDomainEntity()`, `ToResponse()`, `UpdateDomainEntity()`
   - Conversão Guid ↔ String para IDs
   - Tratamento correto de value objects

4. **Documentação**:
   - XML comments em DTOs
   - Exemplos de payload JSON

**Checklist**:
- [ ] DTOs de requisição validados
- [ ] DTOs de resposta com campos calculados
- [ ] Mapper com 3 métodos
- [ ] XML comments adicionados
- [ ] Build sem erros
- [ ] Commit: `feat: Create request/response DTOs and mapper`

**Critério de Aceitação**:
- ✅ Todos os DTOs criados e tipados
- ✅ Mapper converte bidirecionalmente
- ✅ Campos calculados presentes
- ✅ Build sem erros

---

### TASK-26: Documentar casos de uso

**ID Azure DevOps**: 109  
**Título**: TASK-26 | Documentar casos de uso da aplicação  
**Status**: To Do  
**Sprint**: Sprint 1  
**Prioridade**: 2  
**Tempo Estimado**: 1.5h  

**Descrição**:
Documentar todos os casos de uso (use cases) da aplicação OrderHub, incluindo fluxos completos, validações e cenários de erro.

**O que fazer**:

1. **Criar documento de especificação**:
   - Arquivo: `DOC_IA/EPIC-03/USE_CASES_SPECIFICATION.md`
   - Documentar 5 casos de uso:
     - **UC-001**: Create Order
     - **UC-002**: Get Order by ID
     - **UC-003**: Get Orders by Customer
     - **UC-004**: Update Order
     - **UC-005**: Cancel Order

2. **Estrutura de cada UC**:
   ```
   - Identificador
   - Ator
   - Pré-condições
   - Fluxo Principal
   - Fluxo Alternativo (erros)
   - Pós-condições
   - Validações
   - Exceções
   ```

3. **Incluir**:
   - Fluxos de sucesso e erro
   - Validações por UC
   - Exemplos de payload JSON
   - Diagrama de fluxo (Mermaid ou ASCII)
   - Integrações (Database, Email, Payment)

**Checklist**:
- [ ] Arquivo `USE_CASES_SPECIFICATION.md` criado
- [ ] 5 casos de uso documentados
- [ ] Fluxos principais e alternativos descritos
- [ ] Validações listadas
- [ ] Exemplos JSON inclusos
- [ ] Diagrama presente
- [ ] Build ok
- [ ] Commit: `docs: Document application use cases specification`

**Critério de Aceitação**:
- ✅ Documento técnico completo (4-6 páginas)
- ✅ Todos os 5 UCs descritos
- ✅ Fluxos de sucesso e erro documentados
- ✅ Pronto para desenvolvimento

---

## 🔗 Relacionamento entre Tasks

```
FEAT-04 (Application Layer - Complementação)
├── TASK-23: ICreateOrderUseCase (1.5h)
├── TASK-24: IGetOrderUseCase (1.5h)
├── TASK-25: Request/Response DTOs (1.5h)
└── TASK-26: Use Cases Documentation (1.5h)
```

---

## ✅ Critérios de Sucesso

Ao completar FEAT-04:

- ✅ 4 tasks em estado "Done"
- ✅ 2 interfaces claras: `ICreateOrderUseCase`, `IGetOrderUseCase`
- ✅ 4 DTOs criados/validados
- ✅ Mapper bidireccional
- ✅ Documento de 5 casos de uso
- ✅ Build sem erros
- ✅ Pronto para Infrastructure layer

---

## 📆 Estimativa

| Task | Estimado |
|------|----------|
| TASK-23 | 1.5h |
| TASK-24 | 1.5h |
| TASK-25 | 1.5h |
| TASK-26 | 1.5h |
| **TOTAL** | **6-8h** |

---

**Próximos Passos**:
- Infrastructure Layer (Entity Framework, Repositories, UnitOfWork)
- Adapters (SendGrid, Stripe)
- API Layer (Controllers)
