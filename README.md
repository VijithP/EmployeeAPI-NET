# MyWebApi

An ASP.NET Core 10 Web API sample demonstrating JWT bearer authentication, protected employee endpoints, and a public weather forecast endpoint.

## Requirements

- .NET 10 SDK

## Run the API

The development launch profile uses `http://localhost:5209` and opens Swagger UI at `/swagger`.

The application requires a JWT signing key and JWT issuer and audience configuration. For local development, configure them with .NET user secrets instead of committing a real key:

```powershell
dotnet user-secrets init
dotnet user-secrets set "Jwt:SecretKey" "<a-random-secret-of-at-least-32-bytes>"
dotnet user-secrets set "Jwt:Issuer" "MyWebApi"
dotnet user-secrets set "Jwt:Audience" "MyWebApiClient"
dotnet user-secrets set "Jwt:ExpiryMinutes" "15"
```

Run the project from its directory:

```powershell
dotnet run
```

Alternatively, build first:

```powershell
dotnet build
dotnet run
```

Do not use the sample login credentials or a development signing key in production. Replace the demo login with real user validation and configure a strong signing key through a secure production secret provider.

## JWT authentication

### Get an access token

Send a `POST` request to `/api/auth/login` with JSON credentials:

```http
POST http://localhost:5209/api/auth/login
Content-Type: application/json

{
  "email": "admin",
  "password": "admin123"
}
```

The current login endpoint is **demo-only** and accepts the credentials shown above. It returns an `accessToken` and its UTC expiration time. The token contains subject, email, name, and role claims. Invalid credentials return `401 Unauthorized`.

### Call a protected endpoint

Include the token in the `Authorization` header:

```http
GET http://localhost:5209/api/employees
Authorization: Bearer <access-token>
Accept: application/json
```

To use Swagger UI, open [http://localhost:5209/swagger](http://localhost:5209/swagger), call the login endpoint, copy the returned `accessToken`, and select **Authorize**. Paste the token only; Swagger adds the `Bearer` prefix. Requests without a valid token to protected endpoints return `401 Unauthorized`.

## API endpoints

| Method | Route | Authentication | Description |
| --- | --- | --- | --- |
| `POST` | `/api/auth/login` | Public | Validate demo credentials and issue a JWT |
| `GET` | `/api/employees` | Bearer token required | Return the sample employee list |
| `GET` | `/api/employees/me` | Bearer token required | Return the authenticated user's ID, email, and authentication status |
| `GET` | `/WeatherForecast` | Public | Return a sample five-day weather forecast |

Employee records are currently stored in memory in `EmployeeService`; this is sample data, not database-backed persistence.

## Configuration

JWT settings are read from the `Jwt` configuration section:

| Setting | Purpose |
| --- | --- |
| `SecretKey` | HMAC signing key used to issue and validate tokens |
| `Issuer` | Expected token issuer |
| `Audience` | Expected token audience |
| `ExpiryMinutes` | Token lifetime in minutes; the login endpoint defaults to `15` if this value is omitted |

ASP.NET Core configuration supports environment variables using double underscores, for example `Jwt__Issuer` and `Jwt__Audience`. Keep real signing keys out of source control and logs.

## Project structure

- `Controllers/AuthController.cs` — demo login and JWT creation.
- `Controllers/EmployeesController.cs` — authenticated employee and current-user endpoints.
- `Controllers/WeatherForecastController.cs` — public sample forecast endpoint.
- `Services/` — employee service interface and in-memory implementation.
- `Models/EmployeeResponse.cs` — employee response model.
- `Middleware/RequestLoggingMiddleware.cs` — request and response logging.
- `Program.cs` — service registration, JWT validation, Swagger, and middleware pipeline.
