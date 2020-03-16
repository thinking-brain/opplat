using System.ComponentModel.DataAnnotations.Schema;

namespace ContratacionWebApi.Models
{
    public class FormaDePago
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}