namespace Opplat.Infrastructure.Services.Invoicing;

public sealed class VerifactuWorkerOptions
{
    public const string SectionName = "VerifactuWorker";

    /// <summary>How often the outbox polling loop runs.</summary>
    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>Maximum rows dequeued per tick.</summary>
    public int BatchSize { get; set; } = 10;

    /// <summary>Number of attempts before a record is marked <c>Rejected</c>.</summary>
    public int MaxRetries { get; set; } = 10;

    /// <summary>Base delay for exponential back-off: delay = BaseBackoffSeconds × 2^retryCount.</summary>
    public int BaseBackoffSeconds { get; set; } = 30;

    /// <summary>Maximum back-off ceiling.</summary>
    public TimeSpan MaxBackoff { get; set; } = TimeSpan.FromHours(4);
}
