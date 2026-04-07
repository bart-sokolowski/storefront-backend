# Storefront Backend

ASP.NET Core 10 Web API for the Spacehive storefront assessment.

## Prerequisites

- [.NET 10 SDK]

## Run Instructions

1. Clone the repository.

2. Create `appsettings.json` inside `StorefrontApi/StorefrontApi/`:

```json
{
  "Jwt": {
    "Secret": "your-secret-key",
    "Issuer": "StorefrontApi",
    "Audience": "StorefrontClient"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:4200"]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

3. Run the API:

```bash
cd StorefrontApi/StorefrontApi
dotnet run
```

The API starts on `http://localhost:5090` by default.

## Test Accounts

| Email | Password | Role |
|---|---|---|
| `admin@store.com` | `password123` | Admin |
| `user@store.com` | `password123` | User |

Credentials are hardcoded in `Services/AuthService.cs` for assessment purposes.

## API Overview

| Method | Route | Auth |
|---|---|---|
| POST | `/api/auth/login` | None |
| GET | `/api/products` | None |
| POST | `/api/products` | Admin |
| DELETE | `/api/products/{id}` | Admin |
| GET | `/api/orders` | User / Admin |
| GET | `/api/orders/{id}` | User / Admin |
| POST | `/api/orders` | User |
| WS | `/hubs/notifications` | User (JWT via query string) |

---

## Key Technical Decisions

### ApiResult\<T\> Response Wrapper
All endpoints return `ApiResult<T>` with `IsSuccess`, `Data`, `Note`, and `Errors` fields, always with HTTP 200. Unhandled exceptions are the only case where a non-200 status is returned, via `ProblemDetails` through `GlobalExceptionMiddleware`. This keeps client-side error handling uniform — the HTTP status code distinguishes infrastructure failures from business outcomes.

### In-Memory Repositories with ConcurrentDictionary
Repositories use `ConcurrentDictionary` singletons instead of a database. This satisfies the assessment scope without introducing a database dependency, keeps setup to a single `dotnet run`, and still exercises the repository pattern with a realistic interface contract.

### JWT with Role Claims
Authentication issues a signed JWT containing `ClaimTypes.Email` and `ClaimTypes.Role`. The API derives all authorisation decisions from these two claims — no session state, no database lookups per request. The frontend decodes the payload client-side to derive UI permissions.

### Async Payment via System.Threading.Channels
Order payment is processed asynchronously through an unbounded `Channel<PaymentMessage>`. `MockPaymentService` writes to the channel and immediately returns a `Pending` result. `PaymentProcessorService` (a `BackgroundService`) reads from the channel, simulates a delay of 1.5–3.5 seconds, and randomly confirms or fails the payment in the 7/3 ratio. This mirrors an Azure Service Bus producer/consumer pattern without external infrastructure.

### SignalR for Real-Time Notifications
After the background processor settles a payment, it pushes a `PaymentStatusUpdated` event to the user's SignalR group (keyed by email). Because WebSocket connections cannot send custom headers, the JWT is passed as the `access_token` query parameter and extracted in `OnMessageReceived`. The hub uses `[Authorize]` so unauthenticated connections are rejected before `OnConnectedAsync`.

### Idempotency Keys on Order Creation
`POST /api/orders` requires an `Idempotency-Key` header. The repository maintains a secondary `ConcurrentDictionary<string, Guid>` index mapping keys to order IDs. Duplicate submissions within the same process lifetime return the existing order rather than creating a new one, preventing double charges on retries.

### Soft Delete for Products
Deleting a product sets its status to `Archived` rather than removing the record and is then filtered out from the products list. This preserves the product snapshot embedded in historical orders so that order history remains accurate even after a product is removed from the storefront.

### FluentValidation via Service Abstraction
Validators are resolved through `IValidationService`, which wraps `IValidator<T>` resolution. It uses RuleForEach feature for validating each element of a collection, it validates every item in the order's and product's items list independently.

---

## Trade-offs

- **State is not persistent.** Restarting the API clears all orders and restores the seeded product inventory. A production system would use a database with proper transactions.
- **Users are hardcoded.** There is no registration flow; credentials live in `AuthService`.
- **Stock is decremented on order creation or put back to initial state on a failed transaction.** 
- **No JWT refresh tokens.** The issued token has a fixed expiry. Clients must re-login when it expires.
- **Payment success rate is fixed at 70%.** This is sufficient to demonstrate both the confirmed and failed paths.

---

## Assumptions

- All prices are in GBP (no multi-currency support).
- Each checkout creates a single order containing all cart items.
- Regular users can only view their own orders; admins see all orders.
- Concurrent stock updates are safe because `ConcurrentDictionary` with `AddOrUpdate` provides atomic increments/decrements within a single process.
