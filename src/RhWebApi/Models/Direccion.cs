using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("direcciones")]
    public class Direccion
    {
        public int Id { get; set; }

        public string Municipio { get; set; }

        public string Provincia { get; set; }

        public string Calle { get; set; }

        public int? Piso { get; set; }

        public string Rpto { get; set; }

        public string No { get; set; }

        public string Apto { get; set; }

        [Display(Name = "Entre")]
        public string EntreCalle1 { get; set; }

        [Display(Name = "Y")]
        public string EntreCalle2 { get; set; }

        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        public override string ToString()
        {
            return String.Format("Calle {0} # {1} e/ {2} y {3}, {4}, {5}", Calle, No, EntreCalle1, EntreCalle2, Municipio, Provincia);
        }
        [NotMapped]
        private string resumen;

        [NotMapped]
        public string Resumen
        {
            get { return String.Format("Calle {0} # {1} e/ {2} y {3}", Calle, No, EntreCalle1, EntreCalle2); }
            set { resumen = value; }
        }

        [NotMapped]
        public string ResumenCompleto
        {
            get { return String.Format("Calle {0} # {1} e/ {2} y {3}, {4}, {5}", Calle, No, EntreCalle1, EntreCalle2, Municipio, Provincia); }
            set { resumen = value; }
        }

        public virtual ICollection<Trabajador> Trabajadores { get; set; }

        public Direccion()
        {
            Trabajadores = new HashSet<Trabajador>();
        }
    }
}