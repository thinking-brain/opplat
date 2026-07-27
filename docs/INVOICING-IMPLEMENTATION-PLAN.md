# Plan: Invoicing Feature (Full + Simplified) with Verifactu Compliance

## Context gathered
- No Invoice/Billing/Customer/Tax entity exists today. Sales module (`src/Opplat.Domain/Entities/Sales/Sale.cs`) is minimal: Date, Products (SaleDetail), TotalAmount, Discounts. No customer, no tax, no numbering.
- Tenant entity (`src/Opplat.Domain/Entities/Administration/Tenant.cs`) lives in central `AdminTenantCatalogDbContext` (catalog DB) - no fiscal fields (NIF, legal name, address).
- Per-tenant business data (Sales, Inventory, Catalog) lives in `OpplatDbContext` (`src/Opplat.Infrastructure/Persistance/Data/OpplatDbContext.cs`), schema-isolated per tenant via Finbuckle.MultiTenant (`X-Tenant-Identifier` header).
- Architecture: Domain -> Application.Abstractions -> Application (mediator handlers under `Features/{Area}`, using the custom `IMediator`/`IRequestHandler` in `Opplat.Application.Abstractions.Messaging` — MediatR has been replaced) -> Infrastructure (EF configs, repos, services) -> Api.{Area} (minimal APIs) -> Api.Main aggregates via `RegisterEndpoints.cs`.
- Reference feature to clone: Sales (`Sale.cs`, `SaleConfiguration.cs`, `SalesEndpoints.cs`, `SalesServiceCollectionExtensions.cs`, `ListSalesQuery`/`CreateSaleCommand` in `Opplat.Application/Features/Sales/Sales/SaleRequests.cs`).
- `BaseRepository<T>` (`src/Opplat.Infrastructure/Persistance/Repositories/BaseRepository.cs`) gives generic Create/Delete; follow this for `IInvoiceRepository` if needed, or use mediator handlers directly against DbContext like existing Sales handlers.
- No PDF, QR, XML-signature, or crypto/hash libraries present. No FluentValidation. No background job/queue infra (Aspire.Hosting present but no Quartz/Hangfire).
- Frontend `opplat-react` convention: `src/pages/{Feature}Page.tsx` + `src/api/{feature}.api.ts` (axios-based, see `sales.api.ts`, `SellPage.tsx`). No invoicing UI exists.
- Verifactu official docs (`sede.agenciatributaria.gob.es`) link to PDFs (hash algorithm, QR spec, XML schemas, WSDL, electronic signature spec) that our tooling could NOT extract (PDF text extraction failed). Byte-exact field concatenation for the hash and exact QR URL/params MUST be manually verified against those PDFs before production go-live - built from public/general knowledge of Verifactu (RD 1007/2023, Ley Antifraude 11/2021) below, flagged as a verification task.

## Verifactu domain knowledge applied
- Two compliance modes for Spanish "Sistemas Informáticos de Facturación" (SIF): **VERI*FACTU** (near real-time XML submission of each invoice record to AEAT via SOAP web service) and **non-Verifactu SIF** (local hash-chain + mandatory electronic signature (XAdES), records kept and provided to AEAT only on request).
- Every issued invoice produces an immutable **billing record** ("registro de facturación") containing: issuer NIF, invoice series+number, issue date, invoice type (F1 full, F2 simplified, F3 substitutive of simplified, R1-R5 rectifying), tax breakdown per rate, total amount, recipient identification (only for full invoices), hash of the **previous** record (chaining), hash of this record (SHA-256 over a canonical field concatenation), generation timestamp, and identification of the software (name/version/license or "número de instalación").
- **Simplified invoice (F2 / "factura simplificada")**: allowed when total ≤ €400 (VAT incl.) or in qualifying sectors (retail, hospitality/restaurants/bars, hairdressing/barbershops, transport, parking, tolls, etc.). Recipient tax ID/name NOT required. This directly matches the user's "barbershop/restaurant" scenario.
- **Full invoice (F1 / "factura completa")**: required when recipient needs to deduct VAT (B2B) or requests it, or thresholds/sector rules don't allow simplified. Requires recipient name + NIF/VAT + address.
- Invoices/records must be gapless and sequential per series - **never deleted**, only voided via rectifying invoices (R1-R5) or marked cancelled with an audit trail.
- Printed/PDF invoices in VERI*FACTU mode must show the legend "VERI*FACTU" and a QR code linking to an AEAT verification/cotejo endpoint encoding issuer NIF, number, date, and total; non-Verifactu invoices show a different legend ("factura verificable en la sede electrónica de la AEAT").
- Deadlines have been extended per the site's "extension of deadline" notice (as of 2026-07-27) - implementation should not hardcode a hard go-live date; make submission mode configurable per tenant.

