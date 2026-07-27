using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Infrastructure.Persistance.Configurations.Invoicing;

public sealed class TenantFiscalSettingsConfiguration : IEntityTypeConfiguration<TenantFiscalSettings>
{
    public void Configure(EntityTypeBuilder<TenantFiscalSettings> builder)
    {
        builder.Property(settings => settings.LegalName).IsRequired();
        builder.Property(settings => settings.TaxId).IsRequired();
        builder.Property(settings => settings.FiscalAddress).IsRequired();
        builder.Property(settings => settings.DefaultSeries).IsRequired();
    }
}