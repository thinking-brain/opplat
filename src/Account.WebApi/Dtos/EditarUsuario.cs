using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Account.WebApi.Dtos
{
    public class EditarUsuario
    {
        public string Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
    }
}