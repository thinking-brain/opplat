using System.Collections.Generic;

namespace Account.WebApi.Dtos
{
    public class AccountDto
    {
        public string UserId { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Username { get; set; }

        public bool Activo { get; set; }
        public List<string> Roles { get; set; }

        public AccountDto()
        {
            Roles = new List<string>();
        }
    }
}