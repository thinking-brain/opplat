using System.ComponentModel.DataAnnotations;

namespace Opplat.Domain.Entities.Administration;

public sealed class AdminTenantInfo : BaseEntity
{
    [Required]
    public string Identifier { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string DatabaseName { get; set; } = string.Empty;

    [Required]
    public string DatabaseSchema { get; set; } = string.Empty;

    public int UserCount { get; set; }

    public bool IsActive { get; set; } = true;
}
