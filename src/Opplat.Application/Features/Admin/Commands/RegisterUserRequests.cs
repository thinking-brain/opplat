using Opplat.Application.Abstractions.Identity;
using Opplat.Application.Abstractions.Messaging;

namespace Opplat.Application.Features.Admin.Commands;

public sealed record RegisterUserCommand(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Password) : ICommand<RegisterUserResult>;

public sealed record RegisterUserResult(bool Succeeded, string? UserId, string? ErrorMessage)
{
    public static RegisterUserResult Success(string? userId = null) =>
        new(true, userId, null);

    public static RegisterUserResult Failure(string errorMessage) =>
        new(false, null, errorMessage);
}

public sealed class RegisterUserCommandHandler(IKeycloakUserService keycloakUserService)
    : ICommandHandler<RegisterUserCommand, RegisterUserResult>
{
    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var keycloakRequest = new CreateKeycloakUserRequest
        {
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Password = request.Password,
            Enabled = true,
            EmailVerified = false
        };

        var result = await keycloakUserService.CreateUserAsync(keycloakRequest, cancellationToken);

        return result.Succeeded
            ? RegisterUserResult.Success(result.UserId)
            : RegisterUserResult.Failure(result.ErrorMessage ?? "Registration failed.");
    }
}
