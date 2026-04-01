using Opplat.Domain.Entities;

namespace Opplat.Domain.Models;

public class User : BaseEntity
{
    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;

    public bool IsActive { get; set; }
}
