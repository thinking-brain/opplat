namespace Opplat.Application.Dtos;

public class CambiarRolesDto
{
    public string idUsuario { get; set; } = string.Empty;
    public List<string> Roles { get; set; }

    public CambiarRolesDto()
    {
        Roles = [];
    }
}
