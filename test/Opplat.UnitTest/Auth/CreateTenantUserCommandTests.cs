using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Opplat.Application.Abstractions.Auth;
using Opplat.Application.Abstractions.Identity;
using Opplat.Application.Features.Admin.Commands;
using Opplat.Domain.Models;
using Opplat.Infrastructure.Persistance.Data.Administration;

namespace Opplat.UnitTest.Auth;

public class CreateTenantUserCommandTests
{
    [Fact]
    public async Task Handle_RejectsRequestsWithoutAnyTenantRole()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(
            new CreateTenantUserCommand("Tenant", "User", "tenant.user", "tenant.user@opplat.local", []),
            CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_RejectsRolesOutsideTenantAssignableSet()
    {
        var handler = CreateHandler();

        var result = await handler.Handle(
            new CreateTenantUserCommand(
                "Tenant",
                "User",
                "tenant.user",
                "tenant.user@opplat.local",
                [AuthRoles.TenantUser, AuthRoles.SuperAdmin]),
            CancellationToken.None);

        Assert.Null(result);
    }

    private static CreateTenantUserCommandHandler CreateHandler()
    {
        var options = new DbContextOptionsBuilder<AdminTenantCatalogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new AdminTenantCatalogDbContext(options);

        return new CreateTenantUserCommandHandler(
            db,
            CreateTenantAccessor(),
            Mock.Of<IGraphUserService>(),
            Mock.Of<ILogger<CreateTenantUserCommandHandler>>());
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
