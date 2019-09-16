using System.Collections.Generic;

namespace Account.WebApi.Dtos
{
    public class CambiarRolesDto
    {
        public string idUsuario { get; set; }
        public List<string> Roles { get; set; }

        public CambiarRolesDto()
        {
            Roles = new List<string>();
        }
    }
}