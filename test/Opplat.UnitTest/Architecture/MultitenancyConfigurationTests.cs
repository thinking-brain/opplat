using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data;
using Opplat.UnitTest.Auth;

namespace Opplat.UnitTest.Architecture;

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
        var source = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Program.cs");

        Assert.Contains("tenantAccessor?.MultiTenantContext?.TenantInfo", source);
        Assert.Contains("PostgresTenantConnectionStringResolver.Resolve(", source);
        Assert.Contains("GetConnectionString(\"DefaultConnection\")", source);
        Assert.Contains("GetConnectionString(\"MainConnection\")", source);
    }

    [Fact]
    public void Program_ConfiguresTenantResolutionFromRouteAndHeader()
    {
        var source = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Program.cs");

        Assert.Contains("builder.Services.AddMultiTenant<AppTenantInfo>()", source);
        Assert.Contains(".WithRouteStrategy(\"__tenant__\")", source);
        Assert.Contains(".WithHeaderStrategy(\"X-Tenant-Identifier\")", source);
        Assert.Contains(".WithStore<TenantCatalogStore>(ServiceLifetime.Singleton)", source);
        Assert.Contains("app.UseMultiTenant();", source);
    }

    [Fact]
    public void TenantResolutionConsumers_UseTheRegisteredStoreAbstraction()
    {
        var accountEndpoints = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Endpoints", "AccountEndpoints.cs");
        var middleware = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Middleware", "TenantValidationMiddleware.cs");

        Assert.Contains("[FromServices] IMultiTenantStore<AppTenantInfo> tenantStore", accountEndpoints);
        Assert.Contains("GetService<IMultiTenantStore<AppTenantInfo>>()", middleware);
        Assert.DoesNotContain("[FromServices] TenantCatalogStore tenantStore", accountEndpoints, StringComparison.Ordinal);
        Assert.DoesNotContain("GetService<TenantCatalogStore>()", middleware, StringComparison.Ordinal);
    }

    [Fact]
    public void OpplatDbContext_ConfiguresAndEnforcesFinbuckleIsolation()
    {
        var source = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Data", "OpplatDbContext.cs");

        Assert.Contains("builder.ConfigureMultiTenant();", source);
        Assert.Contains("this.EnforceMultiTenant();", source);
        Assert.Contains("TenantMismatchMode => TenantMismatchMode.Throw", source);
        Assert.Contains("TenantNotSetMode => TenantNotSetMode.Throw", source);
    }

    [Fact]
    public void TenantScopedAdminHandlers_DeriveIsolationMetadataFromTheResolvedTenantContext()
    {
        var getTenantUsers = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Features", "Admin", "Queries", "GetTenantUsersQuery.cs");
        var createTenantUser = TestRepository.ReadAllText("src", "Opplat.Api.Main", "Features", "Admin", "Commands", "CreateTenantUserCommand.cs");

        Assert.Contains("public record GetTenantUsersQuery() : IRequest<List<AdminUserDto>>;", getTenantUsers);
        Assert.Contains("var tenant = _tenantAccessor.MultiTenantContext?.TenantInfo;", getTenantUsers);
        Assert.Contains("TenantId = tenant?.Id ?? string.Empty,", getTenantUsers);
        Assert.Contains("TenantIdentifier = tenant?.Identifier ?? string.Empty,", getTenantUsers);
        Assert.DoesNotContain("GetTenantUsersQuery(string", getTenantUsers, StringComparison.Ordinal);

        Assert.Contains("var tenant = _tenantAccessor.MultiTenantContext?.TenantInfo;", createTenantUser);
        Assert.Contains("TenantId = tenant?.Id ?? string.Empty,", createTenantUser);
        Assert.Contains("TenantIdentifier = tenant?.Identifier ?? string.Empty,", createTenantUser);
        Assert.Contains("TenantName = tenant?.Name ?? string.Empty,", createTenantUser);
        Assert.DoesNotContain("string TenantIdentifier", createTenantUser, StringComparison.Ordinal);
        Assert.DoesNotContain("string TenantId", createTenantUser, StringComparison.Ordinal);
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
