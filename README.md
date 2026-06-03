# Triumph Health Management System — Backend

A multi-tenant healthcare management platform built with .NET 10. It provides **dual APIs** for managing tenants, facilities, employees, and user accounts:
- **REST APIs** (via Carter) for write operations (commands)
- **GraphQL APIs** (via HotChocolate) for read operations (queries)

Features event-driven audit trails, role-based access control, and multi-tenant data isolation.

---

## Architecture

The solution follows **Clean Architecture** with a **CQRS pattern**, separating read and write sides:

```
src/
├── Triumph.HealthMs.Host              # Entry point — wires up DI, middleware, and pipeline
├── Triumph.HealthMs.Core              # Domain models, commands, queries, events, and business logic
├── Triumph.HealthMs.Persistence       # EF Core DbContexts and migrations
├── Triumph.HealthMs.Commands          # REST endpoints via Carter (write side — POST, PUT, DELETE)
├── Triumph.HealthMs.Queries           # GraphQL resolvers via HotChocolate (read side — queries)
└── Triumph.HealthMs.ExternalServices  # Auth0 JWT, MassTransit/RabbitMQ, caching, event handlers

tests/
└── Triumph.HealthMs.UnitTests         # xUnit unit tests
```

### API Split

| Side | Protocol | Framework | Purpose |
|------|----------|-----------|---------|
| **Write** | REST | Carter (minimal APIs) | Commands, state changes `/api/v1/*` |
| **Read** | GraphQL | HotChocolate | Queries, data retrieval `/graphql` |
| **Docs** | HTTP | Scalar | Interactive API documentation `/scalar` |

### Domain Isolation

The persistence layer uses **four separate DbContexts** to isolate domain boundaries:

| DbContext | Responsibility |
|---|---|
| `ApplicationUserManagementDbContext` | User accounts, invitations, authentication |
| `TenantManagementDbContext` | Tenants, tenant managers, subscriptions |
| `FacilityManagementDbContext` | Facilities, facility managers, departments |
| `EmployeeManagementDbContext` | Employees, roles, permissions, employment records |

**Audit & Event Store**: Marten (PostgreSQL event store) captures all domain events for audit trails and event sourcing.

---

## Tech Stack

| Category | Tool / Library | Version | Purpose |
|---|---|---|---|
| **Framework** | .NET | 10.0 | Runtime |
| | ASP.NET Core | 10.0 | Web host |
| **APIs** | Carter | 10.0.0 | REST minimal API routing (write side) |
| | HotChocolate | Latest | GraphQL server (read side) |
| **Data** | Entity Framework Core | 10.0.7 | ORM |
| | Npgsql | 10.0.1 | PostgreSQL provider |
| | Marten | 8.34.2 | Event store / audit trail |
| **Messaging** | MassTransit | 8.5.8 | Message bus abstraction |
| | MassTransit.RabbitMQ | 8.5.8 | RabbitMQ transport |
| **Validation** | FluentValidation | 12.1.1 | Command/query validation |
| **Auth** | IdentityModel | Latest | JWT validation |
| | Microsoft.IdentityModel.Tokens | Latest | Token handling |
| **Observability** | Serilog | 10.0.0 | Structured logging |
| | Sentry.Serilog | 6.4.1 | Error tracking (production) |
| **Documentation** | Scalar | 2.14.7 | Interactive GraphQL/OpenAPI UI |
| | Microsoft.AspNetCore.OpenApi | 10.0.1 | OpenAPI endpoint |
| **Testing** | xUnit | 2.9.3 | Unit testing framework |
| **Containerization** | Docker | — | Multi-stage Dockerfile support |

---

## Architectural Decisions

### CQRS Pattern
The application strictly separates commands (write operations) from queries (read operations) using CQRS:
- **Commands** are handled by Carter REST endpoints and routed through command handlers
- **Queries** are resolved through GraphQL resolvers, using a query handler interface
- Benefits: independent scaling, optimized data access patterns, clear API boundaries

### Multi-Tenant Data Isolation
- Each entity includes `TenantId` for row-level security
- Separate DbContexts prevent cross-domain queries, enforcing bounded context principles
- Middleware (`UserResourceResolverMiddleware`) validates tenant ownership before processing requests

### Domain-Driven Design
- Domain boundaries organized by subdomain: ApplicationUser, Tenant, Facility, Employee, Patient
- Command and query folders mirror domain structure
- Events published through MassTransit enable asynchronous workflows between subdomains

### Request/Response Validation
- All commands validated via **FluentValidation** before handler execution
- Authorization checks use an `IPermissionService` injected into handlers
- GraphQL directives enforce field-level authorization

