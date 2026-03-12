# USE CASES SPECIFICATION - OrderHub Application

**EPIC**: EPIC-03 | Implementação dos Ports  
**FEATURE**: FEAT-04 | Input Ports  
**TASK**: TASK-26 | Documentar casos de uso da aplicação  
**Version**: 1.0  
**Date**: 2026-03-12  

---

## Overview

Este documento especifica os cinco casos de uso principais da aplicação OrderHub, que implementam a camada de Application da Arquitetura Hexagonal. Os casos de uso definem as interações entre atores externos e o sistema, orquestrando a validação de requisitos, aplicação de regras de negócio do domínio e persistência de dados.

A aplicação segue o padrão de Porta (Port) para definir interfaces de entrada que permitem inversão de controle e testabilidade.

---

## UC-01: Create Order (Criar Pedido)

### Identificador
- **Code**: UC-01
- **Title**: Create Order
- **Input Port**: `ICreateOrderUseCase.ExecuteAsync(CreateOrderRequest, CancellationToken)`
- **Implemented Service**: `CreateOrderService`

### Atores
- **Primary Actor**: Sistema de Vendas / API Cliente
- **Secondary Actors**: Banco de Dados (persistência), Sistema de Notificação

### Pré-condições
1. O sistema está operacional e conectado ao banco de dados
2. O cliente (CustomerId) existe no registro de negócio
3. Pelo menos um produto com ID válido está disponível no catálogo

### Pós-condições (Sucesso)
1. Um novo agregado `Order` foi criado com status "Pending"
2. O pedido foi persistido no banco de dados
3. Todos os itens foram adicionados ao pedido
4. Uma notificação de criação de pedido foi disparada
5. Um identificador único (OrderId) foi gerado e retornado

### Pós-condições (Falha)
1. Nenhum registro de pedido foi criado
2. Uma exceção com mensagem descritiva foi retornada
3. A transação foi revertida (rollback automático)

### Fluxo Principal (Happy Path)

1. Cliente invoca `ICreateOrderUseCase.ExecuteAsync()` passando `CreateOrderRequest`
2. Sistema valida se o request não é nulo
3. Sistema valida se `CustomerId` não está vazio
4. Sistema valida se a lista de `Items` contém pelo menos 1 item
5. Sistema inicia uma transação no banco de dados
6. Sistema cria um novo `OrderId` (Value Object immutável)
7. Sistema cria um novo `CustomerId` a partir da string fornecida
8. Sistema cria o agregado `Order` via factory method `Order.CreateOrder(orderId, customerId)`
9. Para cada item no request:
   - Sistema converte `UnitPrice` em `OrderAmount` (Value Object)
   - Sistema cria `ProductId` a partir do ID fornecido
   - Sistema cria `OrderItem` com ProductId, Quantity e Amount
   - Sistema adiciona o item ao agregado `Order`
10. Sistema persiste o agregado via `IOrderRepository.SaveAsync()`
11. Sistema confirma a transação (commit)
12. Sistema dispara notificação via `INotificationPort` com dados do pedido
13. Sistema retorna `OrderResponse` com:
    - OrderId (identificador único)
    - CustomerId
    - OrderDate (data de criação)
    - Status ("Pending")
    - Items (lista de itens)
    - TotalAmount (soma de subtotais)
    - Currency ("BRL")

### Fluxos Alternativos

**Alt-1: CustomerId Inválido**
- Pré-requisito: CustomerId está vazio, nulo ou contém apenas espaços
- Ação: Sistema lança `ArgumentException` com mensagem "CustomerId é obrigatório"
- Resultado: Request é rejeitado, nenhum pedido é criado

**Alt-2: Sem Itens**
- Pré-requisito: Lista de Items é nula ou está vazia
- Ação: Sistema lança `ArgumentException` com mensagem "Pedido deve ter no mínimo 1 item"
- Resultado: Request é rejeitado, nenhum pedido é criado

**Alt-3: Erro na Persistência**
- Pré-requisito: Banco de dados está indisponível durante SaveAsync
- Ação: Sistema faz rollback automático da transação
- Resultado: Uma exceção com contexto é retornada, cliente pode retentar

**Alt-4: Request Nulo**
- Pré-requisito: CreateOrderRequest é nulo
- Ação: Sistema lança `ArgumentNullException`
- Resultado: Falha fast-fail, sem processamento

### Validações
- ✅ Request não pode ser nulo
- ✅ CustomerId obrigatório (não vazio, não whitespace)
- ✅ Items não pode ser nulo ou vazio
- ✅ OrderId é gerado automaticamente como UUID
- ✅ Cada OrderItem requer ProductId, Quantity > 0, UnitPrice > 0
- ✅ TotalAmount é calculado automaticamente (∑ Quantity × UnitPrice)

### Exemplo de Request/Response

