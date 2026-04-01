namespace Opplat.Domain.Models;

public class User
{
    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public Guid Id { get; set; }

    public bool IsActive { get; set; }
}
