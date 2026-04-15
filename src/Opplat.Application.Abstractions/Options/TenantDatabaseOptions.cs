namespace Opplat.Application.Abstractions.Options;

public sealed class TenantDatabaseOptions
{
    public const string SectionName = "TenantDatabase";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
