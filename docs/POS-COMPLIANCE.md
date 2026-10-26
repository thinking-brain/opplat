# POS Compliance: simplified invoices (F2), VERI*FACTU and EU rules

> **Version:** 1.0 | **Status:** Draft, pending tax-adviser review | **Last Updated:** 2026-10-26
> **Scope:** `Opplat.Api.Pos` sales flow, `InvoiceIssuingService`, Verifactu hash/QR, ticket PDF
> **Not legal advice.** This document records how the code is designed to meet each rule and where the evidence lives. A tax adviser must confirm the legal conclusions before they are claimed to customers.

Legend: ✅ implemented and tested · 🟡 partial · 🔴 not implemented (blocks go-live in Spain)

## 1. What a POS sale produces

Completing a POS sale (`CompleteSaleCommandHandler`) runs **one database transaction**:

1. Check captured payments equal the sale total (cash change excluded).
2. Build an `Invoice` from the sale snapshot (tax-inclusive prices, per-rate breakdown).
3. `IInvoiceIssuingService.IssueAsync` takes the issuer's lock, assigns the number, links the hash chain, creates the fiscal record and queues it for AEAT.
4. Decrement stock and mark the sale `Completed`.

Any failure rolls everything back, so a sale never exists without its invoice and no invoice number is lost.

Invoice type is chosen by `DefaultInvoiceTypeResolver`:

| Situation | Result |
|---|---|
| No customer tax ID, total ≤ `SimplifiedInvoiceThreshold` (default 400 €) | **F2** simplified invoice |
| No customer tax ID, total above the threshold | POS refuses and asks for customer data |
| Customer supplies name + tax ID | **F1** full invoice |

## 2. Spain: VERI*FACTU (RD 1007/2023, Orden HAC/1177/2024)

Deadlines (AEAT note, RDL 15/2025): systems must be adapted before **2027-01-01** for corporate-tax taxpayers and before **2027-07-01** for everyone else. The Ministry announced on 2026-10-05 that a further extension is expected, so no date is hardcoded.