## Decisions (proceeding autonomously - user unavailable to confirm; documented for review)
- **Submission mode**: Design a pluggable `IFiscalizationProvider` abstraction with a per-tenant config toggle (`Verifactu` real-time submission vs `NonVerifactuSigned` local signing vs `None`/generic for non-Spain tenants). Implement the Verifactu record/hash/QR generation fully; implement AEAT SOAP submission against the **pre-production/sandbox** endpoint behind a feature flag; implement the XAdES signing service as a stub (`NotImplementedException` + TODO) since certificate management is out of MVP scope.
- **Frontend scope**: `opplat-react` only (tenant-facing app), following `pages/{Feature}Page.tsx` + `api/{feature}.api.ts` convention. No `opplat-admin` changes in this plan.
- **Invoice source**: Primary flow is Sale → Invoice (issue an invoice from a completed `Sale`). Also allow standalone invoice creation (needed for services with no prior Sale record, e.g. quick manual invoice) via the same command with an optional `SaleId`.
- **PDF library**: QuestPDF (Community license), added to `Directory.Packages.props`.
- **Digital certificate / signing**: Out of scope for this MVP; interface exists (`IInvoiceSigningService`) but implementation stubbed. Tenant cert upload/storage is a follow-up phase.
- **International extensibility**: `IFiscalizationProvider` abstraction lets other countries plug in later (e.g., Italy SDI/FatturaPA, France Factur-X) without redesigning the Invoice domain model; only `VerifactuFiscalizationProvider` and a `NullFiscalizationProvider` (default, no external compliance) are implemented now.
- Invoices are **immutable once issued**: no update/delete endpoints post-issue; corrections go through rectifying invoices.

## Steps

### Phase 1 - Domain model & persistence (no dependencies)
1. Create `src/Opplat.Domain/Entities/Invoicing/` with: `Invoice.cs` (BaseEntity: Series, Number, FullNumber, IssueDate, InvoiceType enum {Simplified, Full, Rectifying, Substitutive}, Status enum {Draft, Issued, Sent, Cancelled}, SaleId?, CustomerId?, CustomerSnapshot value object (Name, TaxId, Address, Country, IsFinalConsumer), Currency, Subtotal, TotalAmount, PaymentMethod, Notes), `InvoiceLine.cs` (InvoiceId, ProductId?, Description, Quantity, UnitPrice, DiscountAmount, TaxRate, LineTotal), `InvoiceTaxBreakdown.cs` (InvoiceId, TaxType enum {IVA, IGIC, IPSI, Exempt}, Rate, TaxableBase, TaxAmount), `Customer.cs` (Name, TaxId?, Email?, Address?, Country?, IsFinalConsumer), `InvoiceCounter.cs` (Series, Year, LastNumber - unique per tenant+series+year), `InvoiceFiscalRecord.cs` (InvoiceId 1:1, PreviousRecordHash?, RecordHash, HashInput, GeneratedAtUtc, SoftwareName/Version/LicenseId, SubmissionMode enum {Verifactu, NonVerifactuSigned, None}, SubmissionStatus enum {NotApplicable, Pending, Submitted, Accepted, Rejected}, AeatCsv?, AeatSubmittedAtUtc?, AeatResponseRaw?, QrCodePayload, SignatureValue?), `TenantFiscalSettings.cs` (one row per tenant schema: LegalName, TaxId/NIF, FiscalAddress, Country, BusinessSector, DefaultSeries, InvoicingMode, SimplifiedInvoiceThreshold, SoftwareLicenseId).
2. *Parallel with 1*: Add EF Core configurations under `src/Opplat.Infrastructure/Persistance/Configurations/Invoicing/` mirroring `SaleConfiguration.cs` (indexes: unique on `(Series, Number)` per tenant; unique on `(Series, Year)` for `InvoiceCounter`).
3. *Depends on 1-2*: Register new `DbSet<>`s in `OpplatDbContext.cs`, generate EF Core migration `..._InitialInvoicing.cs` under `src/Opplat.Infrastructure/Persistance/Migrations/` (follow `20260401180034_InitialSales.cs` naming/timestamp convention).
4. *Depends on 1*: Extend `Sale.cs` minimally if needed to expose a link back to its Invoice(s) (e.g., `ICollection<Invoice>` nav or just rely on `Invoice.SaleId`) - prefer the latter to avoid touching Sales domain heavily.

