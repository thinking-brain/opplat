using System.ComponentModel.DataAnnotations;

namespace ContabilidadWebApi.Models
{
    public class DetallePlanGI
    {
        public int Id { get; set; }

        public int PlanId { get; set; }

        public virtual PlanGI Plan { get; set; }

        public int ConceptoId { get; set; }

        public virtual ConceptoPlan Concepto { get; set; }

        [DataType(DataType.Currency)]
        public decimal Enero { get; set; }

        [DataType(DataType.Currency)]
        public decimal Febrero { get; set; }

        [DataType(DataType.Currency)]
        public decimal Marzo { get; set; }

        [DataType(DataType.Currency)]
        public decimal Abril { get; set; }

        [DataType(DataType.Currency)]
        public decimal Mayo { get; set; }

        [DataType(DataType.Currency)]
        public decimal Junio { get; set; }

        [DataType(DataType.Currency)]
        public decimal Julio { get; set; }

        [DataType(DataType.Currency)]
        public decimal Agosto { get; set; }

        [DataType(DataType.Currency)]
        public decimal Septiembre { get; set; }

        [DataType(DataType.Currency)]
        public decimal Octubre { get; set; }

        [DataType(DataType.Currency)]
        public decimal Noviembre { get; set; }

        [DataType(DataType.Currency)]
        public decimal Diciembre { get; set; }

        public decimal this[int index]
        {
            get
            {
                switch (index)
                {
                    case 1: return Enero;
                    case 2: return Febrero;
                    case 3: return Marzo;
                    case 4: return Abril;
                    case 5: return Mayo;
                    case 6: return Junio;
                    case 7: return Julio;
                    case 8: return Agosto;
                    case 9: return Septiembre;
                    case 10: return Octubre;
                    case 11: return Noviembre;
                    case 12: return Diciembre;
                    default: return 0;
                }
            }
        }
    }
}