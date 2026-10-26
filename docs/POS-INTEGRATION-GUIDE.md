# POS Integration Guide

> **Version:** 1.0 | **Status:** Draft | **Last Updated:** 2026-10-26
> **Audience:** developers connecting third-party POS software or card-terminal providers to Opplat

All routes are under `/pos` on the main API, require `Authorization: Bearer <token>` and the `X-Tenant-Identifier` header. Errors use RFC 7807 problem details: `400` validation, `404` not found, `409` conflict (wrong state). Interactive docs: `/docs`.

## 1. Authentication for devices

1. In Keycloak create a confidential client with **service accounts enabled**.
2. Assign the service-account user the realm role `PosCashier` (add `PosManager` only if it must manage terminals).
3. Set the service-account user's attributes `tenant_id` and `tenant_identifier` so the token carries the tenant (same mappers as human users).
4. Request a token with the client-credentials grant and send it as a bearer token.

## 2. Selling

```
POST /pos/registers/{registerId}/shifts      {"openingFloat": 50}          open a shift
POST /pos/sales                              {"registerId": "...", "lines": [{"productId": "...", "quantity": 2}]}
POST /pos/sales/{saleId}/payments            {"method": "Cash", "amount": 20}
POST /pos/sales/{saleId}/payments            {"method": "Card", "terminalId": "..."}   + Idempotency-Key header
GET  /pos/payments/{paymentId}               poll until status is not Pending
POST /pos/sales/{saleId}/complete            {"customer": null}            issues the invoice
GET  /invoices/{invoiceId}/pdf               ticket
POST /pos/shifts/{shiftId}/close             {"countedCash": 120.5}        Z report
```

The sale total is computed by the server from the catalog (prices include VAT); a client-side total is only a preview.

Sale states: `Draft → Completed`, or `Draft → Voided`. A sale can be completed only when captured payments equal the total.

### Idempotency

Card payments **require** an `Idempotency-Key` header (or `idempotencyKey` in the body). Replaying the same key returns the original payment and never charges twice. Use a new key only for a genuinely new attempt after a failed one. `POST /sales/{id}/complete` is also safe to repeat: it returns the already completed sale.

### Payment states

| Attempt status | Meaning | What the client does |
|---|---|---|
| `Pending` | Terminal is waiting for the card | Poll `GET /pos/payments/{id}` every 1–2 s; offer cancel |
| `Approved` | Captured | Continue; sale `due` drops |
| `Declined` / `TimedOut` / `Cancelled` | Not charged | Show `failureReason`; allow a retry with a new key |
| `Unknown` | The terminal gave no answer | Do **not** retry blindly; check the terminal, then poll or cancel |

Cash: `amount` is the cash handed over; the server applies up to the amount due and returns `changeGiven`. Cash above `Pos:CashPaymentLimit` (default 1000 €) is rejected.

## 3. Adding a terminal provider

Implement `IPaymentTerminalProvider` (`Opplat.Application.Abstractions.Pos`) and register it as a singleton in `AddPosInfrastructure`:

| Method | Notes |
|---|---|
| `Key` | Lower-case id stored on `PaymentTerminal.Provider` |
| `StartPaymentAsync` | Idempotent on `IdempotencyKey`; normally returns `Pending` with a provider reference |
| `GetPaymentStatusAsync` / `CancelPaymentAsync` | By provider reference |
| `RefundAsync` | Used by the refund flow (not yet exposed) |
| `ParseWebhook` | Verify the provider signature; return `null` if not authentic |

Never persist or log card numbers; return only brand and last four digits.

### Webhooks

`POST /pos/webhooks/{provider}` (anonymous, signature-checked by the provider) with the `X-Tenant-Identifier` header. For the mock provider send `X-Mock-Signature` = upper-case hex HMAC-SHA256 of the raw body with `Pos:MockWebhookSecret`, and a body like:

```json
{ "providerReference": "MOCK-…", "status": "Approved", "authorizationCode": "123456", "cardBrand": "VISA", "cardLast4": "4242" }
```

## 4. Mock terminal for development

Register a terminal with provider `mock`. The cents of the amount choose the outcome: `.01` declined, `.02` never answers (cancel it), anything else is approved after `Pos:MockApprovalDelay` (default 3 s).

## 5. Configuration (`Pos` section)

| Key | Default | Purpose |
|---|---|---|
| `CashPaymentLimit` | `1000` | Max cash per sale (Ley 7/2012); `0` disables |
| `MockWebhookSecret` | `mock-terminal-secret` | Change outside development |
| `MockApprovalDelay` | `00:00:03` | Simulated card interaction time |
