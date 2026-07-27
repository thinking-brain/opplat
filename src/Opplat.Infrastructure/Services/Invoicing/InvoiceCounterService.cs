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
        await using var transaction = await dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

        var existing = await dbContext.InvoiceCounters
            .FromSqlRaw("SELECT * FROM \"InvoiceCounters\" WHERE \"Series\" = {0} AND \"Year\" = {1} FOR UPDATE", series, year)
            .FirstOrDefaultAsync(cancellationToken);

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
        await transaction.CommitAsync(cancellationToken);

        return existing;
    }
}