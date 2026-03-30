namespace Opplat.Application.Dtos;

public class ChangeRolesDto
{
    public string UserId { get; set; } = string.Empty;
    public List<string> Roles { get; set; }

    public ChangeRolesDto()
    {
        Roles = [];
    }
}
