using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Opplat.Application.Abstractions.Identity;
using Opplat.Infrastructure.DependencyInjection;
using Opplat.Infrastructure.Identity;

namespace Opplat.UnitTest.Identity;

public class GraphServiceRegistrationTests
{
    [Fact]
    public void WhenDisabled_RegistersNoOpGraphUserService()
    {
        var config = BuildConfig(new Dictionary<string, string?>
        {
            ["GraphApi:Enabled"] = "false"
        });

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddGraphUserService(config);

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IUserManagementService>();

        Assert.IsType<NoOpGraphUserService>(service);
    }

    [Fact]
    public void WhenEnabledWithCredentials_RegistersRealGraphUserService()
    {
        var config = BuildConfig(new Dictionary<string, string?>
        {
            ["GraphApi:Enabled"] = "true",
            ["GraphApi:TenantId"] = "test-tenant",
            ["GraphApi:ClientId"] = "test-client",
            ["GraphApi:ClientSecret"] = "test-secret",
            ["GraphApi:TenantDomain"] = "test.onmicrosoft.com"
        });

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddGraphUserService(config);

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IUserManagementService>();

        Assert.IsType<GraphUserService>(service);
    }

    [Fact]
    public void WhenEnabledWithoutTenantId_RegistersNoOp()
    {
        var config = BuildConfig(new Dictionary<string, string?>
        {
            ["GraphApi:Enabled"] = "true",
            ["GraphApi:TenantId"] = "",
            ["GraphApi:ClientId"] = "test-client"
        });

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddGraphUserService(config);

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IUserManagementService>();

        Assert.IsType<NoOpGraphUserService>(service);
    }

    [Fact]
    public void WhenNotConfigured_RegistersNoOp()
    {
        var config = BuildConfig(new Dictionary<string, string?>());

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddGraphUserService(config);

        var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IUserManagementService>();

        Assert.IsType<NoOpGraphUserService>(service);
    }

    private static IConfiguration BuildConfig(Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
