namespace Opplat.Application.Abstractions.Admin;

public sealed class AdminSessionUserDto
{
    public string UserId { get; set; } = string.Empty;
    public string ObjectId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

public sealed class AdminSessionDto
{
    public bool IsAuthenticated { get; set; }
    public bool ShellModeEnabled { get; set; }
    public string AuthenticationMode { get; set; } = string.Empty;
    public DateTimeOffset? ExpiresAtUtc { get; set; }
    public string LoginPath { get; set; } = string.Empty;
    public string LogoutPath { get; set; } = string.Empty;
    public string CsrfHeaderName { get; set; } = string.Empty;
    public string? AccessToken { get; set; }
    public AdminSessionUserDto? User { get; set; }
}

public sealed class AdminCsrfTokenDto
{
    public string HeaderName { get; set; } = string.Empty;
    public string RequestToken { get; set; } = string.Empty;
}
