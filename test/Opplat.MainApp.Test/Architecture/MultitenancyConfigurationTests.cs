using System.IO;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Opplat.MainApp.Data;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Test.Architecture;

public class MultitenancyConfigurationTests
{
    [Fact]
    public void OpplatDbContext_UsesThrowModesForTenantSafety()
    {
        var options = new DbContextOptionsBuilder<OpplatDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new OpplatDbContext(options, CreateTenantAccessor("tenant-a", "mojocafe"));

        Assert.Equal("Throw", context.TenantMismatchMode.ToString());
        Assert.Equal("Throw", context.TenantNotSetMode.ToString());
        Assert.Equal("tenant-a", context.TenantInfo?.Id);
    }

    [Fact]
    public void Program_ConfiguresDbContextToPreferTenantConnectionString()
    {
        var current = AppContext.BaseDirectory;
        string? repoRoot = null;

        while (!string.IsNullOrEmpty(current))
        {
            if (File.Exists(Path.Combine(current, "opplat.sln")))
            {
                repoRoot = current;
                break;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        Assert.False(string.IsNullOrEmpty(repoRoot));

        var programPath = Path.Combine(repoRoot!, "src", "Opplat.MainApp", "Program.cs");

        var source = File.ReadAllText(programPath);

        Assert.Contains("tenantAccessor?.MultiTenantContext?.TenantInfo?.ConnectionString", source);
        Assert.Contains("GetConnectionString(\"DefaultConnection\")", source);
        Assert.Contains("GetConnectionString(\"MainConnection\")", source);
    }

    private static IMultiTenantContextAccessor<AppTenantInfo> CreateTenantAccessor(string tenantId, string tenantIdentifier)
    {
        var tenantContext = new Mock<IMultiTenantContext<AppTenantInfo>>();
        tenantContext.SetupGet(x => x.TenantInfo).Returns(new AppTenantInfo
        {
            Id = tenantId,
            Identifier = tenantIdentifier
        });

        var tenantAccessor = new Mock<IMultiTenantContextAccessor<AppTenantInfo>>();
        tenantAccessor.SetupGet(x => x.MultiTenantContext).Returns(tenantContext.Object);
        return tenantAccessor.Object;
    }
}
