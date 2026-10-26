using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Abstractions.Pos;
using Opplat.Application.Services;
using Opplat.Domain.Entities.Sales;
using Opplat.Infrastructure.Persistance.Data;

namespace Opplat.Application.Features.Sales.Pos;

internal static class PosSaleMapping
{
    public static IQueryable<Sale> WithDetails(this IQueryable<Sale> query)
        => query.Include(sale => sale.Products).Include(sale => sale.Payments).ThenInclude(payment => payment.Attempts);

    public static AttemptDto ToDto(PaymentAttempt attempt)
        => new(attempt.Id, attempt.Status, attempt.Provider, attempt.ProviderReference, attempt.AuthorizationCode, attempt.CardBrand, attempt.CardLast4, attempt.FailureReason, attempt.CustomerReceiptText);

    public static PaymentDto ToDto(SalePayment payment)
    {
        var latest = payment.Attempts.OrderByDescending(a => a.RequestedAtUtc).FirstOrDefault();
        return new PaymentDto(payment.Id, payment.SaleId, payment.Method, payment.Status, payment.Amount, payment.TenderedAmount, payment.ChangeGiven, latest is null ? null : ToDto(latest));
    }

    public static async Task<SaleDto> ToDtoAsync(OpplatDbContext dbContext, Sale sale, CancellationToken cancellationToken)
    {
        var invoice = await dbContext.Invoices
            .Where(i => i.SaleId == sale.Id)
            .Select(i => new { i.Id, i.FullNumber })
            .FirstOrDefaultAsync(cancellationToken);

        var paid = sale.Payments.Where(p => p.Status == SalePaymentStatus.Captured).Sum(p => p.Amount);
        return new SaleDto(
            sale.Id,
            sale.Status,
            sale.Date,
            sale.RegisterId,
            sale.ShiftId,
            sale.CashierUserName,
            sale.Currency,
            sale.TotalAmount,
            paid,
            sale.TotalAmount - paid,
            sale.Products
                .Select(d => new SaleLineDto(d.ProductId, d.ProductName ?? string.Empty, d.Quatity, d.UnitPrice, d.TaxRate, d.DiscountAmount, d.Amount))
                .ToList(),
            sale.Payments.OrderBy(p => p.CreatedAt).Select(ToDto).ToList(),
            invoice?.Id,
            invoice?.FullNumber);
    }

    /// <summary>Folds a terminal result into the attempt and the payment it belongs to.</summary>
    public static void Apply(SalePayment payment, PaymentAttempt attempt, TerminalPaymentResult result)
    {
        attempt.Status = result.Status;
        attempt.ProviderReference = result.ProviderReference ?? attempt.ProviderReference;
        attempt.AuthorizationCode = result.AuthorizationCode ?? attempt.AuthorizationCode;
        attempt.CardBrand = result.CardBrand ?? attempt.CardBrand;
        attempt.CardLast4 = result.CardLast4 ?? attempt.CardLast4;
        attempt.FailureReason = result.FailureReason ?? attempt.FailureReason;
        attempt.CustomerReceiptText = result.CustomerReceiptText ?? attempt.CustomerReceiptText;
        attempt.MerchantReceiptText = result.MerchantReceiptText ?? attempt.MerchantReceiptText;

        if (result.Status is not (PaymentAttemptStatus.Pending or PaymentAttemptStatus.Unknown))
        {
            attempt.CompletedAtUtc = DateTime.UtcNow;
        }

        payment.Status = result.Status switch
        {
            PaymentAttemptStatus.Approved => SalePaymentStatus.Captured,
            PaymentAttemptStatus.Declined or PaymentAttemptStatus.TimedOut => SalePaymentStatus.Failed,
            PaymentAttemptStatus.Cancelled => SalePaymentStatus.Cancelled,
            _ => SalePaymentStatus.Pending
        };
    }
}

public sealed record CreateSaleCommand(CreateSaleRequest Request, string? User) : ICommand<PosResult<SaleDto>>;

