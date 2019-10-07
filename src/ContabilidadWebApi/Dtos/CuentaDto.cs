using System.Collections.Generic;
using ContabilidadWebApi.Models;
using Newtonsoft.Json;

namespace ContabilidadWebApi.Dtos
{
    public class CuentaDto
    {
        public int Id { get; set; }

        public string Numero { get; set; }

        public string Descripcion { get; set; }

        public Naturaleza Naturaleza { get; set; }

        [JsonIgnore]
        public CuentaDto Padre { get; set; }

        [JsonIgnore]
        public List<CuentaDto> Subcuentas { get; set; }

        public CuentaDto()
        {
            Subcuentas = new List<CuentaDto>();
        }
    }
}