using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Contabilidad.Domain.Entities;

namespace Opplat.Contabilidad.Domain.Dtos;

public class OperacionDto
{
    public string Tipo { get; set; }
    public TipoDeOperacion TipoDeOperacion{ get; set; }
    public string Descripcion { get; set; }
    public decimal Importe { get; set; }
    public DateTime Fecha { get; set; }
    public string Usuario { get; set; }
}
