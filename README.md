# ProductsAPI — RESTful Backend API

A production-grade RESTful API for managing Products and Items, built with .NET 8 and ASP.NET Core, following Clean Architecture principles. Built as part of a technical assessment.

## Overview

This API provides full CRUD operations for Products, with JWT-based authentication, refresh token rotation, role-based authorization, pagination, structured logging, and a complete automated test suite.

## Architecture

The solution follows **Clean Architecture** with four distinct layers:

```
ProductsAPI/
├── ProductsAPI.API            → Controllers, Middleware, Program.cs (presentation layer)
├── ProductsAPI.Application    → DTOs, Services, Validators, Interfaces (business logic)
├── ProductsAPI.Domain         → Entities, Exceptions (core business models, zero dependencies)
└── ProductsAPI.Infrastructure → DbContext, Repositories, JWT/Auth services (external concerns)
```

**Dependency rule:** outer layers depend on inner layers, never the reverse. `Domain` has zero dependencies on any other layer.

```
API → Application → Domain
API → Infrastructure → Domain
```

## Tech Stack

| Category | Technology |
|---|---|
| Framework | .NET 8, ASP.NET Core Web API |
| Database | SQL Server with Entity Framework Core 8 |
| Authentication | JWT Bearer tokens with refresh token rotation |
| Validation | FluentValidation |
| Object Mapping | AutoMapper |
| API Documentation | Swagger / OpenAPI (Swashbuckle) |
| Logging | Serilog (structured console logging) |
| Testing | xUnit, Moq, FluentAssertions, WebApplicationFactory, EF Core InMemory |
| Containerization | Docker, Docker Compose |
| API Versioning | Asp.Versioning (URL-based, `api/v1/...`) |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (Express edition is sufficient) **or** Docker Desktop
- Visual Studio 2022/2026 (recommended) or VS Code

## Setup Instructions

### 1. Clone the repository

```bash
git clone https://github.com/rohith-08/ProductsAPI.git
cd ProductsAPI
```

### 2. Update the connection string

Open `ProductsAPI.API/appsettings.json` and update `ConnectionStrings:DefaultConnection` to match your local SQL Server instance.

### 3. Restore dependencies

```bash
dotnet restore
```

### 4. Apply database migrations

```bash
cd ProductsAPI.Infrastructure
dotnet ef database update --startup-project ../ProductsAPI.API
```

### 5. Run the application

```bash
cd ../ProductsAPI.API
dotnet run
```

The API will be available locally at:

```
https://localhost:7245/swagger
```

*(Port may vary by machine — check the console output on `dotnet run` or the launch profile in Visual Studio.)*

### Running with Docker

```bash
docker compose build
docker compose up
```

Once running, Swagger UI will be available at:

```
http://localhost:5000/swagger/index.html
```

> **Note:** The Dockerized version runs on plain HTTP (not HTTPS) by default, since the container does not have a TLS certificate mounted. This is standard practice for local Docker demos.

## API Endpoints

### Authentication

| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| POST | `/api/v1/auth/register` | Register a new user | No |
| POST | `/api/v1/auth/login` | Login and receive tokens | No |
| POST | `/api/v1/auth/refresh-token` | Get a new access token | No |
| POST | `/api/v1/auth/revoke-token` | Revoke a refresh token (logout) | No |

### Products

| Method | Endpoint | Description | Auth Required |
|---|---|---|---|
| GET | `/api/v1/products?pageNumber=1&pageSize=10` | Get paginated products | No |
| GET | `/api/v1/products/{id}` | Get a single product | No |
| POST | `/api/v1/products` | Create a product | Yes |
| PUT | `/api/v1/products/{id}` | Update a product | Yes |
| DELETE | `/api/v1/products/{id}` | Delete a product | Yes (Admin role) |

## Authentication Flow

1. Register or login to receive an `accessToken` (60 min expiry) and a `refreshToken` (7 day expiry)
2. Include the access token in the `Authorization` header as `Bearer <token>` for protected endpoints
3. When the access token expires, call `/api/v1/auth/refresh-token` with the refresh token to get a new pair without re-entering credentials

## Running Tests

```bash
dotnet test
```

The test suite includes:

- **`ProductsAPI.Application.Tests`** — unit tests for `ProductService` business logic, with mocked `IUnitOfWork` and `IProductRepository` dependencies via Moq
- **`ProductsAPI.API.Tests`** — integration tests for `AuthController` and `ProductsController` covering the full HTTP pipeline via `WebApplicationFactory`, backed by an EF Core in-memory database
- **`ProductsAPI.Infrastructure.Tests`** — repository-level tests verifying `ProductRepository` behavior (add, update, delete, query with related `Item` entities) against an in-memory database

All 22 tests pass, covering both happy paths (successful create/update/delete, valid login) and failure paths (`NotFoundException`, `401 Unauthorized` on missing token, duplicate email on registration, invalid credentials).

## Security Measures

- JWT authentication with short-lived access tokens and refresh token rotation
- Role-based authorization (e.g. Delete restricted to Admin role)
- Password hashing with BCrypt
- FluentValidation on all incoming requests
- Global exception handling middleware with consistent error responses
- Security headers middleware (X-Frame-Options, X-Content-Type-Options, Content-Security-Policy)
- CORS policy restricted to named origins

## Performance Considerations

- `AsNoTracking()` used on all read-only queries
- Pagination on all collection endpoints
- Async/await throughout for non-blocking I/O

## Author

**NEELA ROHITH**
GitHub: [rohith-08](https://github.com/rohith-08)
Email: rohithneela369@gmail.com
Repository: [github.com/rohith-08/ProductsAPI](https://github.com/rohith-08/ProductsAPI)