### Phase 2 - Numbering & tax calculation services (depends on Phase 1)
5. Create `IInvoiceCounterService` (Application.Abstractions) + `InvoiceCounterService` (Infrastructure) implementing gapless sequential numbering per tenant+series+year using a DB transaction with row-level locking (raw SQL `SELECT ... FOR UPDATE` via `ExecuteSqlRaw`/`FromSqlRaw` on Postgres) to prevent race conditions producing duplicate/skipped numbers under concurrent invoice issuance.
6. Create `ITaxCalculationService` / `TaxCalculationService` (Application) to compute per-line tax and aggregate `InvoiceTaxBreakdown` from `InvoiceLine.TaxRate` groups.
7. Create `IInvoiceTypeResolver` to decide Simplified vs Full based on `TenantFiscalSettings` (sector, threshold) and whether recipient tax data was supplied - encapsulates the Verifactu simplified/full business rule so it's testable in isolation.

### Phase 3 - Verifactu compliance engine (depends on Phase 1, parallel with Phase 2)
8. Define `IFiscalizationProvider` (Application.Abstractions/Invoicing/): `GenerateRecordAsync(Invoice, InvoiceFiscalRecord? previous)`, `SubmitAsync(InvoiceFiscalRecord)`, `BuildQrPayload(Invoice, InvoiceFiscalRecord)`.
9. Implement `VerifactuFiscalizationProvider` (`src/Opplat.Infrastructure/Services/Invoicing/Verifactu/`):
   - `VerifactuHashCalculator.cs` - builds canonical string (issuer NIF, series+number, issue date, invoice type, total tax, total amount, previous hash, generation timestamp) and computes SHA-256 hex digest. **Mark with a TODO/XML doc comment to verify exact field order/format against the AEAT "huella/hash" spec PDF before production use** (tooling could not extract PDF text during planning).
   - `VerifactuQrCodeBuilder.cs` - builds AEAT verification URL + renders QR image (add `ZXing.Net` or `QRCoder` package). Mark same verification TODO for exact URL/param spec.
   - `VerifactuSubmissionClient.cs` - SOAP/HTTP client stub targeting AEAT pre-production endpoint, behind `VerifactuOptions.Enabled` feature flag; writes `AeatCsv`/`AeatResponseRaw`/`SubmissionStatus`.
   - `IInvoiceSigningService` + stub `NotImplementedInvoiceSigningService` for the non-Verifactu XAdES signing path (explicitly out of scope, documented).
10. Implement `NullFiscalizationProvider` (default for non-Spain tenants / `InvoicingMode = None`) - generates the record locally without AEAT-specific fields, for international extensibility.
11. *Depends on 9*: Add `VerifactuSubmissionWorker : BackgroundService` (Infrastructure) polling `InvoiceFiscalRecord` rows with `SubmissionStatus = Pending`, submitting asynchronously with retry/backoff, marking `Rejected` after N attempts for manual resubmission.

### Phase 4 - PDF generation & delivery (depends on Phase 1, parallel with Phase 2-3)
12. Add `QuestPDF` to `Directory.Packages.props` and reference in `Opplat.Infrastructure.csproj`.
13. Create `InvoicePdfRenderer.cs` (Infrastructure/Services/Invoicing) with two layouts: full invoice (issuer + recipient blocks, line items, tax breakdown, QR + legend) and simplified/ticket-style (no recipient block, compact layout suitable for barbershop/restaurant).
14. Create `InvoiceEmailService.cs` reusing existing `mailkit` (4.17.0) setup pattern to send the PDF as an attachment.

