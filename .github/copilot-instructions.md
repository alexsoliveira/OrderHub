# Copilot Instructions - OrderHub Project

## 📋 Project Overview

**OrderHub** is an educational .NET 8 project demonstrating **Hexagonal Architecture** (Ports & Adapters) combined with **Domain-Driven Design (DDD)** principles. The system manages orders with clean separation of concerns, making the business logic independent from external dependencies.

### Key Technologies
- **.NET 8** (net10.0 target framework)
- **Entity Framework Core** for persistence
- **ASP.NET Core** for REST API
- **SQL Server** for database
- **Swagger** for API documentation
- **xUnit** for testing

---

## 🏛️ Architecture Principles

### Hexagonal Architecture Layers

```
┌─────────────────────────────────────────────────┐
│  Adapters.Inbound.Api (Controllers, Models)    │
├─────────────────────────────────────────────────┤
│  Application (UseCases, DTOs, Validators)      │
├─────────────────────────────────────────────────┤
│  Domain (Aggregates, Entities, ValueObjects)   │
├─────────────────────────────────────────────────┤
│  Adapters.Outbound.Persistence (EF Core)       │
└─────────────────────────────────────────────────┘
```

### Core Principles

1. **Isolated Business Logic**: Domain layer contains zero external dependencies
2. **Port-Driven Design**: Communication through interfaces, not concrete implementations
3. **Dependency Inversion**: High-level modules depend on abstractions
4. **Domain Events**: Aggregates raise events for important state changes
5. **Value Objects**: Immutable objects representing domain concepts
6. **Aggregate Roots**: Single entry point for aggregate modifications

---

## 📁 Project Structure & Responsibilities