**Request**:
```json
{
  "customerId": "cust-12345",
  "items": [
    {
      "productId": "prod-001",
      "quantity": 2,
      "unitPrice": 99.90
    },
    {
      "productId": "prod-002",
      "quantity": 1,
      "unitPrice": 149.99
    }
  ],
  "description": "Pedido via aplicativo mobile"
}
```

**Response** (HTTP 201):
```json
{
  "orderId": "order-550e8400-e29b-41d4-a716-446655440000",
  "customerId": "cust-12345",
  "orderDate": "2026-03-12T23:00:00Z",
  "status": "Pending",
  "items": [
    {
      "productId": "prod-001",
      "quantity": 2,
      "unitPrice": 99.90,
      "subTotal": 199.80
    },
    {
      "productId": "prod-002",
      "quantity": 1,
      "unitPrice": 149.99,
      "subTotal": 149.99
    }
  ],
  "totalAmount": 349.79,
  "currency": "BRL",
  "description": "Pedido via aplicativo mobile"
}
```

---

## UC-02: Get Order by ID (Recuperar Pedido por ID)

### Identificador
- **Code**: UC-02
- **Title**: Get Order by ID
- **Input Port**: `IGetOrderUseCase.ExecuteAsync(string orderId, CancellationToken)`
- **Implemented Service**: `GetOrderService`

### Atores
- **Primary Actor**: Sistema de Consultas / API Cliente
- **Secondary Actors**: Banco de Dados (leitura)

### Pré-condições
1. O sistema está operacional e conectado ao banco de dados
2. Um pedido com o OrderId fornecido existe no banco de dados

### Pós-condições (Sucesso)
1. Os dados completos do pedido foram recuperados
2. Uma instância de `OrderResponse` foi retornada com todos os detalhes

### Pós-condições (Falha)
1. Uma exceção descritiva foi lançada
2. Nenhum dado foi modificado no banco de dados

### Fluxo Principal

1. Cliente invoca `IGetOrderUseCase.ExecuteAsync(orderId)`
2. Sistema valida se orderId não está vazio ou nulo
3. Sistema consulta `IOrderRepository.GetByIdAsync(orderId)`
4. Sistema verifica se o pedido foi encontrado no banco
5. Se encontrado:
   - Sistema retorna `OrderResponse` com todos os dados (OrderId, CustomerId, OrderDate, Status, Items, TotalAmount, Currency, Description)
6. Se não encontrado:
   - Sistema lança `InvalidOperationException` com mensagem "Pedido com ID '{orderId}' não encontrado"

### Validações
- ✅ OrderId obrigatório (não vazio, não whitespace)
- ✅ Pedido deve existir no banco de dados
- ✅ Resposta inclui todos os itens do pedido

### Exemplo

**Request**:
```
GET /orders/order-550e8400-e29b-41d4-a716-446655440000
```

**Response** (HTTP 200):
```json
{
  "orderId": "order-550e8400-e29b-41d4-a716-446655440000",
  "customerId": "cust-12345",
  "orderDate": "2026-03-12T23:00:00Z",
  "status": "Pending",
  "items": [
    {
      "productId": "prod-001",
      "quantity": 2,
      "unitPrice": 99.90,
      "subTotal": 199.80
    }
  ],
  "totalAmount": 199.80,
  "currency": "BRL"
}
```

---

## UC-03: Get Orders by Customer (Recuperar Pedidos por Cliente)

### Identificador
- **Code**: UC-03
- **Title**: Get Orders by Customer
- **Input Port**: `IGetOrderUseCase.GetByCustomerAsync(string customerId, CancellationToken)`
- **Implemented Service**: `GetOrderService`

### Atores
- **Primary Actor**: Sistema de Consultas / API Cliente / Cliente
- **Secondary Actors**: Banco de Dados (leitura)

### Pré-condições
1. O sistema está operacional
2. Um cliente com o CustomerId fornecido pode ter 0 ou mais pedidos

### Pós-condições (Sucesso)
1. Uma lista de `OrderResponse` foi retornada (pode estar vazia)
2. Todos os pedidos do cliente foram incluídos

### Fluxo Principal

1. Cliente invoca `IGetOrderUseCase.GetByCustomerAsync(customerId)`
2. Sistema valida se customerId não está vazio
3. Sistema consulta `IOrderRepository.GetByCustomerIdAsync(customerId)`
4. Sistema retorna lista de todos os `OrderResponse` do cliente:
   - Se cliente tem pedidos: lista não vazia
   - Se cliente não tem pedidos: lista vazia (não erro)

### Validações
- ✅ CustomerId obrigatório (não vazio)
- ✅ Retorna lista vazia se cliente não tem pedidos (não falha)

### Exemplo

**Request**:
```
GET /customers/cust-12345/orders
```

