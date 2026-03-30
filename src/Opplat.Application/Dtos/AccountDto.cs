namespace Opplat.Application.Dtos;
public class AccountDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public bool Active { get; set; }
    public List<string> Roles { get; set; }

    public AccountDto()
    {
        Roles = [];
    }
}
