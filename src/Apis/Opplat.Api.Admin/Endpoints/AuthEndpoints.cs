using System.ComponentModel.DataAnnotations;
using Opplat.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Mvc;
using Opplat.Application.Features.Admin.Commands;

namespace Opplat.Api.Admin.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var auth = app.MapGroup("/auth")
            .WithTags("Auth");

        auth.MapPost("/register",
            async ([FromBody] RegisterRequest request, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            {
                var validationErrors = ValidateRegisterRequest(request);
                if (validationErrors.Count > 0)
                    return Results.ValidationProblem(validationErrors);

                var command = new RegisterUserCommand(
                    Username: request.Username,
                    Email: request.Email,
                    FirstName: request.FirstName,
                    LastName: request.LastName,
                    Password: request.Password);

                var result = await mediator.Send(command, cancellationToken);

                return result.Succeeded
                    ? Results.Ok(new RegisterResponse(result.UserId!, "User registered successfully. You can now log in."))
                    : Results.BadRequest(new { error = result.ErrorMessage });
            })
            .WithSummary("Register a new user account")
            .AllowAnonymous();
    }

    private static Dictionary<string, string[]> ValidateRegisterRequest(RegisterRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Username))
            errors["username"] = ["Username is required."];
        else if (request.Username.Length < 3)
            errors["username"] = ["Username must be at least 3 characters."];

        if (string.IsNullOrWhiteSpace(request.Email) || !new EmailAddressAttribute().IsValid(request.Email))
            errors["email"] = ["A valid email address is required."];

        if (string.IsNullOrWhiteSpace(request.FirstName))
            errors["firstName"] = ["First name is required."];

        if (string.IsNullOrWhiteSpace(request.LastName))
            errors["lastName"] = ["Last name is required."];

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
            errors["password"] = ["Password must be at least 8 characters."];

        return errors;
    }
}

public sealed record RegisterRequest(
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string Password);

public sealed record RegisterResponse(string UserId, string Message);
