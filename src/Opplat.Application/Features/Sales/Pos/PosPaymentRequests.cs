using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Opplat.Application.Abstractions.Invoicing;
using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Pos;
using Opplat.Application.Services;
using Opplat.Domain.Entities.Inventory;
using Opplat.Domain.Entities.Invoicing;
using Opplat.Domain.Entities.Sales;
using Opplat.Infrastructure.Persistance.Data;
using Opplat.Infrastructure.Services.Pos;

namespace Opplat.Application.Features.Sales.Pos;

public sealed record AddPaymentCommand(Guid SaleId, AddPaymentRequest Request, string? User) : ICommand<PosResult<PaymentDto>>;

public sealed class AddPaymentCommandHandler(
    OpplatDbContext dbContext,
    IPaymentTerminalProviderRegistry registry,
    IOptions<PosPaymentOptions> options)
    : ICommandHandler<AddPaymentCommand, PosResult<PaymentDto>>
{
    public async Task<PosResult<PaymentDto>> Handle(AddPaymentCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var sale = await dbContext.Sales.WithDetails().FirstOrDefaultAsync(s => s.Id == command.SaleId, cancellationToken);
        if (sale is null)
        {
            return PosResult<PaymentDto>.NotFound("Sale not found.");
        }

        if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
        {
            // A retried request returns the original payment instead of charging twice.
            var existing = await dbContext.PaymentAttempts
                .Include(a => a.SalePayment).ThenInclude(p => p!.Attempts)
                .FirstOrDefaultAsync(a => a.IdempotencyKey == request.IdempotencyKey, cancellationToken);
            if (existing?.SalePayment is not null)
            {
                return existing.SalePayment.SaleId == sale.Id
                    ? PosResult<PaymentDto>.Ok(PosSaleMapping.ToDto(existing.SalePayment))
                    : PosResult<PaymentDto>.Conflict("Idempotency key was used for a different sale.");
            }
        }

        if (sale.Status != SaleStatus.Draft)
        {
            return PosResult<PaymentDto>.Conflict("Payments can only be added to an open sale.");
        }

        if (sale.Payments.Any(p => p.Status == SalePaymentStatus.Pending))
        {
            return PosResult<PaymentDto>.Conflict("A payment is already in progress for this sale.");
        }

        var due = sale.TotalAmount - sale.Payments.Where(p => p.Status == SalePaymentStatus.Captured).Sum(p => p.Amount);
        if (due <= 0)
        {
            return PosResult<PaymentDto>.Conflict("The sale is already fully paid.");
        }

        return request.Method == SalePaymentMethod.Card
            ? await AddCardPaymentAsync(sale, request, due, command.User, cancellationToken)
            : await AddImmediatePaymentAsync(sale, request, due, command.User, cancellationToken);
    }

    private async Task<PosResult<PaymentDto>> AddImmediatePaymentAsync(
        Sale sale, AddPaymentRequest request, decimal due, string? user, CancellationToken cancellationToken)
    {
        var tendered = request.Amount ?? due;
        if (tendered <= 0)
        {
            return PosResult<PaymentDto>.Invalid("The amount must be greater than zero.");
        }

        var applied = Math.Min(tendered, due);
        if (request.Method == SalePaymentMethod.Cash)
        {
            var limit = options.Value.CashPaymentLimit;
            var cashSoFar = sale.Payments.Where(p => p.Method == SalePaymentMethod.Cash && p.Status == SalePaymentStatus.Captured).Sum(p => p.Amount);
            if (limit > 0 && cashSoFar + applied > limit)
            {
                return PosResult<PaymentDto>.Invalid($"Cash payments above {limit:0.00} EUR are not allowed (Ley 7/2012); take the rest by card.");
            }
        }

        var payment = new SalePayment
        {
            SaleId = sale.Id,
            Method = request.Method,
            Status = SalePaymentStatus.Captured,
            Amount = applied,
            TenderedAmount = tendered,
            ChangeGiven = tendered - applied,
            CreatedBy = user
        };
        dbContext.SalePayments.Add(payment);
        await dbContext.SaveChangesAsync(cancellationToken);
        return PosResult<PaymentDto>.Ok(PosSaleMapping.ToDto(payment));
    }

    private async Task<PosResult<PaymentDto>> AddCardPaymentAsync(
        Sale sale, AddPaymentRequest request, decimal due, string? user, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.IdempotencyKey))
        {
            return PosResult<PaymentDto>.Invalid("An idempotency key is required for card payments.");
        }

        var amount = request.Amount ?? due;
        if (amount <= 0 || amount > due)
        {
            return PosResult<PaymentDto>.Invalid("The card amount must be greater than zero and not exceed the amount due.");
        }

        var terminal = request.TerminalId is null
            ? null
            : await dbContext.PaymentTerminals.FirstOrDefaultAsync(t => t.Id == request.TerminalId && t.IsActive, cancellationToken);
        if (terminal is null)
        {
            return PosResult<PaymentDto>.Invalid("An active payment terminal is required for card payments.");
        }

        var provider = registry.Find(terminal.Provider);
        if (provider is null)
        {
            return PosResult<PaymentDto>.Invalid($"Payment provider '{terminal.Provider}' is not available.");
        }

        var payment = new SalePayment
        {
            SaleId = sale.Id,
            Method = SalePaymentMethod.Card,
            Status = SalePaymentStatus.Pending,
            Amount = amount,
            CreatedBy = user
        };
        var attempt = new PaymentAttempt
        {
            TerminalId = terminal.Id,
            Provider = terminal.Provider,
            IdempotencyKey = request.IdempotencyKey.Trim(),
            Amount = amount,
            Currency = sale.Currency,
            Status = PaymentAttemptStatus.Pending,
            RequestedAtUtc = DateTime.UtcNow,
            CreatedBy = user
        };
        payment.Attempts.Add(attempt);
        dbContext.SalePayments.Add(payment);

        // Persist before calling the terminal so a crash cannot leave a charge nobody knows about.
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            var result = await provider.StartPaymentAsync(
                new TerminalPaymentRequest(attempt.IdempotencyKey, terminal.ProviderTerminalId, amount, sale.Currency, sale.Id.ToString("N")),
                cancellationToken);
            PosSaleMapping.Apply(payment, attempt, result);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Unknown outcome: the cashier must check the terminal, not retry blindly.
            attempt.Status = PaymentAttemptStatus.Unknown;
            attempt.FailureReason = ex.Message;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return PosResult<PaymentDto>.Ok(PosSaleMapping.ToDto(payment));
    }
}

