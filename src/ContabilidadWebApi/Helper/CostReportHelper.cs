using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContabilidadWebApi.ViewModels;
using ContabilidadWebApi.Models;
using ContabilidadWebApi.Data;
using Microsoft.EntityFrameworkCore;
using ContabilidadWebApi.VersatModels;

namespace ContabilidadWebApi.Helper
{
    public class CostReportHelper
    {

        private readonly DbContext _context;
        private readonly VersatDbContext _vcontext;

        public CostReportHelper(VersatDbContext _vcontext, ApiDbContext _context)
        {
            this._context = _context;
            this._vcontext = _vcontext;
        }
        public CostReportHelper(VersatDbContext _vcontext, DbContext _context)
        {
            this._context = _context;
            this._vcontext = _vcontext;
        }

        public void FormatearSalidaGeneral(List<CostosViewModel> Costo, List<IGrouping<string, CostosViewModel>> grupos, out Dictionary<string, Dictionary<string, CostosViewModel>> valores, out Dictionary<string, List<decimal>> totales, out List<decimal> general)
        {
            valores = new Dictionary<string, Dictionary<string, CostosViewModel>>();
            totales = new Dictionary<string, List<decimal>>();
            general = new List<decimal>();
            general.Add(0);
            general.Add(0);
            general.Add(0);
            general.Add(0);
            general.Add(0);
            general.Add(0);


            foreach (var item in grupos)
            {

                var porcEjec = (item.Sum(s => s.PlanMes) == 0) ? 0 : (item.Sum(s => s.RealMes) * 100) / item.Sum(s => s.PlanMes);
                var porcRealEjec = (item.Sum(s => s.PlanAcumulado) == 0) ? 0 : (item.Sum(s => s.RealAcumulado) * 100) / item.Sum(s => s.PlanAcumulado);

                var llave = item.First().Grupo;
                if (!valores.ContainsKey(llave))
                {
                    valores.Add(llave, new Dictionary<string, CostosViewModel>());
                    totales.Add(llave, new List<decimal>());
                    totales[llave].Add(0);
                    totales[llave].Add(0);
                    totales[llave].Add(0);
                    totales[llave].Add(0);
                    totales[llave].Add(0);
                    totales[llave].Add(0);
                    var temp = new CostosViewModel
                    {
                        Grupo = "Total - " + item.First().Grupo,
                        Elementos = item.First().Elementos,
                        PlanAcumulado = item.Sum(s => s.PlanAcumulado),
                        PlanMes = item.Sum(s => s.PlanMes),
                        PorcEjec = porcEjec,
                        PorcRealEjec = porcRealEjec,
                        RealAcumulado = item.Sum(s => s.RealAcumulado),
                        RealMes = item.Sum(s => s.RealMes),
                        SubElementos = item.First().Grupo,
                    };

                    totales[llave][0] += temp.PlanMes;
                    totales[llave][1] += temp.RealMes;
                    totales[llave][2] += temp.PorcEjec;
                    totales[llave][3] += temp.PlanAcumulado;
                    totales[llave][4] += temp.RealAcumulado;
                    totales[llave][5] += temp.PorcRealEjec;
                    general[0] += temp.PlanMes;
                    general[1] += temp.RealMes;
                    general[2] += temp.PorcEjec;
                    general[3] += temp.PlanAcumulado;
                    general[4] += temp.RealAcumulado;
                    general[5] += temp.PorcRealEjec;

                    valores[llave].Add(temp.SubElementos, temp);

                    var pr = Costo.SingleOrDefault(s => s.SubElementos.Trim() == "Piezas de repuesto para mantenimiento");
                    if (pr != null)
                    {
                        if (llave.Trim() == "Materiales auxiliares generales")
                        {
                            valores[llave].Add(pr.SubElementos, pr);
                            totales[llave][0] += pr.PlanMes;
                            totales[llave][1] += pr.RealMes;
                            totales[llave][2] += pr.PorcEjec;
                            totales[llave][3] += pr.PlanAcumulado;
                            totales[llave][4] += pr.RealAcumulado;
                            totales[llave][5] += pr.PorcRealEjec;
                            general[0] += pr.PlanMes;
                            general[1] += pr.RealMes;
                            general[2] += pr.PorcEjec;
                            general[3] += pr.PlanAcumulado;
                            general[4] += pr.RealAcumulado;
                            general[5] += pr.PorcRealEjec;
                        }
                    }


                }

            }
        }
        public void ObtenerCosto(string años, int meses, int areas, string cuenta, out int year, out CentroCostoArea centroArea, out List<CostosViewModel> Costo)
        {

            year = Convert.ToInt32(años);
            var years = year;
            centroArea = _context.Set<CentroCostoArea>().Include(s => s.Area).SingleOrDefault(s => s.Id == areas);
            var centro = centroArea;
            IEnumerable<SubMayor> dataGeneral = _context.Set<SubMayor>().Where(s => s.Cta.Trim() == cuenta.Trim() && s.Ano == years).ToList();
            var data = _context.Set<SubMayor>().Where(s => s.Cta.Trim() == cuenta.Trim() && s.Ano == years && s.Mes == meses).ToList();
            var dataOld = _context.Set<SubMayor>().Where(s => s.Cta.Trim() == cuenta.Trim() && s.Ano == years && s.Mes < meses).ToList();
            string mes = "";
            Costo = new List<CostosViewModel>();

            var plan = _context.Set<Plan>().Where(s => s.CentroCosto == centro.CentroCostoId && s.Fecha.Year == years);



            foreach (var item in dataGeneral)
            {
                if (item.Epigrafe.Trim() == centroArea.CentroCostoId.Trim() && item.Debe > 0 && !plan.Any(s => s.SubAnalisis == item.SubAnalisis && s.CentroCosto.Trim() == item.Epigrafe.Trim()))
                {
                    var cta = _vcontext.ConCuenta.SingleOrDefault(s => s.Clave == item.Cta);
                    var sub = _vcontext.CosSubelementogasto.SingleOrDefault(s => s.Codigo == item.SubAnalisis);
                    var planNew = new Plan()
                    {
                        Enero = 0,
                        Febrero = 0,
                        Marzo = 0,
                        Abril = 0,
                        Mayo = 0,
                        Junio = 0,
                        Julio = 0,
                        Agosto = 0,
                        Septiembre = 0,
                        Octubre = 0,
                        Noviembre = 0,
                        Diciembre = 0,
                        Analisis = item.SubCta,
                        CentroCosto = item.Epigrafe,
                        Cuenta = item.Cta,
                        Elemento = item.Analisis,
                        Fecha = new DateTime(item.Fecha.Year, item.Fecha.Month, item.Fecha.Day, 08, 00, 00),
                        //Fecha = DateTime.Now,
                        SubAnalisis = item.SubAnalisis,
                        SubCuenta = item.SubCta,
                        SubElemento = item.SubAnalisis,
                        Descripcion = sub.Descripcion,
                        CentroCostoAreaId = centroArea.Id

                    };

                    _context.Add(planNew);
                    _context.SaveChanges();
                }
            }

            plan = _context.Set<Plan>().Where(s => s.CentroCosto == centro.CentroCostoId && s.Fecha.Year == years);

            if (plan.Count() == 0 || plan == null)
            {
                plan = _context.Set<Plan>().Where(s => s.CentroCosto == centro.CentroCostoId && s.Fecha.Year == DateTime.Now.Year);
                foreach (var item in plan)
                {
                    item.Enero = 0M;
                    item.Febrero = 0M;
                    item.Marzo = 0M;
                    item.Abril = 0M;
                    item.Mayo = 0M;
                    item.Junio = 0M;
                    item.Julio = 0M;
                    item.Agosto = 0M;
                    item.Septiembre = 0M;
                    item.Octubre = 0M;
                    item.Noviembre = 0M;
                    item.Diciembre = 0M;
                }
            }

            foreach (var item in plan)
            {
                decimal saldoInicial = data.Where(s => s.Epigrafe.Trim() == item.CentroCosto.Trim() && s.SubAnalisis.Trim() == item.SubAnalisis.Trim()).Sum(s => s.Debe);
                decimal saldoCredito = data.Where(s => s.Epigrafe.Trim() == item.CentroCosto.Trim() && s.SubAnalisis.Trim() == item.SubAnalisis.Trim()).Sum(s => s.Haber);
                decimal saldoAcumuladoDeb = dataOld.Where(s => s.Epigrafe.Trim() == item.CentroCosto.Trim() && s.SubAnalisis.Trim() == item.SubAnalisis.Trim()).Sum(s => s.Debe);
                decimal saldoAcumuladoCred = dataOld.Where(s => s.Epigrafe.Trim() == item.CentroCosto.Trim() && s.SubAnalisis.Trim() == item.SubAnalisis.Trim()).Sum(s => s.Haber);
                decimal saldoAcumulado = saldoAcumuladoDeb - saldoAcumuladoCred;
                decimal realAcumulado = saldoAcumulado + (saldoInicial - saldoCredito);

                var subElemento = _vcontext.Set<CosSubelementogasto>().SingleOrDefault(s => s.Codigo == item.SubElemento);
                var elemento = _vcontext.Set<CosElementogasto>().SingleOrDefault(s => s.Idelementogasto == subElemento.Idelementogasto);

                mes = ((Mes)meses).ToString();

                if (Costo.Any(s => s.SubElementos.Trim() == subElemento.Descripcion.Trim()))
                {

                    decimal costP = 0;
                    for (int i = 1; i <= meses; i++)
                    {
                        costP = costP + item[i];
                    }


                    var cost = Costo.SingleOrDefault(s => s.SubElementos.Trim() == subElemento.Descripcion.Trim());
                    cost.PlanMes += item[meses];
                    var x = item.Acumulado(meses);
                    cost.PlanAcumulado = costP;
                }
                if (!Costo.Any(s => s.SubElementos.Trim() == subElemento.Descripcion.Trim()))
                {
                    decimal costP = 0;
                    for (int i = 1; i <= meses; i++)
                    {
                        costP = costP + item[i];
                    }

                                        decimal porc = 0;
                    if (item[meses] != 0)
                    {
                        porc = (saldoInicial * 100) / item[meses];
                    }
                    if (item[meses] == 0)
                    {
                        porc = 0;
                    }
                    var descripcionGrupo = "";
                    var grupoSub = _context.Set<GrupoSubElemento_SubElemento>().SingleOrDefault(s => s.SubElementoGastoId.Trim() == subElemento.Codigo.Trim());

                    if (grupoSub != null)
                    {
                        var grupo = _context.Set<GrupoSubelemento>().SingleOrDefault(s => s.Id == grupoSub.GrupoSubelementoId);
                        if (grupo != null)
                        {
                            descripcionGrupo = grupo.Nombre;
                        }

                    }

                    Costo.Add(new CostosViewModel
                    {
                        Grupo = descripcionGrupo,
                        Elementos = elemento.Descripcion,
                        SubElementos = subElemento.Descripcion,

                        PlanMes = item[meses],
                        RealMes = saldoInicial - saldoCredito,
                        PorcEjec = porc,

                        PlanAcumulado = costP,
                        RealAcumulado = saldoAcumulado + (saldoInicial - saldoCredito),
                        PorcRealEjec = porc,
                    });
                }

            }
        }

       
        /// <summary>
        /// Devuelve una lista de Meses
        /// </summary>
        /// <returns></returns>
        public List<string> GetMonths()
        {
            List<string> months = new List<string>();
            for (int i = 1; i < 13; i++)
            {
                months.Add(((Mes)i).ToString());
            }

            return months;
        }


