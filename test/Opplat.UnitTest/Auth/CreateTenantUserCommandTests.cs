using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Features.Admin.Commands;
using Opplat.Domain.Models;

namespace Opplat.UnitTest.Auth;

public class CreateTenantUserCommandTests
{
    [Fact]
    public async Task Handle_RejectsRequestsWithoutAnyTenantRole()
    {
        var userManager = CreateUserManager();
        var handler = CreateHandler(userManager.Object, CreateRoleManager().Object);

        var result = await handler.Handle(
            new CreateTenantUserCommand("Tenant", "User", "tenant.user", "tenant.user@opplat.local", []),
            CancellationToken.None);

        Assert.Null(result);
        userManager.Verify(manager => manager.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_RejectsRolesOutsideTenantAssignableSet()
    {
        var userManager = CreateUserManager();
        var handler = CreateHandler(userManager.Object, CreateRoleManager().Object);

        var result = await handler.Handle(
            new CreateTenantUserCommand(
                "Tenant",
                "User",
                "tenant.user",
                "tenant.user@opplat.local",
                [AuthRoles.TenantUser, AuthRoles.SuperAdmin]),
            CancellationToken.None);

        Assert.Null(result);
        userManager.Verify(manager => manager.CreateAsync(It.IsAny<User>()), Times.Never);
    }

    private static CreateTenantUserCommandHandler CreateHandler(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        return new CreateTenantUserCommandHandler(
            // userManager,
            // roleManager,
            CreateTenantAccessor(),
            Mock.Of<ILogger<CreateTenantUserCommandHandler>>());
    }

    private static Mock<UserManager<User>> CreateUserManager()
    {
        var store = new Mock<IUserStore<User>>();
        return new Mock<UserManager<User>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }

    private static Mock<RoleManager<IdentityRole>> CreateRoleManager()
    {
        var store = new Mock<IRoleStore<IdentityRole>>();
        return new Mock<RoleManager<IdentityRole>>(
            store.Object,
            null!,
            null!,
            null!,
            null!);
    }

    private static IMultiTenantContextAccessor<AppTenantInfo> CreateTenantAccessor()
    {
        var tenantContext = new Mock<IMultiTenantContext<AppTenantInfo>>();
        tenantContext.SetupGet(context => context.TenantInfo).Returns(new AppTenantInfo
        {
            Id = "tenant-a",
            Identifier = "mojocafe",
            Name = "MojoCafe"
        });

        return Mock.Of<IMultiTenantContextAccessor<AppTenantInfo>>(accessor => accessor.MultiTenantContext == tenantContext.Object);
    }
}
