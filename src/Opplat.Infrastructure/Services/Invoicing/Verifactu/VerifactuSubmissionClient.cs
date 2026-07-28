using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opplat.Domain.Entities.Invoicing;

namespace Opplat.Infrastructure.Services.Invoicing.Verifactu;

/// <summary>
/// Submits Verifactu fiscal records to the AEAT SOAP web service.
/// </summary>
/// <remarks>
/// The actual SOAP implementation is a stub pending:
/// 1. Obtaining a valid digital certificate for the production environment.
/// 2. Finalising the XML record schema from the AEAT WSDL / XSD artefacts.
/// 3. Validation against the AEAT pre-production portal (<c>https://preportal.aeat.es/</c>).
///
/// Until <see cref="VerifactuSubmissionOptions.Enabled"/> is <c>true</c> this client
/// does nothing and all records remain in the outbox with <c>SubmissionStatus = Pending</c>.
/// </remarks>
public sealed class VerifactuSubmissionClient(
    IOptions<VerifactuSubmissionOptions> options,
    ILogger<VerifactuSubmissionClient> logger)
{
    private readonly VerifactuSubmissionOptions _opts = options.Value;

    /// <summary>
    /// Submits a single <see cref="InvoiceFiscalRecord"/> to the AEAT Verifactu endpoint.
    /// Populates <see cref="InvoiceFiscalRecord.AeatCsv"/> and
    /// <see cref="InvoiceFiscalRecord.AeatResponseRaw"/> on success.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// Always thrown while the real SOAP client is not yet implemented.
    /// </exception>
    public Task SubmitAsync(InvoiceFiscalRecord record, CancellationToken cancellationToken = default)
    {
        if (!_opts.Enabled)
        {
            logger.LogDebug(
                "Verifactu submission is disabled (VerifactuSubmission:Enabled = false). " +
                "Record {RecordId} will remain pending.", record.Id);
            return Task.CompletedTask;
        }

        // TODO: implement AEAT SOAP submission:
        // 1. Build the XML "RegistroFacturacion" envelope from the record fields.
        // 2. POST to _opts.EndpointUrl with the tenant's digital certificate.
        // 3. Parse the SOAP response; on success populate record.AeatCsv and record.AeatResponseRaw.
        // 4. Throw on transient errors so the outbox worker can apply exponential back-off.
        throw new NotImplementedException(
            $"AEAT Verifactu SOAP submission is not yet implemented. " +
            $"Record {record.Id} cannot be submitted until the SOAP client is built and " +
            $"VerifactuSubmission:Enabled is set to true in appsettings.");
    }
}
