using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContabilidadWebApi.ViewModels;
using ContabilidadWebApi.Models;
using ContabilidadWebApi.Data;
using Microsoft.EntityFrameworkCore;
//using ContabilidadWebApi.VersatModels;
using ContabilidadWebApi.VersatModels2;

namespace ContabilidadWebApi.Helper
{
    public class UpdateSubmayor2Helper
    {

        private readonly DbContext _context;
        private readonly VersatDbContext2 _v2context;

        public UpdateSubmayor2Helper(VersatDbContext2 _v2context, ApiDbContext _context)
        {
            this._context = _context;
            this._v2context = _v2context;
        }
        public UpdateSubmayor2Helper(VersatDbContext2 _v2context, DbContext _context)
        {
            this._context = _context;
            this._v2context = _v2context;
        }
        public List<SubMayor> LlenarSubMayor2()
        {

            var submayor = _context.Set<SubMayor>().ToList();
            //var periodoUltimo = _vcontext.Set<GenPeriodo>().Last();
            //var subUltimo = submayor.Any() ? _context.Set<SubMayor>().Where(s=> s.Ano == periodoUltimo.Inicio.Year).ToList().Last() : null;
            //var dtos = submayor.Any() ? _vcontext.Set<OptCuentaCentroSubPeriodo>().Where(s=>s.IdperiodoNavigation.Inicio.Year >= subUltimo.Ano && s.IdperiodoNavigation.Inicio.Month >= subUltimo.Mes).OrderBy(s => s.Idperiodo).ToList() : _vcontext.Set<OptCuentaCentroSubPeriodo>().OrderBy(s=>s.Idperiodo).ToList();

            //var submayor = _context.Set<SubMayor>().ToList();
            //var periodoUltimo = _vcontext.Set<GenPeriodo>().Last();
            //var subUltimo = submayor.Any() ? _context.Set<SubMayor>().Where(s => s.Ano == periodoUltimo.Inicio.Year).ToList().Last() : null;

            var dtos = _v2context.Set<OptCuentaCentroSubPeriodo>().OrderBy(s => s.Idperiodo).ToList();


            foreach (var item in dtos)
            {               
                ///PERIODO
                var periodo = _v2context.Set<GenPeriodo>().SingleOrDefault(s => s.Idperiodo == item.Idperiodo);
                short ano = Convert.ToInt16(periodo.Inicio.Year);
                var mesx = Convert.ToByte(periodo.Inicio.Month);
                //SUBELEMENTO
                var sub = _v2context.Set<CosSubelementogasto>().SingleOrDefault(s => s.Idsubelemento == item.Idsub);
                var elemento = _v2context.Set<CosElementogasto>().SingleOrDefault(s => s.Idelementogasto == sub.Idelementogasto);
                var moneda = sub.Monnac == true ? "CUP": "CUC";
                //var moneda = _vcontext.Set<ConCuentamoneda>().SingleOrDefault(s => s.Idcuenta == item.Idcuenta);
                if (moneda != null)
                {
                    var tipoM = _v2context.Set<GenMoneda>().SingleOrDefault(s => s.Sigla == moneda);

                    var TipoMonedaOK = tipoM.Sigla.Equals("CUP") ? "100" : "101";


                    if (!submayor.Any( s => 
                    s.Ano == ano && 
                    s.Cta == Convert.ToString(item.Clavecuenta) &&
                    s.Debe == item.Debito &&
                    s.Haber == item.Credito &&
                    s.Epigrafe == item.Clavecentro &&
                    s.Analisis == elemento.Codigo &&
                    s.SubAnalisis == sub.Codigo &&
                    s.Mes == mesx &&
                    s.Fecha == periodo.Inicio && 
                    s.SubCta == TipoMonedaOK 
                   ))
                    {
                        var subNew = new SubMayor
                        {
                            Ano = ano,
                            Cta = Convert.ToString(item.Clavecuenta),
                            Debe = item.Debito,
                            Haber = item.Credito,
                            Epigrafe = item.Clavecentro,
                            Analisis = elemento.Codigo,
                            SubAnalisis = sub.Codigo,
                            Mes = mesx,
                            Fecha = periodo.Inicio,
                            SubCta = TipoMonedaOK,
                        };
                        _context.Set<SubMayor>().Add(subNew);
                        _context.SaveChanges();
                    }
                }
                
            }
                                                     
            return submayor;
        }

       
    }
}
