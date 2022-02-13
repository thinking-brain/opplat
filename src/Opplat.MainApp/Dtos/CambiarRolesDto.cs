namespace Opplat.MainApp.Dtos;

public class CambiarRolesDto
{
    public string idUsuario { get; set; } = String.Empty;
    public List<string> Roles { get; set; }

    public CambiarRolesDto()
    {
        Roles = new List<string>();
    }
}
