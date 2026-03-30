using System.ComponentModel.DataAnnotations;

namespace Opplat.Application.Dtos;


public sealed class AdminUserDto : AccountDto
{
    public string TenantId { get; set; } = string.Empty;
    public string TenantIdentifier { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
}

public sealed class AdminCreateUserRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = [];
}

public sealed class AdminUpdateUserRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public bool Active { get; set; }
}

public sealed class AdminSetUserRolesRequest
{
    public List<string> Roles { get; set; } = [];
}

public sealed class AdminSetUserActiveRequest
{
    public bool Active { get; set; }
}

