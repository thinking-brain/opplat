using System.IO;
using System.Security.Claims;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Http;
using Moq;
using Opplat.MainApp.Middleware;
using Opplat.MainApp.Models;

namespace Opplat.MainApp.Test.Middleware;

public class TenantValidationMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_AuthenticatedUserWithMatchingTenant_CallsNext()
    {
        var nextCalled = false;
        var middleware = new TenantValidationMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        var context = CreateHttpContext(
            CreateAuthenticatedUser(new Claim("tenant_id", "tenant-a")));

        await middleware.InvokeAsync(context, CreateTenantAccessor("tenant-a", "mojocafe"));

        Assert.True(nextCalled);
        Assert.NotEqual(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_AuthenticatedUserWithMismatchedTenant_ReturnsForbidden()
    {
        var nextCalled = false;
        var middleware = new TenantValidationMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        var context = CreateHttpContext(
            CreateAuthenticatedUser(new Claim("tenant_id", "tenant-a")));

        await middleware.InvokeAsync(context, CreateTenantAccessor("tenant-b", "demo"));

        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();

        Assert.False(nextCalled);
        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        Assert.Equal("Token not valid for this tenant.", body);
    }

    [Fact]
    public async Task InvokeAsync_UnauthenticatedUser_CallsNext()
    {
        var nextCalled = false;
        var middleware = new TenantValidationMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        var context = CreateHttpContext(new ClaimsPrincipal(new ClaimsIdentity()));

        await middleware.InvokeAsync(context, CreateTenantAccessor("tenant-a", "mojocafe"));

        Assert.True(nextCalled);
        Assert.NotEqual(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_AuthenticatedUserWithoutTenantClaim_CallsNext()
    {
        var nextCalled = false;
        var middleware = new TenantValidationMiddleware(_ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        var context = CreateHttpContext(CreateAuthenticatedUser());

        await middleware.InvokeAsync(context, CreateTenantAccessor("tenant-a", "mojocafe"));

        Assert.True(nextCalled);
        Assert.NotEqual(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    private static DefaultHttpContext CreateHttpContext(ClaimsPrincipal user)
    {
        return new DefaultHttpContext
        {
            User = user,
            Response =
            {
                Body = new MemoryStream()
            }
        };
    }

    private static ClaimsPrincipal CreateAuthenticatedUser(params Claim[] claims)
    {
        return new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "TestAuth"));
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