### Phase 5 - Application layer & API (depends on Phases 1-4 for full functionality, but command/query skeletons can start once Phase 1 lands)
15. Create `src/Opplat.Application/Features/Invoicing/` commands/queries mirroring `SaleRequests.cs` pattern: `CreateInvoiceCommand` (from `SaleId` or standalone), `IssueInvoiceCommand` (allocates number via `IInvoiceCounterService`, resolves type via `IInvoiceTypeResolver`, calls `IFiscalizationProvider.GenerateRecordAsync`, persists, enqueues submission), `CancelInvoiceCommand` (creates rectifying invoice, never hard-deletes), `GetInvoiceQuery`, `ListInvoicesQuery` (filters: date range, status, series, customer), `GetInvoicePdfQuery`, `SendInvoiceEmailCommand`, `GetTenantFiscalSettingsQuery`/`UpsertTenantFiscalSettingsCommand`. Use an `InvoiceCommandResult` type mirroring `SalesCommandResult` (`src/Opplat.Application/Features/Sales/Common/SalesCommandResult.cs`).
16. Create `AddInvoicingApplication()` DI extension (Application/DependencyInjection) registering mediator handlers (via `services.AddRequestHandlersFromAssembly(...)`) + services, following `SalesServiceCollectionExtensions.cs`.
17. Create new `src/Apis/Opplat.Api.Invoicing/` project (csproj + `Program.cs` + `Endpoints/InvoicingEndpoints.cs` + `Extensions/`) mirroring `Opplat.Api.Sales` structure exactly: `MapInvoicingEndpoints()` with `MapGroup("/invoices")` for CRUD/issue/pdf/send/cancel and `MapGroup("/invoicing/settings")` for tenant fiscal settings. Add project to `opplat.slnx`.
18. Wire into `src/Apis/Opplat.Api.Main/Extensions/RegisterEndpoints.cs` (`app.MapInvoicingEndpoints()`) and `Program.cs` DI registration (`AddInvoicingApplication()`), matching how Sales/Catalog are wired today.

### Phase 6 - Frontend (opplat-react) (depends on Phase 5 endpoints existing, can build UI against mocked API earlier)
19. Create `src/opplat-react/src/api/invoices.api.ts` (axios calls: list, get, create, issue, downloadPdf, sendEmail, cancel) following `sales.api.ts` conventions (`axiosClient.ts`, `tenantPath.ts`).
20. Create `src/opplat-react/src/pages/InvoicesPage.tsx` (list + filters + detail/PDF preview + issue/send/cancel actions) and `src/opplat-react/src/pages/InvoiceSettingsPage.tsx` (tenant fiscal profile form: legal name, NIF, address, invoicing mode, default series, simplified threshold/sector). Register routes in `App.tsx` alongside existing pages.
21. *Depends on 19*: Add "Generate Invoice" action on the existing `SellPage.tsx` flow so a completed Sale can be converted directly into a (simplified or full) invoice.

### Phase 7 - Testing & compliance verification (depends on Phases 1-6, incremental testing alongside each phase preferred)
22. Unit tests under `test/Opplat.UnitTest/Invoicing/` (new folder) following `CreateTenantUserCommandTests.cs` pattern (in-memory `OpplatDbContext` + Moq): counter service concurrency/gaplessness, `InvoiceTypeResolver` simplified-vs-full rules, `VerifactuHashCalculator` deterministic output + chain linkage, command handlers (create/issue/cancel).
23. Add `Routing/InvoicingEndpointSurfaceTests.cs` (mirroring `EndpointSurfaceTests.cs`) to assert the new endpoints are registered/secured.
24. Manual/compliance verification: download and read the actual AEAT PDFs (hash algorithm, QR spec, XML record layouts, WSDL) to confirm exact byte-level formats before enabling real submission; validate against AEAT's external test portal (`https://preportal.aeat.es/`) mentioned on the technical info page.

