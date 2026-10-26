namespace Opplat.Application.Abstractions.Invoicing;

/// <summary>
/// Fiscal date/time helpers. Spanish invoices are dated in Europe/Madrid local time,
/// not UTC, so a sale at 00:30 local on 1 January belongs to the new numbering year.
/// </summary>
public static class FiscalTime
{
    private static readonly TimeZoneInfo Madrid = ResolveMadrid();

    public static DateTimeOffset ToFiscalTime(DateTime utc)
    {
        var asUtc = utc.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(utc, DateTimeKind.Utc)
            : utc.ToUniversalTime();
        return TimeZoneInfo.ConvertTime(new DateTimeOffset(asUtc), Madrid);
    }

    public static DateOnly ToFiscalDate(DateTime utc) => DateOnly.FromDateTime(ToFiscalTime(utc).DateTime);

    private static TimeZoneInfo ResolveMadrid()
    {
        foreach (var id in new[] { "Europe/Madrid", "Romance Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
            }
        }

        return TimeZoneInfo.Utc;
    }
}