**Response** (HTTP 200):
```json
[
  {
    "orderId": "order-550e8400-e29b-41d4-a716-446655440000",
    "customerId": "cust-12345",
    "orderDate": "2026-03-12T23:00:00Z",
    "status": "Pending",
    "items": [
      {
        "productId": "prod-001",
        "quantity": 2,
        "unitPrice": 99.90,
        "subTotal": 199.80
      }
    ],
    "totalAmount": 199.80,
    "currency": "BRL"
  },
  {
    "orderId": "order-660f9402-f30c-52e5-b827-557766551111",
    "customerId": "cust-12345",
    "orderDate": "2026-03-11T10:30:00Z",
    "status": "Completed",
    "items": [
      {
        "productId": "prod-003",
        "quantity": 1,
        "unitPrice": 49.99,
        "subTotal": 49.99
      }
    ],
    "totalAmount": 49.99,
    "currency": "BRL"
  }
]
```

---

## UC-04: Update Order (Atualizar Pedido)

### Identificador
- **Code**: UC-04
- **Title**: Update Order
- **Input Port**: `IUpdateOrderUseCase.ExecuteAsync(string orderId, UpdateOrderRequest, CancellationToken)` *(A ser implementado em FEAT-05)*
- **Implemented Service**: `UpdateOrderService`

### Atores
- **Primary Actor**: Sistema de Vendas / API Cliente
- **Secondary Actors**: Banco de Dados (leitura/escrita)

### Pré-condições
1. Um pedido com o OrderId fornecido existe no banco de dados
2. O pedido está em status "Pending" (apenas pedidos pendentes podem ser atualizados)
3. A requisição contém pelo menos 1 item

### Pós-condições (Sucesso)
1. Os itens do pedido foram substituídos pelos novos itens
2. O TotalAmount foi recalculado
3. O OrderDate permanece o original (data de criação)
4. Status permanece "Pending"
5. Mudanças foram persistidas no banco de dados

### Fluxo Principal

1. Cliente invoca `IUpdateOrderUseCase.ExecuteAsync(orderId, updateRequest)`
2. Sistema valida se orderId não está vazio
3. Sistema consulta e recupera o pedido do banco via `IOrderRepository.GetByIdAsync(orderId)`
4. Sistema valida se o pedido foi encontrado
5. Sistema valida se o pedido está em status "Pending"
6. Sistema valida se updateRequest contém Items
7. Sistema remove todos os itens atuais do agregado
8. Sistema adiciona novos itens seguindo a mesma lógica de CreateOrder
9. Sistema persiste as alterações via `IOrderRepository.SaveAsync()`
10. Sistema retorna `OrderResponse` atualizado

### Validações
- ✅ OrderId obrigatório
- ✅ Pedido deve existir
- ✅ Pedido deve estar em status "Pending"
- ✅ Items não pode estar vazio
- ✅ Os novos items devem ser válidos

### Exemplo

**Request**:
```json
{
  "items": [
    {
      "productId": "prod-001",
      "quantity": 3,
      "unitPrice": 99.90
    }
  ]
}
```

**Response** (HTTP 200):
```json
{
  "orderId": "order-550e8400-e29b-41d4-a716-446655440000",
  "customerId": "cust-12345",
  "orderDate": "2026-03-12T23:00:00Z",
  "status": "Pending",
  "items": [
    {
      "productId": "prod-001",
      "quantity": 3,
      "unitPrice": 99.90,
      "subTotal": 299.70
    }
  ],
  "totalAmount": 299.70,
  "currency": "BRL"
}
```

---

## UC-05: Cancel Order (Cancelar Pedido)

### Identificador
- **Code**: UC-05
- **Title**: Cancel Order
- **Input Port**: `ICancelOrderUseCase.ExecuteAsync(string orderId, CancellationToken)` *(A ser implementado em FEAT-05)*
- **Implemented Service**: `CancelOrderService`

### Atores
- **Primary Actor**: Sistema de Vendas / API Cliente / Cliente
- **Secondary Actors**: Banco de Dados (escrita), Sistema de Notificação

### Pré-condições
1. Um pedido com o OrderId fornecido existe no banco de dados
2. O pedido pode estar em status "Pending" ou "Confirmed" (não "Completed" ou "Cancelled")

### Pós-condições (Sucesso)
1. O status do pedido foi alterado para "Cancelled"
2. Os itens foram mantidos para auditoria/histórico
3. Uma notificação de cancelamento foi disparada
4. Mudanças foram persistidas no banco de dados

### Pós-condições (Falha)
1. Se pedido já está "Completed" ou "Cancelled": exceção é lançada
2. Se pedido não existe: exceção "não encontrado" é lançada

### Fluxo Principal

