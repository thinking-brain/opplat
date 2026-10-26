using Opplat.Domain.Entities.Invoicing;
using Opplat.Infrastructure.Services.Invoicing.Verifactu;

namespace Opplat.UnitTest.Invoicing;

public class VerifactuHashCalculatorTests
{
    private static readonly DateTimeOffset GeneratedAt = new(2024, 1, 1, 19, 20, 30, TimeSpan.FromHours(1));

    [Fact]
    public void ComputeAlta_BuildsAeatFieldString()
    {
        var (input, _) = VerifactuHashCalculator.ComputeAlta(
            "89890001K", "12345678/G33", new DateOnly(2024, 1, 1), "F1", 12.35m, 123.45m, null, GeneratedAt);

        Assert.Equal(
            "IDEmisorFactura=89890001K&NumSerieFactura=12345678/G33&FechaExpedicionFactura=01-01-2024" +
            "&TipoFactura=F1&CuotaTotal=12.35&ImporteTotal=123.45&Huella=&FechaHoraHusoGenRegistro=2024-01-01T19:20:30+01:00",
            input);
    }

    // Worked example from the AEAT hash specification (first record in the chain).
    [Fact]
    public void ComputeAlta_MatchesAeatWorkedExample()
    {
        var (_, hash) = VerifactuHashCalculator.ComputeAlta(
            "89890001K", "12345678/G33", new DateOnly(2024, 1, 1), "F1", 12.35m, 123.45m, null, GeneratedAt);

        Assert.Equal("3C464DAF61ACB827C65FDA19F352A4E3BDC2C640E9E9FC4CC058073F38F12F60", hash);
    }

    [Fact]
    public void ComputeAlta_IncludesPreviousHashAndIsUppercaseHex()
    {
        var previous = new string('A', 64);

        var (input, hash) = VerifactuHashCalculator.ComputeAlta(
            "B12345678", "T1-000002", new DateOnly(2026, 7, 27), "F2", 21m, 121m, previous, GeneratedAt);

        Assert.Contains($"&Huella={previous}&", input);
        Assert.Equal(64, hash.Length);
        Assert.Equal(hash.ToUpperInvariant(), hash);
    }

    [Fact]
    public void ComputeAnulacion_UsesCancellationFieldNames()
    {
        var (input, _) = VerifactuHashCalculator.ComputeAnulacion(
            "89890001K", "12345678/G33", new DateOnly(2024, 1, 1), "ABC", GeneratedAt);

        Assert.Equal(
            "IDEmisorFacturaAnulada=89890001K&NumSerieFacturaAnulada=12345678/G33&FechaExpedicionFacturaAnulada=01-01-2024" +
            "&Huella=ABC&FechaHoraHusoGenRegistro=2024-01-01T19:20:30+01:00",
            input);
    }

    [Fact]
    public void FormatTimestamp_DropsFractionalSeconds()
    {
        var value = new DateTimeOffset(2026, 7, 27, 10, 0, 0, TimeSpan.FromHours(2)).AddMilliseconds(987);

        Assert.Equal("2026-07-27T10:00:00+02:00", VerifactuHashCalculator.FormatTimestamp(value));
    }

    [Fact]
    public void QrPayload_UsesIssuerNifAndAeatParameters()
    {
        var url = VerifactuQrCodeBuilder.BuildPayload("B12345678", "T1-000001", new DateOnly(2026, 7, 27), 12.5m);

        Assert.Equal(
            "https://www2.agenciatributaria.gob.es/wlpl/TIKE-CONT/ValidarQR?nif=B12345678&numserie=T1-000001&fecha=27-07-2026&importe=12.50",
            url);
    }

    [Fact]
    public void QrPayload_UsesMadridLocalDateAroundMidnight()
    {
        // 23:30 UTC on 31 Dec is already 1 Jan in Madrid.
        var invoice = new Invoice
        {
            Series = "T1",
            FullNumber = "T1-000001",
            IssueDate = new DateTime(2026, 12, 31, 23, 30, 0, DateTimeKind.Utc),
            TotalAmount = 10m
        };

        var url = VerifactuQrCodeBuilder.BuildPayload("B12345678", invoice);

        Assert.Contains("fecha=01-01-2027", url);
    }
}
