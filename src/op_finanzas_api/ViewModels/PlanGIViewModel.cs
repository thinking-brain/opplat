using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace op_finanzas_api.ViewModels
{
    public class PlanGIViewModel
    {
        public string Grupo { get; set; }
        public decimal PlanMes { get; set; }
        public decimal RealMes { get; set; }
        public decimal PorcCumplimiento { get; set; }

        public decimal PorcRelacionIngresos { get; set; }
        public decimal PorcGastosFuncionTotal { get; set; }

        public decimal PlanAcumulado { get; set; }
        public decimal RealAcumulado { get; set; }
        public decimal PorcCumpAcumulado { get; set; }


        public decimal PorcIngresosFuncionTotal { get; set; }
        public decimal PorcGastosFuncionTotalAcumulado { get; set; }


    }
}
