namespace Opplat.Infrastructure.Services.Pos;

public sealed class PosPaymentOptions
{
    public const string SectionName = "Pos";

    /// <summary>Shared secret used to sign webhooks sent by the mock terminal.</summary>
    public string MockWebhookSecret { get; set; } = "mock-terminal-secret";

    /// <summary>How long the mock terminal takes to "present the card" before it reports a result.</summary>
    public TimeSpan MockApprovalDelay { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>Largest cash payment accepted when a business is a party (Ley 7/2012: EUR 1,000).</summary>
    public decimal CashPaymentLimit { get; set; } = 1000m;
}
