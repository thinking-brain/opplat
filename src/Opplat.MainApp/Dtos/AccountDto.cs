namespace Opplat.MainApp.Dtos;
public class AccountDto
{
    public string UserId { get; set; } = String.Empty;
    public string Nombres { get; set; } = String.Empty;
    public string Apellidos { get; set; } = String.Empty;
    public string Username { get; set; } = String.Empty;

    public bool Activo { get; set; }
    public List<string> Roles { get; set; }

    public AccountDto()
    {
        Roles = new List<string>();
    }
}
