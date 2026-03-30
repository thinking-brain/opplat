using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Accounting;

public class Cuenta : BaseEntity
{
    public Guid NivelId { get; set; }

    public virtual Nivel? Nivel { get; set; }

    public Naturaleza Naturaleza { get; set; }

    public virtual Disponibilidad? Disponibilidad { get; set; }

    public virtual ICollection<Movimiento> Movimientos { get; set; } = [];

    [NotMapped]
    public string Numero
    {
        get
        {
            if (Nivel == null)
            {
                return "+++" + Id;
            }
            var numero = Nivel.Numero;
            var nivel = Nivel.NivelSuperior;
            while (nivel != null)
            {
                numero = nivel.Numero + "-" + numero;
                nivel = nivel.NivelSuperior;
            }
            return numero;
        }
    }

    [NotMapped]
    public string Nombre
    {
        get { return Nivel != null ? Nivel.Nombre : "Cuenta " + Id; }
    }

    [NotMapped]
    public bool EsValida
    {
        get { return Nivel != null && Nivel.NivelesInferiores.Count == 0; }
    }

}

