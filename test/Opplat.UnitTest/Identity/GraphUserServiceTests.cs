using Opplat.Application.Abstractions.Identity;
using Opplat.Infrastructure.Identity;
using Opplat.UnitTest.Auth;

namespace Opplat.UnitTest.Identity;

public class GraphUserServiceTests
{
    [Fact]
    public void Interface_DefinesAllRequiredOperations()
    {
        var methods = typeof(IGraphUserService).GetMethods();
        var methodNames = methods.Select(m => m.Name).ToHashSet();

        Assert.Contains("CreateUserAsync", methodNames);
        Assert.Contains("EnableUserAsync", methodNames);
        Assert.Contains("DisableUserAsync", methodNames);
        Assert.Contains("DeleteUserAsync", methodNames);
        Assert.Contains("ResetPasswordAsync", methodNames);
    }

    [Fact]
    public void Interface_AllMethodsReturnGraphUserResult()
    {
        var methods = typeof(IGraphUserService).GetMethods();

        foreach (var method in methods)
        {
            Assert.Equal(typeof(Task<GraphUserResult>), method.ReturnType);
        }
    }

    [Fact]
    public void Interface_AllMethodsAcceptCancellationToken()
    {
        var methods = typeof(IGraphUserService).GetMethods();

        foreach (var method in methods)
        {
            Assert.Contains(method.GetParameters(), p => p.ParameterType == typeof(CancellationToken));
        }
    }

    [Fact]
    public void GraphUserResult_Success_SetsCorrectProperties()
    {
        var result = GraphUserResult.Success("test-oid");

        Assert.True(result.Succeeded);
        Assert.Equal("test-oid", result.ObjectId);
        Assert.Null(result.Error);
        Assert.Null(result.StatusCode);
    }

    [Fact]
    public void GraphUserResult_Failure_SetsCorrectProperties()
    {
        var result = GraphUserResult.Failure("something broke", 404);

        Assert.False(result.Succeeded);
        Assert.Null(result.ObjectId);
        Assert.Equal("something broke", result.Error);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public void GraphUserResult_SuccessWithoutOid_ReturnsNullObjectId()
    {
        var result = GraphUserResult.Success();

        Assert.True(result.Succeeded);
        Assert.Null(result.ObjectId);
    }

    [Fact]
    public void CreateGraphUserRequest_RequiresEmail_DisplayName_TemporaryPassword()
    {
        var request = new CreateGraphUserRequest
        {
            Email = "user@example.com",
            DisplayName = "Test User",
            TemporaryPassword = "P@ssw0rd!"
        };

        Assert.Equal("user@example.com", request.Email);
        Assert.Equal("Test User", request.DisplayName);
        Assert.Equal("P@ssw0rd!", request.TemporaryPassword);
        Assert.Null(request.GivenName);
        Assert.Null(request.Surname);
    }

    [Fact]
    public void CreateGraphUserRequest_SupportsOptionalFields()
    {
        var request = new CreateGraphUserRequest
        {
            Email = "user@example.com",
            DisplayName = "Test User",
            TemporaryPassword = "P@ssw0rd!",
            GivenName = "Test",
            Surname = "User"
        };

        Assert.Equal("Test", request.GivenName);
        Assert.Equal("User", request.Surname);
    }

    [Fact]
    public void GraphApiOptions_Defaults_DisabledWithEmptyStrings()
    {
        var options = new GraphApiOptions();

        Assert.False(options.Enabled);
        Assert.Equal(string.Empty, options.TenantId);
        Assert.Equal(string.Empty, options.ClientId);
        Assert.Null(options.ClientSecret);
        Assert.Null(options.CertificateThumbprint);
        Assert.Equal(string.Empty, options.TenantDomain);
    }

    [Fact]
    public void GraphApiOptions_SectionName_IsGraphApi()
    {
        Assert.Equal("GraphApi", GraphApiOptions.SectionName);
    }

    [Fact]
    public void Source_GraphUserServiceUsesGraphSdk()
    {
        var source = TestRepository.ReadAllText(
            "src", "Opplat.Infrastructure", "Identity", "GraphUserService.cs");

        Assert.Contains("GraphServiceClient", source);
        Assert.Contains("ODataError", source);
        Assert.Contains("ForceChangePasswordNextSignIn", source);
        Assert.Contains("AccountEnabled", source);
        Assert.Contains("UserPrincipalName", source);
        Assert.Contains("_options.TenantDomain", source);
    }

    [Fact]
    public void Source_GraphUserServiceHandlesErrors()
    {
        var source = TestRepository.ReadAllText(
            "src", "Opplat.Infrastructure", "Identity", "GraphUserService.cs");

        Assert.Contains("HandleODataError", source);
        Assert.Contains("ResponseStatusCode", source);
    }

    [Fact]
    public void Source_GraphUserServiceUsesUuidUpnFormat()
    {
        var source = TestRepository.ReadAllText(
            "src", "Opplat.Infrastructure", "Identity", "GraphUserService.cs");

        Assert.Contains("Guid.NewGuid()", source);
        Assert.Contains("_options.TenantDomain", source);
    }
}
