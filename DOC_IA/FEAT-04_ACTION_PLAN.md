# FEAT-04 Action Plan
## Adapter Layer Implementation

**Status**: PLANNED  
**Target**: EPIC-02 (Core Implementation)  
**Estimated Duration**: 8-10 hours  
**Start Date**: [Ready to Start]  

---

## Overview
FEAT-04 will implement the Adapter Layer, bridging Application Services to external systems (databases, email, payments). This layer realizes the concrete implementations of all Ports defined in FEAT-03.

---

## Task Breakdown

### TASK-21: Create OrderHub.Infrastructure Project (2h)
- [ ] Create project structure: `src/OrderHub.Infrastructure/`
- [ ] Organize folders: Repositories, Ports/Adapters, Configuration
- [ ] Add NuGet dependencies:
  - `Microsoft.EntityFrameworkCore` (6.0+)
  - `Microsoft.EntityFrameworkCore.SqlServer`
  - `SendGrid` (email service)
  - `Stripe.net` (payment processing)
- [ ] Setup configuration infrastructure
- [ ] Create DbContext foundation
- [ ] Update solution file (OrderHub.slnx)

**Acceptance Criteria**:
- Project compiles without errors
- All dependencies installed
- Project structure follows hexagonal pattern
- .NET 10.0 configured

---

### TASK-22: Implement SQL Repository (3h)
- Create `IOrderRepository` implementation (OrderRepository.cs)
  - GetByIdAsync → EF Core query
  - GetByCustomerIdAsync → filtered query
  - SaveAsync → AddAsync/UpdateAsync
  - DeleteAsync → remove with transaction
  - ExistsAsync → any() check
- Create EF Core DbContext (OrderHubDbContext)
  - Order entity mapping
  - OrderItem entity mapping
  - Value Object converters
  - Relationships (Customer, Product)
- Add database migrations
- Configure connection string

**Acceptance Criteria**:
- All repository methods tested
- Database schema created via migrations
- CRUD operations working
- Transaction safety verified

---

### TASK-23: Implement IUnitOfWork (1.5h)
- Create `UnitOfWork` implementation
  - Dependency: OrderHubDbContext
  - Orders property → OrderRepository instance
  - BeginTransactionAsync → DbTransaction
  - CommitAsync → SaveChangesAsync + transaction commit
  - RollbackAsync → transaction rollback
  - HasActiveTransaction property
- Add transaction timeout handling
- Implement IAsyncDisposable

**Acceptance Criteria**:
- Transaction isolation tested
- Rollback scenarios working
- Connection pooling configured

---

### TASK-24: Implement Notification Service (2h)
- Create `NotificationAdapter` implementing INotificationPort
  - SendOrderConfirmationAsync → SendGrid email template
  - SendOrderApprovedAsync → approved email
  - SendOrderShippedAsync → tracking email
  - SendOrderDeliveredAsync → delivery confirmation
  - SendOrderCancelledAsync → cancellation notice
  - SendCustomNotificationAsync → generic template
- Configure SendGrid API key
- Create email templates
- Add logging for failures
- Implement retry logic (optional)

**Acceptance Criteria**:
- Emails send successfully (sandbox mode)
- Log entries created for all notifications
- Graceful error handling
- Templates localized (Portuguese)

---

### TASK-25: Implement Payment Integration (2.5h)
- Create `PaymentAdapter` implementing IPaymentPort
  - ValidatePaymentAsync → Stripe card validation
  - ProcessPaymentAsync → charge card, return transactionId
  - RefundPaymentAsync → create refund
  - GetPaymentStatusAsync → query transaction status
- Configure Stripe API key
- Implement idempotency keys
- Add logging for compliance
- Handle payment failures gracefully

**Acceptance Criteria**:
- Stripe test environment working
- Mock payment data processed
- Refund capability verified
- Error codes properly handled

---

### TASK-26: Integration Tests (2h)
- Create `OrderHub.Infrastructure.Tests` project
- Test OrderRepository operations
  - Create, Read, Update, Delete
  - Query by customer
  - Transaction rollback scenarios
- Test UnitOfWork
  - Transaction flow
  - Connection management
- Test Notification adapter (mock SendGrid)
- Test Payment adapter (mock Stripe)

**Acceptance Criteria**:
- 20+ integration tests
- All database operations tested
- External service mocks working
- >80% infrastructure code coverage

---

## Dependencies & Setup

### NuGet Packages Required
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0" />
  <PackageReference Include="SendGrid" Version="10.0.0" />
  <PackageReference Include="Stripe.net" Version="48.0.0" />
  <PackageReference Include="Microsoft.Extensions.Configuration" Version="10.0.0" />
  <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.0" />
</ItemGroup>
```

### Configuration Setup
- Database connection string: `appsettings.json`
- SendGrid API key: User Secrets
- Stripe API key: User Secrets
- Payment webhook endpoint (for async updates)

---

## Architecture Pattern

```
Application Layer (FEAT-03 ✅)
    ↓
[PORTS → ADAPTERS PATTERN]
    ↓
Infrastructure/Adapter Layer (FEAT-04)
    ├── Repositories
    │   └── OrderRepository
    │       └── EF Core DbContext
    │       └── SQL Database
    ├── Notifications
    │   └── SendGrid Email Service
    └── Payments
        └── Stripe Payment Gateway
```

---

## Git Workflow
Each TASK will follow:
1. Create feature branch: `git checkout -b feat/TASK-XX-description`
2. Implement code with tests
3. Commit atomically: `git commit -m "feat: TASK-XX message"`
4. Push to develop: `git push origin develop`
5. Update work item state: To Do → Doing → Done

---

## Success Criteria
- [ ] All 6 tasks completed
- [ ] 30+ integration tests passing
- [ ] Database migrations created
- [ ] External APIs (Stripe, SendGrid) integrated
- [ ] Transaction management verified
- [ ] Code coverage >80%
- [ ] Zero build warnings
- [ ] Deployment-ready infrastructure

---

## Known Considerations
- **Database**: Currently SQL Server, adapter pattern allows PostgreSQL switch
- **Email**: SendGrid free tier (100 emails/day) sufficient for MVP
- **Payments**: Stripe test mode for development, production keys in secrets
- **Security**: All API keys in appsettings.json (local dev) or Azure Key Vault (production)
- **Idempotency**: Payment transactions need idempotent keys to prevent duplicates

---

## Deployment Notes
- EF Core migrations must run on deployment
- Database user needs schema creation permissions
- SendGrid API key must be valid
- Stripe webhook endpoint requires HTTPS in production
- Connection pooling configured for multi-instance deployments

---

## Ready to Start?
Execute following sequence:
1. ✅ FEAT-03 Complete
2. → Create FEAT-04 work item (Feature)
3. → Create TASK-21 through TASK-26 as child tasks
4. → Set FEAT-04 to Doing
5. → Begin TASK-21 implementation

**Estimated Sprint Hours**: 8-10 hours  
**Complexity**: Medium (dependent on API integrations)  
**Risk Level**: Low (Ports well-defined, clear contracts)
