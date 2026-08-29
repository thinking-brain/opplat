namespace Opplat.Application.Abstractions.Options;

public sealed class OpplatLegalDataOptions
{
    public const string SectionName = "OpplatLegal";

    public string CompanyName { get; set; } = "Opplat";

    public string TaxId { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
}
