using System.ComponentModel.DataAnnotations;
using ContabilidadWebApi.Models;

namespace ContabilidadWebApi.Dtos
{
    public class NuevaCuentaDto
    {
        [Required]
        public string Numero { get; set; }

        [Required]
        public string Nombre { get; set; }

        public Naturaleza Naturaleza { get; set; }
    }
}