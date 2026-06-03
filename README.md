# Triumph Health Management System — Backend

A multi-tenant healthcare management platform built with .NET 10. It provides REST APIs for managing tenants, facilities, employees, and user accounts, with event-driven audit trails and role-based access control.

---

## Architecture

The solution follows **Clean Architecture** with a **CQRS** pattern, split across focused projects:

```
src/
├── Triumph.HealthMs.Host              # Entry point — wires up DI, middleware, and pipeline
├── Triumph.HealthMs.Core              # Domain models, commands, events, and business logic
├── Triumph.HealthMs.Persistence       # EF Core DbContexts and migrations
├── Triumph.HealthMs.Commands          # Carter module endpoints (write side)
├── Triumph.HealthMs.Queries           # GraphQL with HotChocalate (read side)
└── Triumph.HealthMs.ExternalServices  # Auth0 JWT integration, MassTransit/RabbitMQ, Caching

tests/
└── Triumph.HealthMs.UnitTests         # xUnit unit tests
```

The persistence layer uses **four separate DbContexts** to isolate domain boundaries:

| Context | Responsibility |
|---|---|
| `ApplicationUserManagementDbContext` | User accounts and link invitations |
| `TenantManagementDbContext` | Tenants, tenant managers, subscriptions |
| `FacilityManagementDbContext` | Facilities and facility managers |
| `EmployeeManagementDbContext` | Employees, roles, and permissions |

Audit logs are stored separately in **Marten** (PostgreSQL event store).

---

## Tech Stack

| Tool / Library | Version | Purpose |
|---|---|---|
| .NET | 10.0 | Framework |
| ASP.NET Core | 10.0 | Web host |
| Carter | 10.0.0 | Minimal API endpoint routing |
| Entity Framework Core | 10.0.7 | ORM |
| Npgsql | 10.0.1 | PostgreSQL EF Core provider |
| Marten | 8.34.2 | Event store / audit log |
| MassTransit | 8.5.8 | Message bus abstraction |
| MassTransit.RabbitMQ | 8.5.8 | RabbitMQ transport |
| FluentValidation | 12.1.1 | Command validation |
| Auth0 / JWT Bearer | 10.0.7 | Authentication |
| Serilog | 10.0.0 | Structured logging |
| Sentry | 6.4.1 | Error tracking (production) |
| Scalar | 2.14.7 | Interactive API documentation |
| xUnit | 2.9.3 | Unit testing |

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

- HTTP: `http://localhost:5129`
- HTTPS: `https://localhost:7153`
- Interactive docs (Scalar): `http://localhost:5129/scalar`
- OpenAPI spec: `http://localhost:5129/openapi/v1.json`

---

## Authentication

The API uses **Auth0 JWT Bearer** authentication. Tokens must be passed as `Authorization: Bearer <token>`.

The development Auth0 tenant is pre-configured in `appsettings.Development.json`. For production, set the following in your environment or `appsettings.Production.json`:

```json
{
  "AuthServer": {
    "Authority": "<your-auth0-domain>/",
    "Audience": "<your-api-audience>",
    "AuthorizationUrl": "<your-auth0-domain>/oauth/authorize",
    "TokenUrl": "<your-auth0-domain>/oauth/token",
    "ClientId": "<your-client-id>"
  }
}
```

JWT claims used by the API:

| Claim | Header fallback | Purpose |
|---|---|---|
| `sub` | — | Authenticated user ID |
| `tenant_id` | `x-ms-tenant-id` | Active tenant context |
| `facility_id` | `x-ms-facility-id` | Active facility context |

---

## API Endpoints

All endpoints are versioned under `/api/v1`. Responses follow a standard envelope:

```json
{
  "status": 201,
  "isSuccess": true,
  "message": "User account created successfully",
  "data": "<resource-id>",
  "errors": []
}
```

### User Accounts

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/accounts` | Public | Register a new user account |
| `PUT` | `/api/v1/accounts/{id}` | Required | Update account details |
| `POST` | `/api/v1/accounts/link` | Required | Link user to an existing account |

### Tenants

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/tenants` | Required | Create a tenant |
| `POST` | `/api/v1/tenants/{id}/managers` | Required | Add a tenant manager |
| `DELETE` | `/api/v1/tenants/{id}/managers/{managerId}` | Required | Remove a tenant manager |
| `POST` | `/api/v1/tenants/{id}/subscriptions/renew` | Required | Renew a tenant subscription |

### Facilities

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/facilities` | Manager | Add a facility |
| `PUT` | `/api/v1/facilities/{id}` | Manager | Update facility details |
| `POST` | `/api/v1/facilities/{id}/managers` | Manager | Add a facility manager |
| `DELETE` | `/api/v1/facilities/{id}/managers/{managerId}` | Manager | Remove a facility manager |

### Employees

| Method | Path                                 | Auth | Description                     |
|--------|--------------------------------------|---|---------------------------------|
| `POST` | `/api/v1/employees`                  | Required | Add an employee                 |
| `PUT`  | `/api/v1/employees/{id}/permissions` | Required | Update an employee's permission |
| `PUT`  | `/api/v1/employees/{id}/roles`       | Required | Update an employee's roles      |

### Patients

| Method | Path | Auth | Description |
|---|---|---|---|
| `POST` | `/api/v1/patients` | Required | Create a new patient record |
| `GET` | `/api/v1/patients/{id}` | Required | Retrieve a patient record by ID |
| `PUT` | `/api/v1/patients/{id}` | Required | Update an existing patient record |
| `DELETE` | `/api/v1/patients/{id}` | Required | Delete a patient record |

---

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

A multi-stage Dockerfile is included at `src/Triumph.HealthMs.Host/Dockerfile`. It builds with the .NET 10 SDK and runs on the `aspnet:10.0` runtime image, exposing port `8080`.

The project includes an `appsettings.Docker.json` for Docker-specific configuration overrides.

---

## CI/CD

GitHub Actions workflows are defined in `.github/workflows/`:

- **`dev-ci.yml`** — Runs tests and builds a Docker image tagged with the short commit SHA, then pushes to Docker Hub. Triggers on push to `main` and on pull requests.
- **`development-cd.yml`** — Continuous deployment pipeline.

Required repository secrets: `DOCKER_USERNAME`, `DOCKER_PASSWORD`.