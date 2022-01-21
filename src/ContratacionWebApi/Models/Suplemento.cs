
namespace ContratacionWebApi.Models
{
    public class Suplemento: Documento
    {
       public int ContratoId { get; set; }

        public virtual Contrato Contrato { get; set; }
    }
}