| Requirement | How it is met | Code | Test | Status |
|---|---|---|---|---|
| Billing record created for every invoice, in sequence | One `InvoiceFiscalRecord` per issued invoice, written in the same transaction as the invoice | `InvoiceIssuingService` | `PosFlowTests.CompleteSale_ChainsRecordsAndNumbersSequentially` | ✅ |
| Hash ("huella") SHA-256 over the fields in the AEAT order, `campo=valor&…`, UTF-8, upper-case hex, ISO-8601 timestamp with offset | `VerifactuHashCalculator.ComputeAlta` / `ComputeAnulacion` | `VerifactuHashCalculator` | `VerifactuHashCalculatorTests.ComputeAlta_MatchesAeatWorkedExample` reproduces the worked example `3C464DAF…F12F60`. The AEAT PDF could not be fetched automatically; confirm the vector against it. | ✅ |
| Hash chain per issuer, each record holds the previous hash | `ChainSequence` + `PreviousRecordHash`; read under a per-issuer Postgres advisory lock held to commit, unique filtered index on `ChainSequence` | `InvoiceIssuingService`, `InvoiceFiscalRecordConfiguration` | chain test above | ✅ |
| Correlative numbering, no gaps | Counter increments inside the issuing transaction (previously it committed separately). One series per register avoids contention. Numbering restarts per Madrid-local year; unique `(Series, FiscalYear, Number)` | `InvoiceCounterService`, `InvoiceConfiguration` | chain test | ✅ |
| Invoice dated in local (Spanish) time | `FiscalTime` converts to Europe/Madrid for the issue date, numbering year, hash, QR and ticket | `FiscalTime` | `QrPayload_UsesMadridLocalDateAroundMidnight` | ✅ |
| Issuer NIF identifies the record, not the customer | `InvoiceFiscalRecord.IssuerTaxId` from tenant settings; QR now uses it (it used the customer's before) | `VerifactuFiscalizationProvider` | `CompleteSale_WithCash_IssuesSimplifiedVerifactuInvoice` | ✅ |
| QR: URL with `nif`, `numserie`, `fecha`, `importe`; ISO/IEC 18004, level M; 30–40 mm; legend "VERI*FACTU" | `VerifactuQrCodeBuilder` (production and test URLs); ticket prints the QR at 99 pt (≈35 mm) with the legend | `VerifactuQrCodeBuilder`, `InvoicePdfRenderer` | `QrPayload_UsesIssuerNifAndAeatParameters` | ✅ (verify printed size on the real ticket printer) |
| Records are not altered after issue | Issued invoices cannot be cancelled through the old flag-flip (`CancelInvoiceCommand` now refuses); POS voids only unpaid drafts | `CancelInvoiceCommandHandler`, `VoidSaleCommandHandler` | covered by handler logic | 🟡 add a database/EF guard against updates and deletes |
| Real-time submission to AEAT (VERI*FACTU mode) | Records are created `Pending` and queued for `VerifactuSubmissionWorker` (outbox with retries and `SKIP LOCKED`) | `VerifactuSubmissionWorker` | `VerifactuWorkerOutboxTests` | 🔴 `VerifactuSubmissionClient` still throws `NotImplementedException`: XML RegistroAlta, SOAP + mTLS with the tenant's certificate, response handling and wait-time are missing |
| Anulación records (erroneously generated invoices) | Hash calculation exists; record type, XML and flow do not | `ComputeAnulacion` | hash test only | 🔴 |
| Refunds / returns | Must be a rectifying invoice **R5** (simplified), not a deletion | none yet | none | 🔴 |
| Full invoice requested after a ticket was issued | Must be **F3** substituting the F2 | none yet | none | 🔴 (at the till the customer can already ask for F1 before paying) |
| Software identification block (`SistemaInformatico`: producer NIF, name, 2-char id, version, installation number) and producer's responsible declaration | Name/version/licence stored on each record; the formal block, declaration and per-tenant installation number are not | `InvoiceFiscalRecord` | none | 🔴 |
| Event log | Required only in the non-VERI*FACTU mode; the POS targets VERI*FACTU mode | n/a | n/a | n/a |
| Retention | Records and invoices are never deleted by the application; keep at least 6 years (Código de Comercio art. 30; tax limitation period 4 years) | n/a | n/a | 🟡 backup/retention policy to define |

## 3. Spain: content of the simplified invoice (RD 1619/2012)

Simplified invoices are allowed when the total (VAT included) is within the limit (400 €, or 3,000 € for the sectors listed in art. 4 such as retail and hospitality) and the customer does not ask for a full invoice or is not a business needing one. The threshold is a tenant setting; **the sector rule must be confirmed per tenant**.

Required ticket content (art. 7) and where it comes from:

| Item | Source on the ticket |
|---|---|
| Series and number | `T1-000123` (register series + sequence) |
| Issue date | Madrid-local date |
| Issuer name and NIF | `TenantFiscalSettings.LegalName` / `TaxId` |
| Description of goods | line names (snapshot at sale time) |
| Tax rate and "IVA incluido" | per-rate base/quota block and the "IVA incluido" line |
| Total price | gross total |

Test: `InvoicePdfRenderer.RenderSimplified` is covered visually; QuestPDF compresses text, so assertions on PDF text need a PDF text-extraction library (not yet added).

## 4. Other Spanish rules built into the POS

| Rule | Behaviour |
|---|---|
| Cash limit, Ley 7/2012 (1,000 € when a business is a party) | `AddPaymentCommandHandler` rejects cash above `Pos:CashPaymentLimit` (default 1000) per sale. Test: `CashPayment_AboveLegalLimit_IsRejected` |
| Prices include VAT; customer pays exactly the sum of line grosses | `PosPricing` extracts base and quota per rate so base + quota = amount charged. Test: `Pricing_ExtractsTaxFromTaxInclusivePrices` |

## 5. EU rules

| Rule | Position |
|---|---|
| VAT Directive 2006/112/EC, arts. 220–238 (invoicing, simplified invoices) | Same content as the Spanish rules above; Spain transposes with RD 1619/2012 |
| PSD2 (Directive 2015/2366) strong customer authentication | Performed by the card terminal/acquirer (EMV chip+PIN, contactless limits). Opplat never handles card data, so it cannot weaken SCA |
| PSD2 art. 62(4) / Interchange Regulation 2015/751: no surcharge on consumer cards | The POS never adds a card surcharge |
| PCI DSS | Only brand and last four digits are stored (`PaymentAttempt.CardBrand/CardLast4`). The terminal and PSP own PAN handling; use P2PE-validated terminals. A merchant questionnaire is still the tenant's duty |
| GDPR | F2 tickets carry no personal data. Customer data exists only on a full invoice, limited to name, tax ID and address needed by law |
| European Accessibility Act (Directive 2019/882, in force since 2025-06-28) | Applies to payment terminals; terminal vendors are responsible. The POS web UI uses labelled buttons and large touch targets; a formal accessibility audit is pending |
| Other member states' fiscal-device regimes (DE TSE, FR NF525, IT, PT, PL …) | Not implemented. `IInvoiceFiscalizationProvider` is the extension point; non-Spanish tenants currently get the plain hash-chained receipt (`NullInvoiceFiscalizationProvider`) |
| EN 16931 e-invoicing / ViDA | B2C tickets are out of scope; B2B e-invoicing is a separate project |

## 6. Go-live blockers (Spain)

1. Implement AEAT submission (XML, SOAP/mTLS, certificate custody) and test against `preportal.aeat.es`.
2. R5 refunds and F3 substitution.
3. `SistemaInformatico` block, responsible declaration and installation number.
4. Database/EF guard against modifying issued records.
5. Verify the hash vector and QR parameters against the AEAT PDFs; print a real ticket and measure the QR.
6. Tax-adviser review of this document, including the 400 € / 3,000 € rule per tenant.
