namespace Opplat.Application.Abstractions.Options;

public sealed class DatabaseInstanceOptions
{
    public const string SectionName = "DatabaseInstance";

    public int MaxTenantsPerInstance { get; set; } = 100;

    public int InitialDatabaseInstanceId { get; set; } = 1;

    public string DefaultConnectionString { get; set; } = string.Empty;
}
