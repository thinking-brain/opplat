using Microsoft.EntityFrameworkCore;
using Opplat.Application.Abstractions.Messaging;
using Opplat.Application.Features.Invoicing.Common;
using Opplat.Domain.Entities.Invoicing;
using Opplat.Infrastructure.Persistance.Data;

namespace Opplat.Application.Features.Invoicing.Settings;

public sealed record GetTenantFiscalSettingsQuery() : IQuery<TenantFiscalSettings?>;

public sealed class GetTenantFiscalSettingsQueryHandler(OpplatDbContext dbContext)
    : IQueryHandler<GetTenantFiscalSettingsQuery, TenantFiscalSettings?>
{
    public Task<TenantFiscalSettings?> Handle(GetTenantFiscalSettingsQuery request, CancellationToken cancellationToken)
        => dbContext.TenantFiscalSettings.FirstOrDefaultAsync(cancellationToken);
}

public sealed record UpsertTenantFiscalSettingsCommand(TenantFiscalSettings TenantFiscalSettings) : ICommand<InvoiceCommandResult>;

public sealed class UpsertTenantFiscalSettingsCommandHandler(OpplatDbContext dbContext)
    : ICommandHandler<UpsertTenantFiscalSettingsCommand, InvoiceCommandResult>
{
    public async Task<InvoiceCommandResult> Handle(UpsertTenantFiscalSettingsCommand request, CancellationToken cancellationToken)
    {
        var existing = await dbContext.TenantFiscalSettings.FirstOrDefaultAsync(cancellationToken);
        if (existing is null)
        {
            await dbContext.TenantFiscalSettings.AddAsync(request.TenantFiscalSettings, cancellationToken);
        }
        else
        {
            existing.LegalName = request.TenantFiscalSettings.LegalName;
            existing.TaxId = request.TenantFiscalSettings.TaxId;
            existing.FiscalAddress = request.TenantFiscalSettings.FiscalAddress;
            existing.Country = request.TenantFiscalSettings.Country;
            existing.BusinessSector = request.TenantFiscalSettings.BusinessSector;
            existing.DefaultSeries = request.TenantFiscalSettings.DefaultSeries;
            existing.InvoicingMode = request.TenantFiscalSettings.InvoicingMode;
            existing.SimplifiedInvoiceThreshold = request.TenantFiscalSettings.SimplifiedInvoiceThreshold;
            existing.SoftwareLicenseId = request.TenantFiscalSettings.SoftwareLicenseId;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return InvoiceCommandResult.From(true, "Tenant fiscal settings saved.");
    }
}