        public List<dynamic> MonthsList()
        {
            List<dynamic> result = new List<dynamic>();
            var months = GetMonths();
            foreach (var m in months)
            {
                var item = new
                {
                    Id = months.IndexOf(m) + 1,
                    Nombre = m
                };
                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Devuelve el mes asociado a un valor entero entre 1 y 12.
        /// </summary>
        /// <param name="month">Valor asociado al mes.</param>
        /// <returns>Nombre del Mes o la cadena vacia en caso de que el valor no este asociado a ningun mes.</returns>
        public string GetAMonth(int month = 0)
        {
            return (month > 0 && month < 13) ? ((Mes)month).ToString() : "";
        }


        public IEnumerable<dynamic> GetAreas()
        {
            return _context.Set<Area>().Select(a => new { Id = a.Id, Detalles = a.Detalles }).ToList();
        }

        public Area GetArea(int idArea)
        {
            var area = _context.Set<Area>().FirstOrDefault(a => a.Id == idArea);

            return area;
        }

        //public List<CostosViewModel> GetMonthlyCost(Area area, int month, int year,string cuenta)
        //{

        //    var centers = _context.Set<CentroCostoArea>().Where(c => c.AreaId == area.Id).Select(c => c.CentroCostoId).ToList<string>();

        //    var plans = GetYearlyPlan(year, centers, cuenta).GroupBy(p => p.SubElemento);
        //    var cost = GetYearlySimpleCost(cuenta, year, centers);

        //    List<CostosViewModel> values = new List<CostosViewModel>();
        //    FillMonthlyCost(month, plans, cost, values);

        //    return values;

        //}

        //private void FillMonthlyCost(int month, IEnumerable<IGrouping<string, Plan>> plans, List<Submayor> cost, List<CostosViewModel> values)
        //{
        //    foreach (var item in plans)
        //    {
        //        var costo = new CostosViewModel();
        //        string subelemento = item.First().SubElemento.Trim();
        //        costo.Elementos = item.First().Elemento;
        //        costo.SubElementos = subelemento;
        //        costo.PlanMes = item.Sum(i => i[month]);
        //        costo.PlanAcumulado = item.Sum(i => i.Acumulado(month));
        //        costo.RealAcumulado = cost.Where(c => c.SubAnalisis == subelemento && c.Mes < month).Sum(c => c.Debe);
        //        costo.RealMes = cost.Where(c => c.SubAnalisis == subelemento && c.Mes == month).Sum(c => c.Debe);

        //        var subgrupo = _context.Set<GrupoSubElemento_SubElemento>().FirstOrDefault(s => s.SubElementoGastoId == subelemento);
        //        var grupo = _vcontext.Set<SubelementoGasto>().First(s => s.Id_SubElemGasto.Trim() == subelemento).Desc_Subelemento.Trim();
        //        if (subgrupo != null)
        //        {
        //            grupo = _context.Set<GrupoSubelemento>().First(g => g.Id == subgrupo.GrupoSubelementoId).Nombre;
        //        }
        //        costo.Grupo = grupo;


        //        values.Add(costo);

        //    }
        //}

        //public Dictionary<int, List<CostosViewModel>> GetYearlyCost(Area area,int year, string cuenta)
        //{
        //    var centers = _context.Set<CentroCostoArea>().Where(c => c.AreaId == area.Id).Select(c => c.CentroCostoId).ToList<string>();

        //    var plans = GetYearlyPlan(year, centers, cuenta).GroupBy(p => p.SubElemento);
        //    var cost = GetYearlySimpleCost(cuenta, year, centers);

        //    Dictionary<int, List<CostosViewModel>> values = new Dictionary<int, List<CostosViewModel>>();
        //    for (int i = 1; i < 13; i++)
        //    {
        //        if (!values.ContainsKey(i))
        //        {
        //            values.Add(i, new List<CostosViewModel>());
        //        }
        //        FillMonthlyCost(i, plans, cost, values[i]);
        //    }

        //    return values;

        //}

        public Dictionary<string, Dictionary<string, Dictionary<string, List<decimal>>>> FormatYearlyReport(Dictionary<string, List<decimal>> values)
        {
            Dictionary<string, Dictionary<string, Dictionary<string, List<decimal>>>> table = new Dictionary<string, Dictionary<string, Dictionary<string, List<decimal>>>>();
            var elementos = _vcontext.Set<CosElementogasto>().ToList();
            var subelementos = _vcontext.Set<CosSubelementogasto>().ToList();

            foreach (var item in values)
            {
                string llave = item.Key.Trim();
                if (llave == "800302")
                {
                    llave = "800202";
                }
                var subE = subelementos.Single(s => s.Codigo.Trim() == llave);
                var ele = elementos.Single(e => e.Idelementogasto == subE.Idelementogasto);


                string llaveE = ele.Descripcion;
                string llaveS = subE.Descripcion;

                if (!table.ContainsKey(llaveE))
                {
                    table.Add(llaveE, new Dictionary<string, Dictionary<string, List<decimal>>>());
                }
                if (!table[llaveE].ContainsKey(llaveS))
                {
                    table[llaveE].Add(llaveS, new Dictionary<string, List<decimal>>());
                }
                if (!table[llaveE][llaveS].ContainsKey(llave))
                {
                    table[llaveE][llaveS].Add(llave, item.Value);
                }

            }
            return table;

        }

        public Dictionary<string, List<decimal>> FormatYearlyPlanTotals(Dictionary<string, Dictionary<string, Dictionary<string, List<decimal>>>> table)
        {
            Dictionary<string, List<decimal>> totals = new Dictionary<string, List<decimal>>();
            decimal[] general = new decimal[13];
            foreach (var element in table)
            {
                decimal[] elements = new decimal[13];
                foreach (var sub in element.Value)
                {
                    decimal[] values = new decimal[13];
                    foreach (var item in sub.Value)
                    {
                        for (int i = 0; i < item.Value.Count; i++)
                        {
                            values[i] += item.Value[i];
                            elements[i] += item.Value[i];
                            general[i] += item.Value[i];
                        }
                    }

                    totals.Add(sub.Key, new List<decimal>(values));
                }
                totals.Add(element.Key, new List<decimal>(elements));
            }
            totals.Add("Total", new List<decimal>(general));
            return totals;
        }
        /// <summary>
        /// Obtiene el reporte consolidado para el año
        /// </summary>
        /// <param name="area"></param>
        /// <param name="year"></param>
        /// <param name="cuenta"></param>
        /// <returns></returns>
        public Dictionary<string, List<decimal>> PlanYearlyReport(Area area, int year, string cuenta)
        {
            var centers = _context.Set<CentroCostoArea>().Where(c => c.AreaId == area.Id).Select(c => c.CentroCostoId).ToList<string>();

            var plans = GetYearlyPlan(year, centers, cuenta).GroupBy(p => p.SubElemento);

            Dictionary<string, List<decimal>> values = new Dictionary<string, List<decimal>>();
            foreach (var item in plans)
            {
                var key = item.Key;
                if (!values.ContainsKey(key))
                {
                    values.Add(key, new List<decimal>());

                }
                decimal total = 0;
                for (int i = 1; i < 13; i++)
                {
                    decimal plan = item.Sum(j => j[i]);
                    total += plan;
                    values[key].Add(plan);
                }
                values[key].Add(total);
            }

            return values;
        }

        //public Dictionary<string, List<decimal>> CostYearlyReport(Area area, int year, string cuenta)
        //{

        //    var centers = _context.Set<CentroCostoArea>().Where(c => c.AreaId == area.Id).Select(c => c.CentroCostoId).ToList<string>();

        //    var cost = GetYearlySimpleCost(cuenta, year, centers).GroupBy(c => c.SubAnalisis);

        //    Dictionary<string, List<decimal>> values = new Dictionary<string, List<decimal>>();
        //    foreach (var item in cost)
        //    {
        //        var key = item.Key;
        //        if (!values.ContainsKey(key))
        //        {
        //            values.Add(key, new List<decimal>());

        //        }
        //        decimal total = 0;
        //        for (int i = 1; i < 13; i++)
        //        {
        //            decimal costo = item.Sum(j => j[i]);
        //            total += costo;
        //            values[key].Add(costo);
        //        }
        //        values[key].Add(total);
        //    }

        //    return values;
        //}




        //public List<SubMayor> GetYearlySimpleCost(string cuenta, int year, List<string> center)
        //{
        //    return _context.Set<SubMayor>().Where(s => s.Ano == year && s.Cta.Trim() == cuenta && center.Contains(s.Epigrafe)).ToList<SubMayor>();

        //}

        /// <summary>
        /// Obtiene todos las planes anuales para una serie de centros de costos
        /// </summary>
        /// <param name="year">Año Deseado</param>
        /// <param name="centers">Lista de centro de costos</param>
        /// <param name="cuenta">Cuenta de pago</param>
        /// <returns></returns>
        public List<Plan> GetYearlyPlan(int year, List<string> centers, string cuenta)
        {
            return _context.Set<Plan>().Where(p => p.Fecha.Year == year && centers.Contains(p.CentroCosto) && p.Cuenta == cuenta).ToList<Plan>();

        }



    }
}
