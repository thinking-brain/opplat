using Microsoft.EntityFrameworkCore;
using op_costos_api.Data;
using op_costos_api.Helpers;
using op_costos_api.Models;
using op_costos_api.VersatModels;
using op_costos_api.VersatModels2;
using op_costos_api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace op_costos_api.Helper
{
    public class ObtenerPronosticoProductivo
    {
        private readonly DbContext _context;
        private readonly VersatDbContext _vcontext;
        private readonly VersatDbContext2 _v2context;

        public ObtenerPronosticoProductivo(VersatDbContext _vcontext, VersatDbContext2 _v2context, ApiDbContext _context)
        {
            this._context = _context;
            this._vcontext = _vcontext;
            this._v2context = _v2context;

        }
        public ObtenerPronosticoProductivo(VersatDbContext _vcontext, VersatDbContext2 _v2context, DbContext _context)
        {
            this._context = _context;
            this._vcontext = _vcontext;
            this._v2context = _v2context;

        }

        public List<PronosticoProductivoVM> Obtener(int year, int meses)
        {
            var plan = new List<PronosticoProductivoVM>();

            //Tabla de Movimientos en el Opplat (SUBMAYOR-CUENTA)
            var data = _context.Set<SubMayorCuenta>().ToList();

            //Buscando el Primer Periodo del Año consultado
            var periodoInicioAño = _context.Set<SubMayorCuenta>().OrderBy(s => s.Año).Where(s => s.Año == year).ToList().First();

            //Seleccionando solo los movimientos dentro del periodo del mes consultado
            var dataInMes = data.Where(s => s.Año == year && s.Mes == meses).ToList();

            //Seleccionando todos los movimientos que esten entre el comienzo del año y sean menores e igual al mes consultado
            var dataInMesesAnteriores = data.Where(s => s.Año == year && s.Mes <= meses && s.Mes >= periodoInicioAño.Mes).ToList();

            //Tabla que contiene los planes de Pronosticos Productivos
            var planesPP = _context.Set<PlanPronosticoProductivo>().Where(s => s.Año == Convert.ToString(year));

            foreach (var item in dataInMes)
            {
                var cuentaNat = _vcontext.ConCuentanat.SingleOrDefault(s => s.Idcuenta == item.IdCuenta);
                var clav = item.ClaveCuenta.TrimEnd().Split(' ');
                if (cuentaNat.Naturaleza == -1 && clav.Length > 1)
                {
                    var cuenta = _vcontext.ConCuenta.SingleOrDefault(s => s.Idcuenta == item.IdCuenta);
                    string moneda = _vcontext.ConCuentamoneda.SingleOrDefault(s => s.Idcuenta == cuenta.Idcuenta).Idmoneda == 2 ? "CUC" : "CUP";
                    var conceptosCta = _context.Set<ConceptoCuentas>().SingleOrDefault(s => s.CuentaId == cuenta.Idcuenta);
                    if (conceptosCta != null)
                    {
                        var conceptosPlan = _context.Set<ConceptoPlan>().SingleOrDefault(s => s.Id == conceptosCta.ConceptoPlanId);

                        if (conceptosPlan != null)
                        {
                            var planes = _context.Set<PlanPronosticoProductivo>().Where(s => s.ConceptoPlanId == conceptosPlan.Id);

                            foreach (var pp in planes)
                            {

                                if (!plan.Any(s => s.Obra.Trim().ToUpper().Equals(pp.ConceptoPlan.Concepto.Trim().ToUpper())))
                                {

                                    if (moneda.Equals("CUC"))
                                    {
                                        plan.Add(new PronosticoProductivoVM { Obra = pp.ConceptoPlan.Concepto, CUC = (item.Importe * -1), PlanMes = planes != null ? pp[meses] : 0M });
                                    }
                                    if (moneda.Equals("CUP"))
                                    {
                                        plan.Add(new PronosticoProductivoVM { Obra = pp.ConceptoPlan.Concepto, CUP = (item.Importe * -1), PlanMes = planes != null ? pp[meses] : 0M });
                                    }
                                }
                                else
                                {
                                    var p = plan.SingleOrDefault(s => s.Obra.Trim().ToUpper().Equals(pp.ConceptoPlan.Concepto.Trim().ToUpper()));
                                    if (moneda.Equals("CUC"))
                                    {
                                        p.CUC = p.CUC + (item.Importe * -1);
                                    }
                                    if (moneda.Equals("CUP"))
                                    {
                                        p.CUP = p.CUP + (item.Importe * -1);
                                    }
                                }

                            }
                        }
                    }
                   
                    //var planes = _context.Set<PlanPronosticoProductivo>().Where(s => s.CuentaMN == Convert.ToString(cuentaNat.Idcuenta).Trim() || s.CuentaCUC == Convert.ToString(cuentaNat.Idcuenta).Trim());


                }
            }
            return plan;
        }

    }

}
