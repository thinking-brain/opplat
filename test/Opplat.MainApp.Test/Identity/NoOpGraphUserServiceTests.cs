using Microsoft.Extensions.Logging;
using Moq;
using Opplat.Application.Abstractions.Identity;
using Opplat.Infrastructure.Identity;

namespace Opplat.MainApp.Test.Identity;

public class NoOpGraphUserServiceTests
{
    private readonly NoOpGraphUserService _sut;

    public NoOpGraphUserServiceTests()
    {
        _sut = new NoOpGraphUserService(Mock.Of<ILogger<NoOpGraphUserService>>());
    }

    [Fact]
    public async Task CreateUserAsync_ReturnsSuccess_WithNoopObjectId()
    {
        var request = new CreateGraphUserRequest
        {
            Email = "test@example.com",
            DisplayName = "Test User",
            TemporaryPassword = "TempP@ss1"
        };

        var result = await _sut.CreateUserAsync(request);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.ObjectId);
        Assert.StartsWith("noop-", result.ObjectId);
    }

    [Fact]
    public async Task EnableUserAsync_ReturnsSuccess_WithSameObjectId()
    {
        var result = await _sut.EnableUserAsync("test-oid");

        Assert.True(result.Succeeded);
        Assert.Equal("test-oid", result.ObjectId);
    }

    [Fact]
    public async Task DisableUserAsync_ReturnsSuccess_WithSameObjectId()
    {
        var result = await _sut.DisableUserAsync("test-oid");

        Assert.True(result.Succeeded);
        Assert.Equal("test-oid", result.ObjectId);
    }

    [Fact]
    public async Task DeleteUserAsync_ReturnsSuccess_WithSameObjectId()
    {
        var result = await _sut.DeleteUserAsync("test-oid");

        Assert.True(result.Succeeded);
        Assert.Equal("test-oid", result.ObjectId);
    }

    [Fact]
    public async Task ResetPasswordAsync_ReturnsSuccess_WithSameObjectId()
    {
        var result = await _sut.ResetPasswordAsync("test-oid", "NewP@ss1");

        Assert.True(result.Succeeded);
        Assert.Equal("test-oid", result.ObjectId);
    }

    [Fact]
    public void NoOpImplementsInterface()
    {
        Assert.IsAssignableFrom<IGraphUserService>(_sut);
    }
}
