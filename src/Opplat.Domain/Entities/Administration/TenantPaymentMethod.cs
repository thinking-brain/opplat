namespace Opplat.Domain.Entities.Administration;

public sealed class TenantPaymentMethod : BaseEntity
{
    public Guid TenantId { get; set; }

    public string StripePaymentMethodId { get; set; } = string.Empty;

    public string? Brand { get; set; }

    public string? Last4 { get; set; }

    public long? ExpMonth { get; set; }

    public long? ExpYear { get; set; }

    public bool IsDefault { get; set; }

    public Tenant? Tenant { get; set; }
}