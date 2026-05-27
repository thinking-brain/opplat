using Opplat.Application.Abstractions.Identity;
using Opplat.Infrastructure.Identity;
using Opplat.UnitTest.Auth;

namespace Opplat.UnitTest.Identity;

public class GraphUserServiceTests
{
    [Fact]
    public void Interface_DefinesAllRequiredOperations()
    {
        var methods = typeof(IUserManagementService).GetMethods();
        var methodNames = methods.Select(m => m.Name).ToHashSet();

        Assert.Contains("CreateUserAsync", methodNames);
        Assert.Contains("EnableUserAsync", methodNames);
        Assert.Contains("DisableUserAsync", methodNames);
        Assert.Contains("DeleteUserAsync", methodNames);
        Assert.Contains("ResetPasswordAsync", methodNames);
    }

    [Fact]
    public void Interface_AllMethodsReturnUserOperationResult()
    {
        var methods = typeof(IUserManagementService).GetMethods();

        foreach (var method in methods)
        {
            Assert.Equal(typeof(Task<UserOperationResult>), method.ReturnType);
        }
    }

    [Fact]
    public void Interface_AllMethodsAcceptCancellationToken()
    {
        var methods = typeof(IUserManagementService).GetMethods();

        foreach (var method in methods)
        {
            Assert.Contains(method.GetParameters(), p => p.ParameterType == typeof(CancellationToken));
        }
    }

    [Fact]
    public void UserOperationResult_Success_SetsCorrectProperties()
    {
        var result = UserOperationResult.Success("test-oid");

        Assert.True(result.Succeeded);
        Assert.Equal("test-oid", result.ObjectId);
        Assert.Null(result.Error);
        Assert.Null(result.StatusCode);
    }

    [Fact]
    public void UserOperationResult_Failure_SetsCorrectProperties()
    {
        var result = UserOperationResult.Failure("something broke", 404);

        Assert.False(result.Succeeded);
        Assert.Null(result.ObjectId);
        Assert.Equal("something broke", result.Error);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public void UserOperationResult_SuccessWithoutOid_ReturnsNullObjectId()
    {
        var result = UserOperationResult.Success();

        Assert.True(result.Succeeded);
        Assert.Null(result.ObjectId);
    }

    [Fact]
    public void CreateUserRequest_RequireFields()
    {
        var request = new CreateUserRequest
        {
            Email = "user@example.com",
            UserName = "Test User",
            Password = "P@ssw0rd!",
            FirstName = "Test",
            LastName = "User"
        };

        Assert.Equal("user@example.com", request.Email);
        Assert.Equal("Test User", request.UserName);
        Assert.Equal("P@ssw0rd!", request.Password);
        Assert.Equal("Test", request.FirstName);
        Assert.Equal("User", request.LastName);
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
