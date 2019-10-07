using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContabilidadWebApi.Models
{
    [Table("contb_cuentas")]
    public class Cuenta
    {
        public int Id { get; set; }

        [Required]
        public string NumeroParcial { get; set; }

        [Required]
        public string Nombre { get; set; }

        public int? CuentaSuperiorId { get; set; }

        public virtual Cuenta CuentaSuperior { get; set; }

        public virtual ICollection<Cuenta> Subcuentas { get; set; }

        public Naturaleza Naturaleza { get; set; }

        public virtual ICollection<Movimiento> Movimientos { get; set; }

        public Cuenta()
        {
            Movimientos = new HashSet<Movimiento>();
        }

        [NotMapped]
        public string Numero
        {
            get
            {
                var numero = NumeroParcial;
                var nivel = CuentaSuperior;
                while (nivel != null)
                {
                    numero = nivel.NumeroParcial + "-" + numero;
                    nivel = nivel.CuentaSuperior;
                }
                return numero;
            }
        }

        [NotMapped]
        public bool EsFinal
        {
            get { return Subcuentas.Count == 0; }
        }

    }
}