public sealed record GetPaymentQuery(Guid PaymentId) : IQuery<PosResult<PaymentDto>>;

/// <summary>Polling endpoint: asks the terminal for the latest state while the payment is pending.</summary>
public sealed class GetPaymentQueryHandler(OpplatDbContext dbContext, IPaymentTerminalProviderRegistry registry)
    : IQueryHandler<GetPaymentQuery, PosResult<PaymentDto>>
{
    public async Task<PosResult<PaymentDto>> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var payment = await dbContext.SalePayments.Include(p => p.Attempts)
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId, cancellationToken);
        if (payment is null)
        {
            return PosResult<PaymentDto>.NotFound("Payment not found.");
        }

        var attempt = payment.Attempts.OrderByDescending(a => a.RequestedAtUtc).FirstOrDefault();
        if (payment.Status == SalePaymentStatus.Pending && attempt?.ProviderReference is not null)
        {
            var provider = registry.Find(attempt.Provider);
            if (provider is not null)
            {
                var result = await provider.GetPaymentStatusAsync(attempt.ProviderReference, cancellationToken);
                PosSaleMapping.Apply(payment, attempt, result);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        return PosResult<PaymentDto>.Ok(PosSaleMapping.ToDto(payment));
    }
}

public sealed record CancelPaymentCommand(Guid PaymentId) : ICommand<PosResult<PaymentDto>>;

public sealed class CancelPaymentCommandHandler(OpplatDbContext dbContext, IPaymentTerminalProviderRegistry registry)
    : ICommandHandler<CancelPaymentCommand, PosResult<PaymentDto>>
{
    public async Task<PosResult<PaymentDto>> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await dbContext.SalePayments.Include(p => p.Attempts)
            .FirstOrDefaultAsync(p => p.Id == request.PaymentId, cancellationToken);
        if (payment is null)
        {
            return PosResult<PaymentDto>.NotFound("Payment not found.");
        }

        if (payment.Status != SalePaymentStatus.Pending)
        {
            return PosResult<PaymentDto>.Conflict("Only pending payments can be cancelled.");
        }

        var attempt = payment.Attempts.OrderByDescending(a => a.RequestedAtUtc).First();
        if (attempt.ProviderReference is null)
        {
            return PosResult<PaymentDto>.Conflict("The terminal has not acknowledged this payment yet; check the terminal before retrying.");
        }

        var provider = registry.Find(attempt.Provider);
        if (provider is null)
        {
            return PosResult<PaymentDto>.Invalid($"Payment provider '{attempt.Provider}' is not available.");
        }

        var result = await provider.CancelPaymentAsync(attempt.ProviderReference, cancellationToken);
        PosSaleMapping.Apply(payment, attempt, result);
        await dbContext.SaveChangesAsync(cancellationToken);
        return PosResult<PaymentDto>.Ok(PosSaleMapping.ToDto(payment));
    }
}

