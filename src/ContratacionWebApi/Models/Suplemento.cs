
namespace ContratacionWebApi.Models
{
    public class Suplemento: DocumentoDeContrato
    {
       public int ContratoId { get; set; }

        public virtual Contrato Contrato { get; set; }
    }
}