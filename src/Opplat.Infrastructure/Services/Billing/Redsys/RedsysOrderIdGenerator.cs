using System.Globalization;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.Infrastructure.Services.Billing.Redsys;

/// <summary>
/// Generates unique Ds_Order values: Redsys requires 4-12 alphanumeric characters with a
/// numeric first 4 characters. Uniqueness is enforced against the redsys_transactions.order
/// unique index.
/// </summary>
public sealed class RedsysOrderIdGenerator(AdminTenantCatalogDbContext dbContext)
{
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public async Task<string> GenerateAsync(CancellationToken cancellationToken = default)
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var candidate = Generate();
            var exists = await dbContext.RedsysTransactions
                .AnyAsync(transaction => transaction.Order == candidate, cancellationToken);
            if (!exists)
                return candidate;
        }

        throw new InvalidOperationException("Unable to generate a unique Redsys order id after 5 attempts.");
    }

    private static string Generate()
    {
        var numericPrefix = (DateTime.UtcNow.Ticks % 10000).ToString("D4", CultureInfo.InvariantCulture);
        return numericPrefix + GenerateAlphanumeric(8);
    }

    private static string GenerateAlphanumeric(int length)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        var chars = new char[length];
        for (var i = 0; i < length; i++)
            chars[i] = Alphabet[bytes[i] % Alphabet.Length];
        return new string(chars);
    }
}