public sealed class CreateSaleCommandHandler(OpplatDbContext dbContext)
    : ICommandHandler<CreateSaleCommand, PosResult<SaleDto>>
{
    public async Task<PosResult<SaleDto>> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        if (request.Lines.Count == 0)
        {
            return PosResult<SaleDto>.Invalid("A sale needs at least one line.");
        }

        var shift = await dbContext.PosShifts
            .FirstOrDefaultAsync(s => s.RegisterId == request.RegisterId && s.Status == PosShiftStatus.Open, cancellationToken);
        if (shift is null)
        {
            return PosResult<SaleDto>.Conflict("Open a shift on this register before selling.");
        }

        var productIds = request.Lines.Select(l => l.ProductId).Distinct().ToList();
        var products = await dbContext.ProductsForSale
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        var missing = productIds.FirstOrDefault(id => !products.TryGetValue(id, out var p) || !p.Active);
        if (missing != Guid.Empty)
        {
            return PosResult<SaleDto>.Invalid($"Product {missing} is unknown or inactive.");
        }

        PricedSale priced;
        try
        {
            priced = PosPricing.Price(request.Lines.Select(l =>
            {
                var product = products[l.ProductId];
                return new PricingInput(product.Id, product.Name, product.Price, product.TaxRate, l.Quantity, l.Discount);
            }));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return PosResult<SaleDto>.Invalid(ex.Message);
        }

        if (priced.Total <= 0)
        {
            return PosResult<SaleDto>.Invalid("The sale total must be greater than zero.");
        }

        var sale = new Sale
        {
            Date = DateTime.UtcNow,
            Status = SaleStatus.Draft,
            RegisterId = request.RegisterId,
            ShiftId = shift.Id,
            CashierUserName = command.User,
            CreatedBy = command.User,
            TotalAmount = priced.Total
        };
        foreach (var line in priced.Lines)
        {
            sale.Products.Add(new SaleDetail
            {
                ProductId = line.ProductId,
                ProductName = line.Name,
                Quatity = line.Quantity,
                UnitPrice = line.UnitPrice,
                TaxRate = line.TaxRate,
                DiscountAmount = line.Discount,
                Amount = line.Gross
            });
        }

        dbContext.Sales.Add(sale);
        await dbContext.SaveChangesAsync(cancellationToken);
        return PosResult<SaleDto>.Ok(await PosSaleMapping.ToDtoAsync(dbContext, sale, cancellationToken));
    }
}

public sealed record GetPosSaleQuery(Guid Id) : IQuery<SaleDto?>;

public sealed class GetPosSaleQueryHandler(OpplatDbContext dbContext)
    : IQueryHandler<GetPosSaleQuery, SaleDto?>
{
    public async Task<SaleDto?> Handle(GetPosSaleQuery request, CancellationToken cancellationToken)
    {
        var sale = await dbContext.Sales.WithDetails().FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        return sale is null ? null : await PosSaleMapping.ToDtoAsync(dbContext, sale, cancellationToken);
    }
}

public sealed record ListPosSalesQuery(Guid? ShiftId, int Take = 50) : IQuery<IReadOnlyList<SaleDto>>;

public sealed class ListPosSalesQueryHandler(OpplatDbContext dbContext)
    : IQueryHandler<ListPosSalesQuery, IReadOnlyList<SaleDto>>
{
    public async Task<IReadOnlyList<SaleDto>> Handle(ListPosSalesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Sales.WithDetails().Where(s => s.RegisterId != null);
        if (request.ShiftId is not null)
        {
            query = query.Where(s => s.ShiftId == request.ShiftId);
        }

        var sales = await query.OrderByDescending(s => s.Date).Take(Math.Clamp(request.Take, 1, 200)).ToListAsync(cancellationToken);
        var result = new List<SaleDto>(sales.Count);
        foreach (var sale in sales)
        {
            result.Add(await PosSaleMapping.ToDtoAsync(dbContext, sale, cancellationToken));
        }

        return result;
    }
}

public sealed record VoidSaleCommand(Guid Id, string? User) : ICommand<PosResult<SaleDto>>;

public sealed class VoidSaleCommandHandler(OpplatDbContext dbContext)
    : ICommandHandler<VoidSaleCommand, PosResult<SaleDto>>
{
    public async Task<PosResult<SaleDto>> Handle(VoidSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await dbContext.Sales.WithDetails().FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (sale is null)
        {
            return PosResult<SaleDto>.NotFound();
        }

        if (sale.Status != SaleStatus.Draft)
        {
            return PosResult<SaleDto>.Conflict("Only unfinished sales can be voided; use a refund for completed sales.");
        }

        if (sale.Payments.Any(p => p.Status is SalePaymentStatus.Captured or SalePaymentStatus.Pending))
        {
            return PosResult<SaleDto>.Conflict("Cancel or refund the payments before voiding the sale.");
        }

        sale.Status = SaleStatus.Voided;
        sale.ModifiedBy = request.User;
        await dbContext.SaveChangesAsync(cancellationToken);
        return PosResult<SaleDto>.Ok(await PosSaleMapping.ToDtoAsync(dbContext, sale, cancellationToken));
    }
}
