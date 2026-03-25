using System.IO;

namespace Opplat.MainApp.Test.Auth;

public class AdminApiSplitContractTests
{
    [Fact]
    public void AdminApiProject_UsesNewRootProjectInsteadOfLegacyServiceHost()
    {
        var solution = TestRepository.ReadAllText("opplat.slnx");
        var testProject = TestRepository.ReadAllText("test", "Opplat.MainApp.Test", "Opplat.MainApp.Test.csproj");
        var readme = TestRepository.ReadAllText("README.md");
        var compose = TestRepository.ReadAllText("docker-compose.yml");

        Assert.Contains("src/Opplat.AdminApi/Opplat.AdminApi.csproj", solution);
        Assert.DoesNotContain("Opplat.Services.Admin.Api", solution);

        Assert.Contains("..\\..\\src\\Opplat.AdminApi\\Opplat.AdminApi.csproj", testProject);
        Assert.DoesNotContain("Opplat.Services.Admin.Api.csproj", testProject);

        Assert.Contains("src/Opplat.AdminApi", readme);
        Assert.DoesNotContain("src/Services/Admin/Opplat.Services.Admin.Api", readme);

        Assert.Contains("dockerfile: src/Opplat.AdminApi/Dockerfile", compose);
        Assert.DoesNotContain("dockerfile: src/Services/Admin/Opplat.Services.Admin.Api/Dockerfile", compose);

        Assert.False(Directory.Exists(TestRepository.ResolvePath("src", "Services", "Admin", "Opplat.Services.Admin.Api")));
    }

    [Fact]
    public void AdminApiProject_MapsDedicatedEndpointModulesWithoutTemplateEndpoints()
    {
        var generalEndpoints = TestRepository.ReadAllText("src", "Opplat.AdminApi", "Endpoints", "GeneralEndpoints.cs");
        var adminEndpoints = TestRepository.ReadAllText("src", "Opplat.AdminApi", "Endpoints", "AdminEndpoints.cs");
        var program = TestRepository.ReadAllText("src", "Opplat.AdminApi", "Program.cs");

        Assert.Contains("public static class GeneralEndpoints", generalEndpoints);
        Assert.Contains("MapGeneralEndpoints", generalEndpoints);
        Assert.Contains("/health", generalEndpoints);
        Assert.Contains("/healthcheck", generalEndpoints);

        Assert.Contains("public static class AdminEndpoints", adminEndpoints);
        Assert.Contains("MapAdminEndpoints", adminEndpoints);
        Assert.Contains("/admin", adminEndpoints);

        Assert.Contains("using Opplat.AdminApi.Endpoints;", program);
        Assert.Contains("app.MapGeneralEndpoints();", program);
        Assert.Contains("app.MapAdminEndpoints();", program);
        Assert.DoesNotContain("weatherforecast", program);
    }

    [Fact]
    public void AdminApiComposeHealthProbe_MatchesNewProjectHealthEndpoint()
    {
        var generalEndpoints = TestRepository.ReadAllText("src", "Opplat.AdminApi", "Endpoints", "GeneralEndpoints.cs");
        var compose = TestRepository.ReadAllText("docker-compose.yml");
        var dockerfile = TestRepository.ReadAllText("src", "Opplat.AdminApi", "Dockerfile");

        Assert.Contains("/health", generalEndpoints);
        Assert.Contains("/healthcheck", generalEndpoints);
        Assert.Contains("ENTRYPOINT [\"dotnet\", \"Opplat.AdminApi.dll\"]", dockerfile);
        Assert.Contains("GET /health HTTP/1.1", compose);
    }

    [Fact]
    public void AdminFrontend_RuntimeTargetsDedicatedAdminApiWithoutBrowserOidcDependencies()
    {
        var packageJson = TestRepository.ReadAllText("src", "opplat-admin", "package.json");
        var runtimeConfig = TestRepository.ReadAllText("src", "opplat-admin", "src", "runtimeConfig.ts");
        var envExample = TestRepository.ReadAllText("src", "opplat-admin", ".env.example");
        var viteConfig = TestRepository.ReadAllText("src", "opplat-admin", "vite.config.ts");
        var dockerfile = TestRepository.ReadAllText("src", "opplat-admin", "Dockerfile");
        var axiosClient = TestRepository.ReadAllText("src", "opplat-admin", "src", "api", "axiosClient.ts");
        var composeOverride = TestRepository.ReadAllText("docker-compose.override.yml");

        Assert.DoesNotContain("oidc-client-ts", packageJson);
        Assert.DoesNotContain("react-oidc-context", packageJson);

        Assert.Contains("const adminApiBaseUrl = trimTrailingSlash(readConfig('VITE_ADMIN_API_URL', ''));", runtimeConfig);
        Assert.Contains("adminApiUrl: adminApiBaseUrl,", runtimeConfig);
        Assert.Contains("VITE_ADMIN_API_URL=", envExample);
        Assert.Contains("VITE_DEV_PROXY_TARGET=http://localhost:8084", envExample);

        Assert.Contains("const proxyTarget = env.VITE_DEV_PROXY_TARGET", viteConfig);
        Assert.Contains("|| env.VITE_ADMIN_API_URL", viteConfig);
        Assert.Contains("|| 'http://localhost:8084';", viteConfig);
        Assert.Contains("'/admin': proxyOptions", viteConfig);
        Assert.DoesNotContain("'/auth': proxyOptions", viteConfig);

        Assert.Contains("VITE_DEV_PROXY_TARGET=http://admin-api:8080", composeOverride);
        Assert.Contains("VITE_ADMIN_API_URL=http://localhost:8084", composeOverride);

        Assert.Contains("proxy_pass http://admin-api:8080;", dockerfile);
        Assert.DoesNotContain("proxy_pass http://api:8080;", dockerfile);
        Assert.DoesNotContain("Authorization", axiosClient);
    }
}
