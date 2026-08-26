namespace Opplat.Application.Abstractions.Options;

public sealed class StripeOptions
{
    public const string SectionName = "Stripe";

    public bool Enabled { get; set; }

    public string SecretKey { get; set; } = string.Empty;

    public string PublishableKey { get; set; } = string.Empty;

    public string WebhookSecret { get; set; } = string.Empty;
}
