using System.Collections.Concurrent;
using Npgsql;
using Opplat.Infrastructure.Services;

namespace Opplat.UnitTest.Infrastructure;

public class TenantProvisioningServiceTests
{
    [Fact]
    public void ShouldSkipProvisioning_ReturnsFalse_WhenSchemaIsNotReady()
    {
        var provisionedTenants = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
        provisionedTenants.TryAdd("tenant-1", 0);

        var result = TenantProvisioningService.ShouldSkipProvisioning(provisionedTenants, "tenant-1", tenantSchemaReady: false);

        Assert.False(result);
    }

    [Fact]
    public void ShouldSkipProvisioning_ReturnsTrue_WhenSchemaIsReady()
    {
        var provisionedTenants = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
        provisionedTenants.TryAdd("tenant-1", 0);

        var result = TenantProvisioningService.ShouldSkipProvisioning(provisionedTenants, "tenant-1", tenantSchemaReady: true);

        Assert.True(result);
    }

    [Theory]
    [InlineData("42P07", true)]
    [InlineData("42710", true)]
    [InlineData("23505", false)]
    [InlineData("42P01", false)]
    public void IsRecoverableMigrationError_ReturnsExpectedValue(string sqlState, bool expected)
    {
        var exception = new PostgresException(
            "duplicate relation",
            "ERROR",
            "ERROR",
            sqlState,
            "duplicate relation",
            "already exists",
            0,
            0,
            null,
            "where",
            "schema",
            "table",
            null,
            null,
            "file",
            "line",
            "routine");

        var result = TenantProvisioningService.IsRecoverableMigrationError(exception);

        Assert.Equal(expected, result);
    }
}
