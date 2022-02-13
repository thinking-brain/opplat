using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ContratacionWebApi.Models;

public class Revision
    {
        public string Fichero { get; set; }

        [Display(Name = "Administrador")]
        public int AdminContratoId { get; set; }
        public virtual AdminContrato AdminContrato { get; set; }
}