### `OrderHub.Domain`
**Core business logic with zero external dependencies**
- **Aggregates/**: Root entities (`Order`, `OrderItem`, etc.)
  - Inherit from `AggregateRoot` base class
  - Raise domain events via `RaiseDomainEvent()`
  - Validate state using `DomainValidator` static methods
- **ValueObjects/**: Immutable domain concepts (`OrderId`, `CustomerId`, `OrderStatus`, etc.)
  - Implement equality by value, not reference
  - Encapsulate domain validations
- **Ports/**: Interfaces for external communication
  - `IOrderRepository`: Contract for order persistence
  - Output ports (driven dependencies)
- **Exceptions/**: Domain-specific exceptions (`DomainException`)
- **Events/**: Domain event classes
- **Constants/**: Domain-level constants and enumerations

### `OrderHub.Application`
**Use cases and business workflows**
- **UseCases/**: Command/Query handlers implementing business operations
  - Interface: `ICreateOrderUseCase`, `IGetOrderUseCase`, etc.
  - Interface-driven design for easy mocking
  - Accept application DTOs, return application DTOs
- **DTOs/**: Data Transfer Objects for input/output boundaries
  - Separate from domain objects
  - No business logic
- **Validators/**: Application-level validators (FluentValidation style)
- **Mappers/**: Convert between Domain ↔ Application layers
- **Commands/**: Encapsulate command data with handlers
- **Queries/**: Encapsulate query data with handlers
- **Exceptions/**: Application-specific exceptions
- **Ports/**: Interfaces defining adapter contracts (input and output ports)

### `OrderHub.Adapters.Inbound.Api`
**HTTP API layer - Entry point to the system**
- **Controllers/**: REST endpoints mapped to use cases
  - Dependency inject use case interfaces
  - Validate requests and return appropriate HTTP responses
  - Map API models ↔ Application DTOs
- **Models/**: Request/Response DTOs (API contracts)
  - `CreateOrderRequest`, `OrderResponse`, etc.
  - Match API versioning (e.g., `/api/v1/orders`)
- **Middleware/**: Custom HTTP middleware (logging, error handling)
- **Validators/**: Validate incoming API requests
- **Filters/**: Action filters, authorization, etc.
- **Mappers/**: Convert between API models and application DTOs

### `OrderHub.Adapters.Outbound.Persistence`
**Data persistence implementation (currently EF Core)**
- **OrderHubDbContext**: Entity Framework Core DbContext
  - Configures entity mappings and relationships
  - Manages database schema
- **Repositories/**: Implement output port interfaces from Domain
  - `IOrderRepository` implementation
  - CRUD operations on domain aggregates
- **Mappings/**: EF Core entity configurations
- **Migrations/**: Database schema change scripts

### `tests/`
**Test projects following layer structure**
- **OrderHub.Domain.Tests/**: Unit tests for domain logic
  - Aggregate tests, ValueObject tests
  - No external dependencies (pure .NET)
- **OrderHub.Application.Tests/**: UseCase and mapper tests
  - Mock repositories and external services
  - Test business workflow logic

---

## 🎯 Naming Conventions & Code Style

### General Conventions
- **Language**: Portuguese for domain language (class names, properties, comments)
- **Namespaces**: `OrderHub.{Layer}.{Feature}` structure
- **Casing**: 
  - `PascalCase` for classes, properties, methods
  - `camelCase` for local variables, parameters, private fields (prefixed with `_`)
  - `UPPER_CASE` for constants

### Naming Patterns

| Element | Pattern | Example |
|---------|---------|---------|
| **Aggregates** | `{Entity}` | `Order`, `OrderItem` |
| **ValueObjects** | `{Concept}` | `OrderId`, `CustomerId`, `OrderStatus` |
| **Repositories** | `I{Aggregate}Repository` | `IOrderRepository` |
| **UseCases** | `I{Operation}{Noun}UseCase` | `ICreateOrderUseCase`, `IGetOrderUseCase` |
| **DTOs** | `{Noun}Dto` or `{Operation}{Noun}Dto` | `OrderDto`, `CreateOrderDto` |
| **Controllers** | `{Plural}Controller` | `OrdersController` |
| **Validators** | `{Class}Validator` | `CreateOrderValidator` |
| **Exceptions** | `{Context}Exception` | `DomainException`, `ApplicationException` |

### Code Organization
- **File naming**: Match class name (one public class per file)
- **Using statements**: Group by System, external packages, then project namespaces
- **XML documentation**: Use `/// <summary>` comments on public members
- **Null handling**: Use explicit null checks or nullable reference types (`string?`)

---

## 🛠️ Development Guidelines

### When Creating New Features

1. **Start in Domain Layer**
   ```csharp
   // 1. Create domain aggregate or value object
   public class {Entity} : AggregateRoot { }
   
   // 2. Define domain port (repository interface)
   public interface I{Entity}Repository { }
   
   // 3. Define domain exceptions if needed
   public class {Entity}Exception : DomainException { }
   ```

2. **Add Application Layer**
   ```csharp
   // 1. Create use case interface
   public interface I{Operation}{Noun}UseCase { }
   
   // 2. Implement use case
   public class {Operation}{Noun}UseCase : I{Operation}{Noun}UseCase { }
   
   // 3. Create DTOs
   public class {Operation}{Noun}Dto { }
   
   // 4. Create mappers if needed
   public static class {Noun}Mapper { }
   ```

3. **Add API Adapter Layer**
   ```csharp
   // 1. Create API request/response models
   public class {Operation}{Noun}Request { }
   public class {Noun}Response { }
   
   // 2. Add endpoint in controller
   [HttpPost]
   public async Task<IActionResult> {Operation}{Noun}Async(...) { }
   
   // 3. Add validation if needed
   public class {Operation}{Noun}Validator : AbstractValidator<{Operation}{Noun}Request> { }
   ```

4. **Implement Persistence Adapter**
   ```csharp
   // 1. Create EF Core entity mapping
   public class {Entity}Configuration : IEntityTypeConfiguration<{Entity}> { }
   
   // 2. Implement repository interface
   public class {Entity}Repository : I{Entity}Repository { }
   
   // 3. Add DbSet to OrderHubDbContext
   public DbSet<{Entity}> {Entities} { get; set; }
   ```

### Dependency Injection Pattern

```csharp
// In Program.cs or extension method
builder.Services.AddScoped<I{UseCase}UseCase, {UseCase}>();
builder.Services.AddScoped<I{Repository}Repository, {Repository}>();
```

### Error Handling

```csharp
// Domain Layer - Throw DomainException
throw new DomainException("Business rule violation message");

// Application Layer - Catch and wrap if needed
try {
    // domain operation
} catch (DomainException ex) {
    throw new ApplicationException("Use case failed", ex);
}

// API Layer - Map to HTTP response
catch (DomainException ex) {
    return BadRequest(new { error = ex.Message });
}
```

### Validation

```csharp
// Domain - Use DomainValidator for rules
public static class DomainValidator {
    public static void ThrowIfNull<T>(T? value, string message) where T : class;
    public static void ThrowIfNegativeOrZero(decimal value, string message);
    public static void ThrowIfEmpty(string? value, string message);
    // More methods...
}

// Application - Create FluentValidation validators
public class CreateOrderValidator : AbstractValidator<CreateOrderDto> {
    public CreateOrderValidator() {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty();
    }
}
```

### Mapping Between Layers

```csharp
// API Model → Application DTO
var appDto = MapToApplicationDto(apiRequest);

// Application DTO → Domain Aggregate
var aggregate = CreateOrderUseCase.ExecuteAsync(appDto);

// Domain Aggregate → Application DTO
var appResponse = MapToApplicationDto(aggregate);

// Application DTO → API Response
var apiResponse = MapToApiResponse(appResponse);
```

---

## ✅ Testing Guidelines

### Test Project Structure
- Mirror source project structure
- Test class naming: `{ClassUnderTest}Tests`
- Test method naming: `{OperationScenario}_Should{ExpectedResult}`

### Domain Layer Testing
```csharp
public class OrderTests {
    [Fact]
    public void CreateOrder_WithValidData_ShouldCreateSuccessfully() {
        // Arrange
        var customerId = new CustomerId(Guid.NewGuid());
        var items = new List<OrderItem> { /* ... */ };
        
        // Act
        var order = Order.Create(customerId, items);
        
        // Assert
        Assert.NotNull(order);
        Assert.Equal(customerId, order.CustomerId);
    }
}
```

### Application Layer Testing
```csharp
public class CreateOrderUseCaseTests {
    [Fact]
    public async Task Execute_WithValidRequest_ShouldCreateOrder() {
        // Arrange
        var mockRepository = new Mock<IOrderRepository>();
        var useCase = new CreateOrderUseCase(mockRepository.Object);
        var request = new CreateOrderDto { /* ... */ };
        
        // Act
        var result = await useCase.ExecuteAsync(request, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        mockRepository.Verify(r => r.SaveAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

### Test Fixtures
- Use fixture classes for common setup
- Located in `Fixtures/` folder per test project
- Mock external dependencies (repositories, services)

---

## 🔄 Git Workflow & Branching Strategy

### Branch Naming Convention
- **Feature**: `feature/{EPIC-##}/{FEAT-##}_description`
  - Example: `feature/EPIC-02/FEAT-01_create-order-api`
- **Bugfix**: `bugfix/{BUG-##}_description`
- **Release**: `release/v{version}`
  - Example: `release/v1.0.0`

### Branch Protection Rules

| Branch | Protection Level |
|--------|------------------|
| `main` | Requires 2 reviewers, all checks pass, all comments resolved |
| `develop` | Requires 1 reviewer, build must pass, all comments resolved |
| `feature/*` | PR to `develop` |
| `bugfix/*` | PR to `develop` |

### Pull Request Workflow
1. Create feature branch from `develop`
2. Commit with meaningful messages (imperative mood)
3. Push and create PR with description of changes
4. Address review comments
5. Merge and auto-delete branch

---

## 🔧 Common Development Tasks

### Building & Testing
```powershell
# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run all tests
dotnet test

# Run specific test project
dotnet test tests/OrderHub.Domain.Tests

# Build in Release configuration
dotnet build -c Release
```

### Database Operations
```powershell
# Add migration
dotnet ef migrations add {MigrationName} -p src/OrderHub.Adapters.Outbound.Persistence

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove -p src/OrderHub.Adapters.Outbound.Persistence
```

### Running the API
```powershell
cd src/OrderHub.Adapters.Inbound.Api
dotnet run
# API available at https://localhost:7000
# Swagger UI at https://localhost:7000/swagger
```

### Code Quality Checks
```powershell
# Check for code style issues
dotnet build /p:EnforceCodeStyleInBuild=true

# Run code analysis
dotnet build /p:EnableNETAnalyzers=true
```

---

## 📚 File Structure Quick Reference

```
OrderHub/
├── src/
│   ├── OrderHub.Domain/              # Core business logic (no dependencies)
│   │   ├── Aggregates/               # Root entities
│   │   ├── ValueObjects/             # Immutable value objects
│   │   ├── Ports/                    # Output port interfaces
│   │   ├── Exceptions/               # Domain exceptions
│   │   └── DomainValidator.cs        # Centralized validation rules
│   │
│   ├── OrderHub.Application/         # Use cases and application logic
│   │   ├── UseCases/                 # IUseCase interfaces and implementations
│   │   ├── DTOs/                     # Data transfer objects
│   │   ├── Validators/               # Application-level validators
│   │   ├── Mappers/                  # Conversion logic
│   │   └── Exceptions/               # Application exceptions
│   │
│   ├── OrderHub.Adapters.Inbound.Api/  # REST API entry point
│   │   ├── Controllers/              # REST endpoints
│   │   ├── Models/                   # API request/response models
│   │   ├── Mappers/                  # API ↔ Application DTO conversion
│   │   ├── Validators/               # API request validators
│   │   ├── Program.cs                # Startup configuration
│   │   ├── appsettings.json          # Configuration files
│   │   └── OrderHub.Adapters.Inbound.Api.http  # HTTP test file
│   │
│   └── OrderHub.Adapters.Outbound.Persistence/  # Data access
│       ├── Repositories/             # Repository implementations
│       ├── Mappings/                 # EF Core configurations
│       ├── Migrations/               # Database schema versions
│       ├── OrderHubDbContext.cs      # EF Core context
│       └── OrderHubDbContextFactory.cs  # Context factory
│
├── tests/
│   ├── OrderHub.Domain.Tests/        # Domain layer tests
│   ├── OrderHub.Application.Tests/   # Application layer tests
│   └── {other}.Tests/                # Other layer tests
│
├── DOC_IA/                           # AI-generated documentation
│   ├── EPIC-##/
│   │   ├── FEAT-##_ACTION_PLAN.md
│   │   ├── FEAT-##_COMPLETION_REPORT.md
│   │   └── ...
│
├── OrderHub.slnx                     # Solution file
├── azure-pipelines.yml               # CI/CD pipeline
├── BRANCH_POLICY.md                  # Branching guidelines
└── README.md                         # Project documentation
```

---

## ⚙️ Configuration & Settings

### Database Connection
**Location**: `src/OrderHub.Adapters.Inbound.Api/appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=OrderHubDb;Trusted_Connection=true;"
  }
}
```

### Development Settings
**Location**: `src/OrderHub.Adapters.Inbound.Api/appsettings.Development.json`
- Overrides base settings for local development
- Disables HTTPS redirect
- Enables detailed logging

### API Versioning
- Current version: **v1**
- Route pattern: `/api/v1/{resource}`

---

## 🚀 CI/CD Pipeline

### Azure Pipelines
- Triggered on: `main`, `develop`, `feature/*`, `bugfix/*`
- Stages:
  1. **Build**: Restore, build, run tests
  2. **Deploy** (when merged to main): Publish to production

### Build Configuration
- Framework: .NET 8
- Configuration: Release for CI, Debug for development
- All tests must pass before merge

---

## 📖 Key Patterns Used

### Aggregate Root Pattern
```csharp
public abstract class AggregateRoot {
    private readonly List<object> _domainEvents = new();
    
    public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();
    
    protected void RaiseDomainEvent(object @event) => _domainEvents.Add(@event);
    
    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

### Repository Pattern
```csharp
public interface IOrderRepository {
    Task<Order?> GetByIdAsync(OrderId id, CancellationToken cancellationToken);
    Task SaveAsync(Order order, CancellationToken cancellationToken);
    Task DeleteAsync(OrderId id, CancellationToken cancellationToken);
}
```

### Use Case Pattern
```csharp
public interface ICreateOrderUseCase {
    Task<OrderDto> ExecuteAsync(CreateOrderDto request, CancellationToken cancellationToken);
}

public class CreateOrderUseCase : ICreateOrderUseCase {
    public async Task<OrderDto> ExecuteAsync(CreateOrderDto request, CancellationToken cancellationToken) {
        // Business logic here
    }
}
```

### Value Object Pattern
```csharp
public class OrderId : IEquatable<OrderId> {
    public Guid Value { get; }
    
    public OrderId(Guid value) {
        DomainValidator.ThrowIfEmpty(value, "Order ID cannot be empty");
        Value = value;
    }
    
    public override bool Equals(object? obj) => obj is OrderId id && id.Value == Value;
    public override int GetHashCode() => Value.GetHashCode();
}
```

---

## 🎓 Learning Resources

- **Hexagonal Architecture**: Alistair Cockburn's original article
- **Domain-Driven Design**: Eric Evans' DDD book
- **.NET 8 Documentation**: Microsoft Learn
- **Entity Framework Core**: Official Microsoft documentation
- **SOLID Principles**: Robert C. Martin

---

## 📞 Support & Questions

When assisting with this project, prioritize:
1. **Maintaining architecture integrity** - Keep layers properly separated
2. **Domain-first approach** - Domain layer has no external dependencies
3. **Interface-driven design** - Use ports for all external communication
4. **Comprehensive testing** - Domain and application layers thoroughly tested
5. **Code consistency** - Follow established naming conventions and patterns

---

**Last Updated**: March 2026  
**Framework Version**: .NET 8  
**Architecture Pattern**: Hexagonal Architecture with DDD
