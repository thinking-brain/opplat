using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;
using RhWebApi.Utils;

namespace RhWebApi.Models {
    [Table ("puestos_de_trabajos")]
    public class PuestoDeTrabajo : ITreeNode {
        public int Id { get; set; }

        public int CargoId { get; set; }
        public virtual Cargo Cargo { get; set; }

        public int UnidadOrganizativaId { get; set; }
        public virtual UnidadOrganizativa UnidadOrganizativa { get; set; }

        public string Descripcion { get; set; }

        public int CantidadPorPlantilla { get; set; }

        public int? JefeId { get; set; }
        public virtual PuestoDeTrabajo Jefe { get; set; }

        public virtual ICollection<PuestoDeTrabajo> Subordinados { get; set; }
        public virtual ICollection<HistoricoPuestoDeTrabajo> Historicos { get; set; }

        public PuestoDeTrabajo () {
            Subordinados = new HashSet<PuestoDeTrabajo> ();
            Trabajadores = new HashSet<Trabajador> ();
            Historicos = new HashSet<HistoricoPuestoDeTrabajo> ();
        }

        public ITreeNode GetParent () {
            return Jefe;
        }

        public IEnumerable<ITreeNode> GetChildrens () {
            return Subordinados;
        }

        public virtual ICollection<Trabajador> Trabajadores { get; set; }

        [NotMapped]
        public string Detalle {
            get { return Cargo.Nombre + " - " + UnidadOrganizativa.Detalle; }
        }
    }
}