1. Cliente invoca `ICancelOrderUseCase.ExecuteAsync(orderId)`
2. Sistema valida se orderId não está vazio
3. Sistema consulta e recupera o pedido via `IOrderRepository.GetByIdAsync(orderId)`
4. Sistema valida se o pedido foi encontrado
5. Sistema valida se o pedido pode ser cancelado (status é "Pending" ou "Confirmed")
6. Sistema altera o status para "Cancelled" via método de domínio
7. Sistema persiste as alterações via `IOrderRepository.SaveAsync()`
8. Sistema dispara notificação de cancelamento via `INotificationPort`
9. Sistema retorna `OrderResponse` com status atualizado

### Validações
- ✅ OrderId obrigatório
- ✅ Pedido deve existir
- ✅ Pedido não pode estar "Completed" ou "Cancelled"
- ✅ Apenas status "Pending" e "Confirmed" podem ser cancelados

### Exemplo

**Request**:
```
PATCH /orders/order-550e8400-e29b-41d4-a716-446655440000/cancel
```

**Response** (HTTP 200):
```json
{
  "orderId": "order-550e8400-e29b-41d4-a716-446655440000",
  "customerId": "cust-12345",
  "orderDate": "2026-03-12T23:00:00Z",
  "status": "Cancelled",
  "items": [
    {
      "productId": "prod-001",
      "quantity": 2,
      "unitPrice": 99.90,
      "subTotal": 199.80
    }
  ],
  "totalAmount": 199.80,
  "currency": "BRL"
}
```

---

## Summary Table

| UC Code | Título | Port Interface | Serviço | FEAT | Status |
|---------|--------|-----------------|---------|------|--------|
| UC-01 | Create Order | `ICreateOrderUseCase` | `CreateOrderService` | FEAT-04 | ✅ Implementado |
| UC-02 | Get Order by ID | `IGetOrderUseCase` | `GetOrderService` | FEAT-04 | ✅ Implementado |
| UC-03 | Get Orders by Customer | `IGetOrderUseCase` | `GetOrderService` | FEAT-04 | ✅ Implementado |
| UC-04 | Update Order | `IUpdateOrderUseCase` | `UpdateOrderService` | FEAT-05 | ⏳ Próximas |
| UC-05 | Cancel Order | `ICancelOrderUseCase` | `CancelOrderService` | FEAT-05 | ⏳ Próximas |

---

## Architectural Context

### Hexagonal Architecture Alignment

Cada caso de uso representa um **Input Port** (porta de entrada) da arquitetura hexagonal:

```
┌─────────────────────────────────────────────────────────────┐
│                    ADAPTERS (Controllers)                   │
│  HTTP API, gRPC Clients, CLI, Message Queue Subscribers    │
└──────────────┬──────────────────────────────────────────────┘
               │
┌──────────────▼──────────────────────────────────────────────┐
│                    INPUT PORTS (Use Cases)                  │
│  ICreateOrderUseCase, IGetOrderUseCase, IUpdateOrderUseCase │
└──────────────┬──────────────────────────────────────────────┘
               │
┌──────────────▼──────────────────────────────────────────────┐
│              APPLICATION SERVICES (Orchestration)           │
│  CreateOrderService, GetOrderService, UpdateOrderService    │
└──────────────┬──────────────────────────────────────────────┘
               │
┌──────────────▼──────────────────────────────────────────────┐
│                    DOMAIN LAYER (Business)                  │
│  Aggregates: Order, OrderItem | Value Objects: OrderId ...  │
└──────────────┬──────────────────────────────────────────────┘
               │
┌──────────────▼──────────────────────────────────────────────┐
│                   OUTPUT PORTS (Interfaces)                 │
│  IOrderRepository, INotificationPort, IPaymentPort          │
└──────────────┬──────────────────────────────────────────────┘
               │
┌──────────────▼──────────────────────────────────────────────┐
│              INFRASTRUCTURE ADAPTERS (Implementations)      │
│  EntityFramework Repository, SMTP Notification, PayPal      │
└─────────────────────────────────────────────────────────────┘
```

### Dependências de Portas

```
Use Cases Dependencies:
  - CreateOrderService → IUnitOfWork, INotificationPort
  - GetOrderService → IOrderRepository
  - UpdateOrderService → IOrderRepository
  - CancelOrderService → IOrderRepository, INotificationPort
```

---

## Notes

1. **FEAT-04** implementa as interfaces e especificações de UC-01, UC-02, UC-03
2. **FEAT-05** implementará as interfaces para UC-04 e UC-05 (Update e Cancel)
3. **Infrastructure Layer** virá em **FEAT-06** com implementações de Database e Notification
4. Todos os casos de uso usam **cancellation tokens** para operações assíncronas
5. Validações são duplicadas no Application Service para "fail-fast" e no Domain para invariantes de negócio
6. TransactionScope garante ACID compliance

---

**Last Updated**: 2026-03-12  
**Version**: 1.0  
**Author**: Development Team