### Caching Strategy
- Subscription data cached for 30 days in distributed cache
- Cache service abstraction allows in-memory or distributed implementations
- User resource resolver caches tenant/facility lookups per request

### Event-Driven Architecture
- **MassTransit** provides publish-subscribe for domain events
- **Marten** event store captures audit trails for compliance
- RabbitMQ transport in production; in-memory bus for local development

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL 12+](https://www.postgresql.org/)
- [RabbitMQ](https://www.rabbitmq.com/) — optional in development (falls back to in-memory bus)
- An [Auth0](https://auth0.com/) tenant — credentials are pre-configured for development

---

## Local Setup

### 1. Clone and restore

```bash
git clone <repo-url>
cd triumph-health-backend-new
dotnet restore Triumph.HealthMs.sln
```

### 2. Configure the database connection

Edit `src/Triumph.HealthMs.Host/appsettings.Development.json` and set your PostgreSQL connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TriumphHealthDb;User Id=<user>;Password=<password>;"
  }
}
```

### 3. Apply EF Core migrations

```bash
dotnet ef database update \
  --project src/Triumph.HealthMs.Persistence \
  --startup-project src/Triumph.HealthMs.Host
```

### 4. Run the application

```bash
dotnet run --project src/Triumph.HealthMs.Host
```

The API will be available at:

| Endpoint | URL | Purpose |
|---|---|---|
| REST API | `https://localhost:7153/api/v1/*` | Write operations (commands) |
| GraphQL | `https://localhost:7153/graphql` | Read operations (queries) |
| GraphQL Sandbox | `https://localhost:7153/scalar` | Interactive GraphQL explorer |
| OpenAPI Doc | `https://localhost:7153/openapi/v1.json` | REST API schema |
| Health Check | `https://localhost:7153/graphql?query={healthCheck}` | Service readiness |

### Example Requests

#### Create a Tenant (REST)

```bash
curl -X POST https://localhost:7153/api/v1/tenants \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Acme Healthcare",
    "identifier": "acme-hc"
  }'
```

#### Query Tenant Facilities (GraphQL)

```graphql
query {
  facilities(request: { tenantId: "123e4567-e89b-12d3-a456-426614174000" }) {
    id
    name
    address
    managers {
      employeeId
      name
    }
  }
}
```

Visit `/scalar` in your browser for an interactive API explorer.

---

## Authentication & Authorization

### JWT Bearer Authentication

The API uses **JWT Bearer tokens** for authentication. All token validation is performed by the `UserResourceResolverMiddleware`:

1. Token is extracted from `Authorization: Bearer <token>` header
2. Issued by your configured Auth0 tenant or identity provider
3. Claims are mapped to `ILoggedInUserService` for request context

### Tenant & Facility Context

Multi-tenant requests pass context through:

| Claim | Header fallback | Purpose |
|---|---|---|
| `sub` | — | Authenticated user ID |
| `tenant_id` | `x-ms-tenant-id` | Active tenant context (required for tenant operations) |
| `facility_id` | `x-ms-facility-id` | Active facility context (required for facility operations) |

The middleware validates:
- User exists in `ApplicationUsersDbContext`
- Tenant exists in `TenantManagementDbContext`
- Facility exists in `FacilityManagementDbContext`

### Authorization Policies

Authorization is enforced at two levels:

1. **Endpoint/Field Level** — via `[Authorize]` and `[AllowAnonymous]` attributes
2. **Business Logic Level** — via `IPermissionService` for role-based and feature-based checks

Example roles:
- `SuperAdmin` — system administration
- `TenantManager` — tenant-level management
- `FacilityManager` — facility-level management
- `Employee` — general access

### Configuration

Development Auth0 settings are in `appsettings.Development.json`. For production, configure:

```json
{
  "AuthServer": {
    "Authority": "https://your-auth0-domain/",
    "Audience": "your-api-identifier",
    "AuthorizationUrl": "https://your-auth0-domain/authorize",
    "TokenUrl": "https://your-auth0-domain/oauth/token",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "RoleClaimType": "https://your-domain/roles/roles"
  }
}
```

**Note:** Roles are mapped from a custom claim `https://your-domain/roles/roles` (configured in `RegisterExternalServicesLayer.cs`).

---

## API Endpoints

The API provides two ways to interact with data:

### REST API (Write Operations via Carter)

All REST endpoints are versioned under `/api/v1`. Responses follow a standard envelope:

```json
{
  "status": 201,
  "isSuccess": true,
  "message": "User account created successfully",
  "data": "<resource-id>",
  "errors": []
}
```

#### User Accounts

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/accounts` | Public | Register a new user account |
| `PUT` | `/api/v1/accounts/{id}` | Required | Update account details |

#### Tenants

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/tenants` | Required | Create a tenant |
| `POST` | `/api/v1/tenants/{id}/managers` | Required | Add a tenant manager |
| `DELETE` | `/api/v1/tenants/{id}/managers/{managerId}` | Required | Remove a tenant manager |
| `POST` | `/api/v1/tenants/{id}/departments` | Required | Add a department to a tenant |
| `POST` | `/api/v1/tenants/{id}/subscriptions/renew` | Required | Renew a tenant subscription |

#### Facilities

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/facilities` | Manager | Create a facility |
| `PUT` | `/api/v1/facilities/{id}` | Manager | Update facility details |
| `POST` | `/api/v1/facilities/{id}/managers` | Manager | Add a facility manager |
| `DELETE` | `/api/v1/facilities/{id}/managers/{managerId}` | Manager | Remove a facility manager |

#### Employees

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/employees` | Required | Add an employee |
| `PUT` | `/api/v1/employees/{id}/permissions` | Required | Update employee permissions |
| `PUT` | `/api/v1/employees/{id}/roles` | Required | Update employee roles |

#### Patients

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/patients` | Required | Create a patient |
| `POST` | `/api/v1/patients/{id}/visitations` | Required | Add a patient visit |
| `POST` | `/api/v1/patients/{id}/vital-measurements` | Required | Record patient vital signs |
| `PUT` | `/api/v1/patients/{id}` | Required | Update patient details |
| `POST` | `/api/v1/patients/{id}/identifications` | Required | Add patient identification |
| `DELETE` | `/api/v1/patients/{id}/identifications/{identificationId}` | Required | Remove patient identification |

#### Common Entities (Health Internals)

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/drugs` | Admin | Add a drug to the system |
| `POST` | `/api/v1/health-diagnoses` | Admin | Add a health diagnosis |
| `POST` | `/api/v1/lab-tests` | Admin | Add a lab test type |

### GraphQL API (Read Operations via HotChocolate)

Access the GraphQL endpoint at `/graphql`. Use the interactive Scalar docs at `/scalar` for schema exploration.

#### Root Query Fields

| Field | Auth | Description |
|---|---|---|
| `subscriptions` | Public | Get all available subscription tiers |
| `employeePermissions` | Required | Get all permissions in the system |
| `drugs` | Required | Get all drugs in the system |
| `healthDiagnoses` | Required | Get all health diagnoses |
| `labTests` | Required | Get all laboratory tests |
| `vitalItems` | Required | Get all vital measurement items |

#### Tenant Queries

| Field | Auth | Description |
|---|---|---|
| `allTenants` | SuperAdmin | List all tenants with optional filters |
| `singleTenant` | Manager | Get current tenant details |
| `facilities` | Public | Get facilities for a tenant |
| `departments` | Manager | Get departments in a tenant |

#### Employee Queries

| Field | Auth | Description |
|---|---|---|
| `employees` | Required | List employees with filters (facility, role, etc.) |
| `totalEmployees` | Required | Get total employee count |

#### Application Health

| Field | Auth | Description |
|---|---|---|
| `healthCheck` | Public | Health check probe for readiness |

**GraphQL Features:**
- Field-level authorization via `@authorize` directive
- Selective field loading (prevents N+1 queries)
- Caching on subscription data (30-day TTL)

---

## Request Pipeline

### Middleware Order (in `PipelineStartup.cs`)

1. **ForwardedHeaders** — handles proxy headers (X-Forwarded-*)
2. **ExceptionHandler** — global exception handling
3. **CORS** — enables cross-origin requests via "SecurePolicy"
4. **Authentication** — JWT bearer token validation
5. **UserResourceResolverMiddleware** — validates tenant/facility ownership, throws `GraphQLRequestException` for invalid claims
6. **Authorization** — enforces `[Authorize]` attributes
7. **GraphQL** — routes `/graphql` requests
8. **Carter** — routes `/api/v1/*` REST endpoints

### Request Flow Examples

**REST Command:**
```
POST /api/v1/tenants
  ↓
ForwardedHeaders, CORS
  ↓
Authentication (JWT validation)
  ↓
UserResourceResolverMiddleware (tenant validation)
  ↓
Authorization (endpoint-level checks)
  ↓
Carter route handler
  ↓
Command validation (FluentValidation)
  ↓
Command handler execution
  ↓
Event publication (MassTransit)
  ↓
201 response with resource ID
```

**GraphQL Query:**
```
POST /graphql
  ↓
Authentication, UserResourceResolverMiddleware
  ↓
HotChocolate resolver dispatch
  ↓
Field-level authorization checks
  ↓
Query handler execution
  ↓
Selective field resolution + caching
  ↓
200 response with data
```

## Database Migrations

Create a new migration (replace `MigrationName` with a descriptive name):

```bash
dotnet ef migrations add <MigrationName> \
  --project src/Triumph.HealthMs.Persistence \
  --startup-project src/Triumph.HealthMs.Host
```

Apply pending migrations:

```bash
dotnet ef database update \
  --project src/Triumph.HealthMs.Persistence \
  --startup-project src/Triumph.HealthMs.Host
```

---

## Running Tests

```bash
dotnet test tests/Triumph.HealthMs.UnitTests/Triumph.HealthMs.UnitTests.csproj
```

---

## Docker

A multi-stage **Dockerfile** is included at `src/Triumph.HealthMs.Host/Dockerfile`:

1. **Build Stage** — uses `mcr.microsoft.com/dotnet:10.0-sdk` to compile the solution
2. **Runtime Stage** — uses `mcr.microsoft.com/dotnet:10.0-aspnet` for minimal image size

The application listens on **port 8080** inside the container. Expose it with:

```bash
docker build -t triumph-health-api:latest -f src/Triumph.HealthMs.Host/Dockerfile .
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Server=postgres-server;Database=TriumphHealthDb;..." \
  triumph-health-api:latest
```

**Configuration overrides** for Docker are defined in `appsettings.Docker.json`.

## Project Structure & Conventions

### Commands Project (`Triumph.HealthMs.Commands`)

Organized by domain subdomain (DDD approach):

```
V1/
├── ApplicationUser/
│   ├── AddUserAccountEndpoint.cs        # Carter ICarterModule
│   └── UpdateUserAccountInformationEndpoint.cs
├── TenantManagement/
│   ├── AddTenantAccountEndpoint.cs      # POST /api/v1/tenants
│   ├── AddTenantManagerEndpoint.cs      # POST /api/v1/tenants/{id}/managers
│   ├── RenewSubscriptionEndpoint.cs
│   └── ...
├── FacilityManagement/
├── EmployeeManagement/
├── PatientManagement/
└── CommonEntitiesManagement/            # Health internals (drugs, diagnoses)
```

**Conventions:**
- File naming: `<Action><Noun>Endpoint.cs` (e.g., `AddTenantEndpoint.cs`)
- Class implements `ICarterModule` and defines `AddRoutes()`
- Endpoints specify auth requirements via `.RequireAuthorization()` and filters
- Filters (`TenantIdRequiredFilter`, `MustBeAManagerFilter`) enforce role/resource checks

### Queries Project (`Triumph.HealthMs.Queries`)

GraphQL query resolvers extending `QueryBase`:

```
QueryTypes/
├── QueryBase.cs                # Root query fields (subscriptions, permissions, drugs, etc.)
├── TenantsQueries.cs           # [ExtendObjectType<QueryBase>]
│   └── AllTenants(), SingleTenant(), Facilities(), Departments()
├── EmployeesQueries.cs         # Employee queries with pagination
├── AppConfigurationQueries.cs  # GetAppConfigurations()
└── HealthCheckQuery.cs         # Health probe
```

**Conventions:**
- Classes decorated with `[ExtendObjectType<QueryBase>]` (HotChocolate type extensions)
- Methods async returning `Task<T>` with `IResolverContext` for selective loading
- Field-level auth via `[Authorize]` and `[AllowAnonymous]` attributes
- Use `resolverContext.IsSelected("fieldName")` to optimize queries

### Core Project (`Triumph.HealthMs.Core`)

Domain logic split by concern:

```
Features/
├── ApplicationUserManagement/
│   ├── ApplicationUserCommandHandler.cs
│   ├── ApplicationUserQuery.cs
│   └── ...
├── TenantManagement/
├── FacilityManagement/
├── EmployeeManagement/
└── PatientManagement/

Interfaces/
├── IPermissionService.cs         # Role/permission checks
├── ILoggedInUserService.cs       # Current user context
└── ...

Models/
├── DTOs/
├── Common/
└── Events/                       # Domain events published via MassTransit
```

### Persistence Project (`Triumph.HealthMs.Persistence`)

Four isolated DbContexts with migrations:

```
Data/
├── ApplicationUserManagementDbContext.cs
├── TenantManagementDbContext.cs
├── FacilityManagementDbContext.cs
└── EmployeeManagementDbContext.cs

Services/                        # Marten event store integration
```

---

## CI/CD

GitHub Actions workflows are defined in `.github/workflows/`:

- **`dev-ci.yml`** — Runs tests and builds a Docker image tagged with the short commit SHA, then pushes to Docker Hub. Triggers on push to `main` and on pull requests.
- **`development-cd.yml`** — Continuous deployment pipeline.

Required repository secrets: `DOCKER_USERNAME`, `DOCKER_PASSWORD`.