## Relevant files
- `src/Opplat.Domain/Entities/Sales/Sale.cs` - reference entity pattern; add `SaleId` link from `Invoice`.
- `src/Opplat.Domain/Entities/Administration/Tenant.cs` - central tenant entity (NOT modified; fiscal data lives in new tenant-schema `TenantFiscalSettings` instead, to avoid cross-DbContext coupling).
- `src/Opplat.Infrastructure/Persistance/Data/OpplatDbContext.cs` - add new `DbSet<Invoice>`, `DbSet<InvoiceLine>`, `DbSet<InvoiceTaxBreakdown>`, `DbSet<Customer>`, `DbSet<InvoiceCounter>`, `DbSet<InvoiceFiscalRecord>`, `DbSet<TenantFiscalSettings>`.
- `src/Opplat.Infrastructure/Persistance/Configurations/Sales/SaleConfiguration.cs` - pattern for new `Configurations/Invoicing/*Configuration.cs`.
- `src/Opplat.Infrastructure/Persistance/Repositories/BaseRepository.cs` - reuse if a repository abstraction is preferred over direct DbContext access in handlers.
- `src/Opplat.Application/Features/Sales/Sales/SaleRequests.cs` - command/query pattern to mirror for Invoicing.
- `src/Opplat.Application/Features/Sales/Common/SalesCommandResult.cs` - result type pattern.
- `src/Opplat.Application/DependencyInjection/SalesServiceCollectionExtensions.cs` - DI registration pattern.
- `src/Apis/Opplat.Api.Sales/Endpoints/SalesEndpoints.cs` - minimal API pattern to mirror in new `Opplat.Api.Invoicing`.
- `src/Apis/Opplat.Api.Main/Extensions/RegisterEndpoints.cs` - wire `MapInvoicingEndpoints()`.
- `Directory.Packages.props` - add QuestPDF, a QR code library (ZXing.Net or QRCoder).
- `opplat.slnx` - add new `Opplat.Api.Invoicing` project.
- `src/opplat-react/src/api/sales.api.ts`, `src/opplat-react/src/pages/SellPage.tsx` - frontend patterns to mirror.
- `test/Opplat.UnitTest/Auth/CreateTenantUserCommandTests.cs`, `test/Opplat.UnitTest/Routing/EndpointSurfaceTests.cs` - test patterns to mirror.

## Verification
1. `dotnet build` (via the `build` task) succeeds after each phase's project changes.
2. Run `test/Opplat.UnitTest` suite (`dotnet test`) - new Invoicing tests pass, including gapless-numbering concurrency test and hash-chain determinism test.
3. Manually issue a simplified invoice (no customer) and a full invoice (with customer NIF) via the new endpoints/UI; confirm PDF renders correct legend + QR, and `InvoiceFiscalRecord` chain links to the prior record's hash.
4. Confirm architecture tests (`test/Opplat.UnitTest/Architecture/ApplicationLayerBoundaryArchitectureTests.cs`) still pass with new layer additions (no illegal cross-layer references).
5. Before enabling live AEAT submission: validate generated XML/hash against AEAT's external test portal (`https://preportal.aeat.es/`) and the official PDFs linked from the technical information page.

## Further Considerations
1. **Exact Verifactu byte-level formats (hash concatenation string, QR URL params, XML schema)** could not be extracted from AEAT's PDFs by available tooling. Recommendation: during Phase 3 implementation, manually download and transcribe the specs from `sede.agenciatributaria.gob.es/.../algoritmo-calculo-codificacion-huella-hash.html` and `.../caracteristicas-qr-especificaciones-servicio-cotejo-factura.html` PDFs, or use the AEAT-provided WSDL/XSD (`.../esquemas.html`, `.../wsdl-servicios-web.html`) directly as the source of truth, rather than relying on this plan's summarized description.
2. **Rectifying/substitutive invoices (R1-R5, F3)** are modeled in the `InvoiceType` enum now (to avoid a breaking migration later) but full rectification workflow (linking to original invoice, partial vs full rectification reasons) is deliberately deferred to a follow-up phase - flag if the user wants it in this same effort. Option A (recommended): reserve enum values only, implement workflow later. Option B: implement full rectification workflow now (adds ~3-4 more steps).
3. **Certificate-based signing (XAdES) for non-Verifactu mode** is stubbed only. If tenants need this legal fallback path before real-time AEAT submission is production-ready, certificate upload/storage and signing must be prioritized as an explicit next phase.
