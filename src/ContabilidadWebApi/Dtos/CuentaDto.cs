using System.Collections.Generic;
using ContabilidadWebApi.Models;

namespace ContabilidadWebApi.Dtos
{
    public class CuentaDto
    {
        public int Id { get; set; }

        public string Numero { get; set; }

        public string Descripcion { get; set; }

        public Naturaleza Naturaleza { get; set; }

        public CuentaDto Padre { get; set; }

        public List<CuentaDto> Subcuentas { get; set; }

        public CuentaDto()
        {
            Subcuentas = new List<CuentaDto>();
        }
    }
}