public sealed record ApplyTerminalWebhookCommand(string Provider, string Body, IReadOnlyDictionary<string, string> Headers)
    : ICommand<PosResult<bool>>;

public sealed class ApplyTerminalWebhookCommandHandler(OpplatDbContext dbContext, IPaymentTerminalProviderRegistry registry)
    : ICommandHandler<ApplyTerminalWebhookCommand, PosResult<bool>>
{
    public async Task<PosResult<bool>> Handle(ApplyTerminalWebhookCommand request, CancellationToken cancellationToken)
    {
        var provider = registry.Find(request.Provider);
        var webhook = provider?.ParseWebhook(request.Body, request.Headers);
        if (webhook is null)
        {
            return PosResult<bool>.Invalid("Invalid webhook.");
        }

        var attempt = await dbContext.PaymentAttempts
            .FirstOrDefaultAsync(a => a.Provider == provider!.Key && a.ProviderReference == webhook.ProviderReference, cancellationToken);
        if (attempt is null)
        {
            return PosResult<bool>.NotFound("Unknown payment reference.");
        }

        var payment = await dbContext.SalePayments.FirstAsync(p => p.Id == attempt.SalePaymentId, cancellationToken);
        PosSaleMapping.Apply(payment, attempt, webhook.Result);
        await dbContext.SaveChangesAsync(cancellationToken);
        return PosResult<bool>.Ok(true);
    }
}

public sealed record CompleteSaleCommand(Guid SaleId, CompleteSaleRequest? Request, string? User) : ICommand<PosResult<SaleDto>>;

