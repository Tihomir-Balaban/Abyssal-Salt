# Abyssal-Salt (AbySalto.Mid)

ASP.NET Core **.NET 9** Web API implementing a small e-commerce-style backend: products, basket, orders, favorites, and JWT auth.

## Solution layout

- **AbySalto.Mid.Domain** — entities (Product, Basket, Order, Favorite, User, …)
- **AbySalto.Mid.Application** — application services, requests/DTOs, validation, app exceptions
- **AbySalto.Mid.Infrastructure** — EF Core persistence (SQL Server), repositories, JWT token service
- **AbySalto.Mid** — ASP.NET Core Web API (controllers, Swagger, exception middleware)
- **AbySalto.Mid.Tests** — unit tests
- **AbySalto.Mid.UI** — UI project (optional / not required to run the API)

## Prerequisites

- .NET SDK **9.x**
- SQL Server (any of):
    - LocalDB (Windows)
    - SQL Server Express
    - Full SQL Server instance

## Configuration

The repository currently keeps DB/JWT settings in **`AbySalto.Mid/appsettings.Development.json`**.

### appsettings.Development.json (example)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SERVER_NAME_HERE;Database=DATABASE_NAME_HERE;Trusted_Connection=True;TrustServerCertificate=true;"
  },
  "Jwt": {
    "Key": "DEV_ONLY_CHANGE_ME_TO_SOMETHING_LONGER_AND_RANDOM_32+CHARS",
    "Issuer": "AbySalto",
    "Audience": "AbySalto"
  }
}
```

Notes:
- `Jwt:Key` must be present (startup throws if missing).
- If your SQL Server uses TLS with a dev/self-signed cert, keep `TrustServerCertificate=true` or set up a trusted certificate chain.

## Database

EF Core migrations live in: **`AbySalto.Mid.Infrastructure/Migrations`**.

### Apply migrations

From the repository root with bash or powershell:

### bash
```bash
dotnet ef database update ^
  --project .\AbySalto.Mid.Infrastructure\AbySalto.Mid.Infrastructure.csproj ^
  --startup-project .\AbySalto.Mid\AbySalto.Mid.WebApi.csproj ^
  --context AppDbContext
```

### powershell
```powershell
dotnet ef database update `
  --project .\AbySalto.Mid.Infrastructure\AbySalto.Mid.Infrastructure.csproj `
  --startup-project .\AbySalto.Mid\AbySalto.Mid.WebApi.csproj `
  --context AppDbContext
```

## Run the API

From the repository root:

```bash
dotnet run --project .\AbySalto.Mid\AbySalto.Mid.WebApi.csproj
```

Or run its in VS or Rider.

By default (development), Swagger UI is hosted at the app root.

## Swagger / OpenAPI

When running in **Development**, the API enables Swagger UI and OpenAPI endpoints.

- Swagger UI: `https://localhost:<port>/`
- Swagger JSON: `https://localhost:<port>/swagger/v1/swagger.json`

### JWT auth in Swagger

1. `POST /api/auth/login` with email/password.
2. Copy the returned `token`.
3. Click **Authorize** in Swagger and paste:

```
Bearer <token>
```

## Error handling

The API uses a global exception middleware that returns **RFC7807 ProblemDetails**:

- `application/problem+json`
- includes `traceId`
- for `AppException`, includes `errorCode` when provided

Validation errors (FluentValidation + MVC model state) return `ValidationProblemDetails` with:
- `title: "Validation failed"`
- `errors` dictionary

## Main endpoints

### Auth
- `POST /api/auth/register` — register user (204 on success)
- `POST /api/auth/login` — login (200 `{ token }`)

### Products
- `GET /api/product` — paged list (query parameters via `GetProductsRequest`)
- `GET /api/product/{id}` — product by id
- `POST /api/product/import` — import products

### Basket (requires JWT)
- `POST /api/basket/me` — create basket for current user
- `GET /api/basket/{basketId}` — get basket
- `POST /api/basket/{basketId}/items` — add item
- `PUT /api/basket/{basketId}/items/{productId}` — update quantity
- `DELETE /api/basket/{basketId}/items/{productId}` — remove item

### Orders (requires JWT)
- `POST /api/orders` — place order for current user
- `GET /api/orders/me` — list my orders
- `GET /api/orders/{orderId}` — get order details
- `POST /api/orders/{basketId}/checkout` — checkout basket into an order

### Favorites (requires JWT)
- `GET /api/favorites` — list my favorites
- `POST /api/favorites/{productId}` — add favorite
- `DELETE /api/favorites/{productId}` — remove favorite

## Tests

Run all tests:

```bash
dotnet test
```

## Notes / conventions

- Controllers are intentionally thin; business decisions live in Application services.
- Expected business failures are raised as `AppException` and mapped to HTTP via middleware.
