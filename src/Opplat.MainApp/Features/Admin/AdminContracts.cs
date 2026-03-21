using System.ComponentModel.DataAnnotations;
using Opplat.MainApp.Dtos;

namespace Opplat.MainApp.Features.Admin;

public sealed class AdminTenantDto
{
    public string Id { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public sealed class UpsertTenantRequest
{
    public string? Id { get; set; }

    [Required]
    public string Identifier { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

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

    public List<string> Roles { get; set; } = new();
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
    public List<string> Roles { get; set; } = new();
}

public sealed class AdminSetUserActiveRequest
{
    public bool Active { get; set; }
}
