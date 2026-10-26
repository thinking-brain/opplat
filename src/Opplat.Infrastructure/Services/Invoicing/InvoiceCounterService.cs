using System.Data;
using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Invoicing;
using Opplat.Domain.Entities.Invoicing;
using Opplat.Infrastructure.Persistance.Data;

namespace Opplat.Infrastructure.Services.Invoicing;

public sealed class InvoiceCounterService(OpplatDbContext dbContext) : IInvoiceCounterService
{
    public async Task<InvoiceCounter> GetNextAsync(string series, int year, CancellationToken cancellationToken = default)
    {
        // Join the caller's transaction when there is one, so the number is released if issuing fails later.
        var ownsTransaction = dbContext.Database.IsRelational() && dbContext.Database.CurrentTransaction is null;
        var transaction = ownsTransaction
            ? await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken)
            : null;

        try
        {
            var existing = dbContext.Database.IsRelational()
                ? await dbContext.InvoiceCounters
                    .FromSqlRaw("SELECT * FROM \"InvoiceCounters\" WHERE \"Series\" = {0} AND \"Year\" = {1} FOR UPDATE", series, year)
                    .FirstOrDefaultAsync(cancellationToken)
                : await dbContext.InvoiceCounters
                    .FirstOrDefaultAsync(counter => counter.Series == series && counter.Year == year, cancellationToken);

            if (existing is null)
            {
                existing = new InvoiceCounter
                {
                    Series = series,
                    Year = year,
                    LastNumber = 1
                };

                await dbContext.InvoiceCounters.AddAsync(existing, cancellationToken);
            }
            else
            {
                existing.LastNumber += 1;
                dbContext.InvoiceCounters.Update(existing);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return existing;
        }
        finally
        {
            if (transaction is not null)
            {
                await transaction.DisposeAsync();
            }
        }
    }
}
