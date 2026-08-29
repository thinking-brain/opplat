namespace Opplat.Infrastructure.Services.Billing;

public sealed class SubscriptionRenewalWorkerOptions
{
    public const string SectionName = "SubscriptionRenewalWorker";

    /// <summary>How often the renewal polling loop runs.</summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);

    /// <summary>Maximum tenants renewed per tick.</summary>
    public int BatchSize { get; set; } = 50;
}
