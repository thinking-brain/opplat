using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace op_costos_api.ViewModels
{
    public class PlanGIViewModel
    {
        public string Grupo { get; set; }
        public decimal PlanMes { get; set; }
        public decimal RealMes { get; set; }
        public decimal PorcCumplimiento { get; set; }
        public decimal PorcRelacionIngresos { get; set; }
        public decimal PlanAcumulado { get; set; }
        public decimal RealAcumulado { get; set; }
        public decimal PorcCumpAcumulado { get; set; }
        //public decimal PorcRealEjec { get; set; }
        public decimal TotalGrupo { get; set; }
        //public decimal TotalEgresosEnMes { get; set; }
    }
}
