using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opplat.Contabilidad.Domain.Entities;
public class Aft
{
    public int Id { get; set; }

    public string Descripcion { get; set; }

    public DateTime FechaDeEntrada { get; set; }

    public decimal ValorInicial { get; set; }

    public decimal ValorActual { get; set; }

    public bool Baja { get; set; }

    public DateTime? FechaDeBaja { get; set; }
}

