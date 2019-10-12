using System;
using ContabilidadWebApi.Models;

namespace ContabilidadWebApi.Dtos
{
    public class MovimientoDto
    {
        public int Id { get; set; }
        public string Cuenta { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public decimal Importe { get; set; }
        public TipoDeOperacion TipoDeOperacion { get; set; }
    }
}