/// <summary>
/// Finishes a paid sale: issues the simplified invoice (F2), takes stock and closes the sale atomically.
/// </summary>
public sealed class CompleteSaleCommandHandler(
    OpplatDbContext dbContext,
    IInvoiceIssuingService invoiceIssuingService)
    : ICommandHandler<CompleteSaleCommand, PosResult<SaleDto>>
{
    public async Task<PosResult<SaleDto>> Handle(CompleteSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await dbContext.Sales.WithDetails().FirstOrDefaultAsync(s => s.Id == command.SaleId, cancellationToken);
        if (sale is null)
        {
            return PosResult<SaleDto>.NotFound("Sale not found.");
        }

        // Double-clicks and client retries get the finished sale back instead of a second invoice.
        if (sale.Status == SaleStatus.Completed)
        {
            return PosResult<SaleDto>.Ok(await PosSaleMapping.ToDtoAsync(dbContext, sale, cancellationToken));
        }

        if (sale.Status != SaleStatus.Draft)
        {
            return PosResult<SaleDto>.Conflict("The sale is not open.");
        }

        var paid = sale.Payments.Where(p => p.Status == SalePaymentStatus.Captured).Sum(p => p.Amount);
        if (paid != sale.TotalAmount || sale.Payments.Any(p => p.Status == SalePaymentStatus.Pending))
        {
            return PosResult<SaleDto>.Conflict("The sale is not fully paid.");
        }

        var settings = await dbContext.TenantFiscalSettings.FirstOrDefaultAsync(cancellationToken);
        if (settings is null)
        {
            return PosResult<SaleDto>.Invalid("Tenant fiscal settings are required before selling.");
        }

        var customer = command.Request?.Customer;
        var hasCustomerTaxId = !string.IsNullOrWhiteSpace(customer?.TaxId);
        if (hasCustomerTaxId && string.IsNullOrWhiteSpace(customer!.Name))
        {
            return PosResult<SaleDto>.Invalid("A customer name is required for a full invoice.");
        }

        if (!hasCustomerTaxId && sale.TotalAmount > settings.SimplifiedInvoiceThreshold)
        {
            return PosResult<SaleDto>.Invalid($"Totals above {settings.SimplifiedInvoiceThreshold:0.00} EUR need the customer's tax data (full invoice).");
        }

        var register = await dbContext.PosRegisters.FirstOrDefaultAsync(r => r.Id == sale.RegisterId, cancellationToken);
        if (register is null)
        {
            return PosResult<SaleDto>.Invalid("The register of this sale no longer exists.");
        }

        var productIds = sale.Products.Select(d => d.ProductId).ToList();
        var products = await dbContext.ProductsForSale
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        var relational = dbContext.Database.IsRelational();
        var transaction = relational ? await dbContext.Database.BeginTransactionAsync(cancellationToken) : null;
        try
        {
            var invoice = BuildInvoice(sale, register, customer, hasCustomerTaxId);
            dbContext.Invoices.Add(invoice);

            var issued = await invoiceIssuingService.IssueAsync(invoice, settings, cancellationToken);
            if (!issued.Succeeded)
            {
                return PosResult<SaleDto>.Conflict(issued.Message);
            }

            await TakeStockAsync(sale, register, products, command.User, cancellationToken);

            sale.Status = SaleStatus.Completed;
            sale.CompletedAtUtc = DateTime.UtcNow;
            sale.ModifiedBy = command.User;
            await dbContext.SaveChangesAsync(cancellationToken);

            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }
        }
        finally
        {
            if (transaction is not null)
            {
                await transaction.DisposeAsync();
            }
        }

        return PosResult<SaleDto>.Ok(await PosSaleMapping.ToDtoAsync(dbContext, sale, cancellationToken));
    }

    private static Invoice BuildInvoice(Sale sale, PosRegister register, CustomerRequest? customer, bool hasCustomerTaxId)
    {
        var priced = PosPricing.Price(sale.Products.Select(d =>
            new PricingInput(d.ProductId, d.ProductName ?? string.Empty, d.UnitPrice, d.TaxRate, d.Quatity, d.DiscountAmount)));

        var methods = sale.Payments.Where(p => p.Status == SalePaymentStatus.Captured).Select(p => p.Method).Distinct().ToList();
        var invoice = new Invoice
        {
            Series = register.InvoiceSeries,
            FullNumber = string.Empty,
            Status = InvoiceStatus.Draft,
            IssueDate = DateTime.UtcNow,
            SaleId = sale.Id,
            Currency = sale.Currency,
            Subtotal = priced.Breakdowns.Sum(b => b.TaxableBase),
            TotalAmount = priced.Total,
            PaymentMethod = methods.Count > 1 ? "Mixto" : methods[0] switch
            {
                SalePaymentMethod.Cash => "Efectivo",
                SalePaymentMethod.Card => "Tarjeta",
                _ => "Otro"
            },
            CustomerSnapshot = hasCustomerTaxId
                ? new CustomerSnapshot { Name = customer!.Name.Trim(), TaxId = customer.TaxId!.Trim(), Address = customer.Address, Country = customer.Country, IsFinalConsumer = false }
                : new CustomerSnapshot { Name = "Cliente final", IsFinalConsumer = true }
        };

        foreach (var line in priced.Lines)
        {
            invoice.Lines.Add(PosPricing.ToInvoiceLine(line));
        }

        foreach (var breakdown in priced.Breakdowns)
        {
            invoice.TaxBreakdowns.Add(PosPricing.ToInvoiceBreakdown(breakdown));
        }

        return invoice;
    }

    private async Task TakeStockAsync(
        Sale sale,
        PosRegister register,
        IReadOnlyDictionary<Guid, ProductForSale> products,
        string? user,
        CancellationToken cancellationToken)
    {
        if (register.StorageId is null)
        {
            return;
        }

        foreach (var line in sale.Products)
        {
            if (!products.TryGetValue(line.ProductId, out var product) || product.InventoryProductId is not { } inventoryProductId)
            {
                continue;
            }

            dbContext.ProductMovements.Add(new ProductMovement
            {
                Date = DateTime.UtcNow,
                ProductId = inventoryProductId,
                StorageId = register.StorageId.Value,
                Quantity = line.Quatity,
                Type = MovementType.IndependentSale,
                Observations = $"POS sale {sale.Id}",
                User = user ?? string.Empty
            });

            var stock = await dbContext.Inventories
                .FirstOrDefaultAsync(i => i.ProductId == inventoryProductId && i.StorageId == register.StorageId, cancellationToken);
            if (stock is null)
            {
                dbContext.Inventories.Add(new ProductInventory
                {
                    ProductId = inventoryProductId,
                    StorageId = register.StorageId.Value,
                    Quantity = -line.Quatity,
                    UpdatedOn = DateTime.UtcNow,
                    User = user ?? string.Empty
                });
            }
            else
            {
                stock.Quantity -= line.Quatity;
                stock.UpdatedOn = DateTime.UtcNow;
                stock.User = user ?? string.Empty;
            }
        }
    }
}
