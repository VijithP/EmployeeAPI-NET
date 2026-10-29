# Employee Portal API

An ASP.NET Core 10 Web API for employee management. The solution uses a clean-architecture-style separation of concerns, JWT bearer authentication, SQL Server persistence, a repository pattern, and a SQL connection factory.

## Features

- JWT bearer token creation and validation.
- Authenticated employee endpoints to list employees, retrieve one employee, and create an employee.
- SQL Server data access through parameterized ADO.NET queries.
- Separate API, application, domain, and infrastructure projects.
- Swagger UI in the Development environment.

> **CRUD status:** Employee creation and read operations are implemented. Update (`PUT`/`PATCH`) and delete (`DELETE`) endpoints are not currently implemented.

## Requirements

- .NET 10 SDK
- SQL Server (local or remote)

## Architecture

The projects separate HTTP concerns, application logic, domain rules, and persistence:

| Project | Responsibility |
| --- | --- |
| `EmployeePortal.Api` | Controllers, JWT authentication/authorization, Swagger, middleware, and dependency registration. |
| `Employee-Portal.Application` | Employee use cases, service/repository interfaces, request and response DTOs. |
| `Employee-Portal.Domain` | The `Employee` entity and its creation/validation rules. |
| `Employee-Portal.Infrastructure` | SQL Server connection creation, SQL repository implementation, and infrastructure registrations. |

The API calls the application service; the application depends on the repository interface, and infrastructure provides its SQL Server implementation. The domain project has no dependency on the API or infrastructure.

`EmployeeRepository` implements the repository pattern. `SqlConnectionFactory` encapsulates creation of SQL Server connections from the configured `EmployeeDb` connection string. Infrastructure registers the factory and repository with dependency injection.

## Configure and run

### Configure secrets and the database connection

Set these values using .NET user secrets during local development. Run the commands from the repository root:

```powershell
dotnet user-secrets init --project EmployeePortal.Api
dotnet user-secrets set "Jwt:SecretKey" "<a-random-secret-of-at-least-32-bytes>" --project EmployeePortal.Api
dotnet user-secrets set "Jwt:Issuer" "JwtAuthApi" --project EmployeePortal.Api
dotnet user-secrets set "Jwt:Audience" "AngularEmployeeApp" --project EmployeePortal.Api
dotnet user-secrets set "Jwt:ExpiryMinutes" "15" --project EmployeePortal.Api
dotnet user-secrets set "ConnectionStrings:EmployeeDb" "Server=localhost;Database=EmployeeDb;Trusted_Connection=True;TrustServerCertificate=True" --project EmployeePortal.Api
```

Use the connection string appropriate for your SQL Server installation. The checked-in `appsettings.json` contains a local Windows-authentication connection string and a placeholder JWT key; replace these values through user secrets or environment variables. Do not commit real signing keys or production credentials.

The API expects a database named `EmployeeDb` and a table with the following schema. Create the database first if it does not exist, then run this table definition in that database:

```sql
CREATE TABLE dbo.Employees
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Salary DECIMAL(18,2) NOT NULL
);
```

### Start the API

From the repository root:

```powershell
dotnet build EmployeePortal.Api\EmployeePortal.Api.csproj
dotnet run --project EmployeePortal.Api\EmployeePortal.Api.csproj
```

The HTTP launch profile listens at `http://localhost:5209`. In Development, Swagger UI is available at [http://localhost:5209/swagger](http://localhost:5209/swagger).

## Authentication

`POST /api/auth/login` issues a signed JWT. **The current login is demo-only**: it accepts the hard-coded credentials below and does not validate users against the database.

```http
POST http://localhost:5209/api/auth/login
Content-Type: application/json

{
  "email": "admin",
  "password": "admin123"
}
```

A successful response contains `accessToken` and `expiresAtUtc`. The token includes subject, email, name, and role claims. Invalid credentials return `401 Unauthorized`.

Pass the token as a bearer token when calling protected employee endpoints:

```http
GET http://localhost:5209/api/employees
Authorization: Bearer <access-token>
```

Swagger's **Authorize** dialog accepts the token without the `Bearer ` prefix. Token validation checks the configured issuer, audience, signing key, and expiration.

> Replace the demo login with real user authentication and use a secure secret provider before deploying this API.

## API endpoints

| Method | Route | Authentication | Description |
| --- | --- | --- | --- |
| `POST` | `/api/auth/login` | Public | Validate demo credentials and issue a JWT. |
| `GET` | `/api/employees` | JWT required | Return all employees from SQL Server. |
| `GET` | `/api/employees/{id}` | JWT required | Return one employee, or `404 Not Found` if it does not exist. |
| `POST` | `/api/employees` | JWT required | Create an employee and return `201 Created`. |
| `GET` | `/api/employees/me` | JWT required | Return the authenticated user's ID, email, and authentication status. |
| `GET` | `/WeatherForecast` | Public | Return a sample five-day forecast. |

Example create request:

```http
POST http://localhost:5209/api/employees
Authorization: Bearer <access-token>
Content-Type: application/json

{
  "name": "Ava Patel",
  "email": "ava.patel@example.com",
  "salary": 75000.00
}
```

The create request requires a name of 2–100 characters, a valid email address, and a salary greater than zero and no more than `100000000`. The API returns the created employee with its database-generated ID.

## Configuration

Settings are read from standard ASP.NET Core configuration sources. Environment variables use double underscores in place of `:`:

| Setting | Purpose |
| --- | --- |
| `Jwt__SecretKey` | HMAC signing and validation key; use a random secret of at least 32 bytes. |
| `Jwt__Issuer` | Issuer written to and expected in JWTs. |
| `Jwt__Audience` | Audience written to and expected in JWTs. |
| `Jwt__ExpiryMinutes` | JWT lifetime in minutes; defaults to `15` when omitted during token creation. |
| `ConnectionStrings__EmployeeDb` | SQL Server connection string used by the connection factory. |

The configured CORS policy permits the `http://localhost:4200` frontend origin.
