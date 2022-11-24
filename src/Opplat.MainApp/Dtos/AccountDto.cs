namespace Opplat.MainApp.Dtos;
public class AccountDto
{
    public string UserId { get; set; } = String.Empty;
    public string Name { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;
    public string Username { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;

    public bool Active { get; set; }
    public List<string> Roles { get; set; }

    public AccountDto()
    {
        Roles = new List<string>();
    }
}
