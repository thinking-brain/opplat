using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using op_costos_api.ViewModels;
using op_costos_api.Models;
using op_costos_api.Data;
using Microsoft.EntityFrameworkCore;
//using op_costos_api.VersatModels;
using op_costos_api.VersatModels;

namespace op_costos_api.Helper
{
    public class UpdateSubmayorCuentaHelper
    {

        private readonly DbContext _context;
        private readonly VersatDbContext _vcontext;

        public UpdateSubmayorCuentaHelper(VersatDbContext _vcontext, ApiDbContext _context)
        {
            this._context = _context;
            this._vcontext = _vcontext;
        }
        public UpdateSubmayorCuentaHelper(VersatDbContext _vcontext, DbContext _context)
        {
            this._context = _context;
            this._vcontext = _vcontext;
        }
        public void LlenarSubMayor2()
        {
            var submayorCuenta = _context.Set<SubMayorCuenta>().ToList();
            var UltimoMes = _context.Set<SubMayorCuenta>().ToList().Last();
            var periodoUltimo = _vcontext.Set<GenPeriodo>().SingleOrDefault(s => s.Inicio.Month == UltimoMes.Mes && s.Inicio.Year == UltimoMes.Año);

            //Tabla de Movimientos en el Versat (SUBMAYOR)
            var data = submayorCuenta.Any(s=>s.Mes >= 4) ? _vcontext.Set<OptCuentaPeriodoOK>().Include(s => s.IdperiodoNavigation).Where(s => s.Idperiodo >= periodoUltimo.Idperiodo).ToList() : _vcontext.Set<OptCuentaPeriodoOK>().Include(s => s.IdperiodoNavigation).Where(s => s.Idperiodo >= 43).ToList();
            //Tabla de Movimientos para la cuenta 810
            var data2 = _vcontext.Set<OptCuentaPeriodo>().Include(s => s.IdperiodoNavigation).Where(s => s.Idperiodo >= 43).ToList();


            foreach (var item in data2)
            {
                if (!_context.Set<SubMayorCuenta>().Any(s =>
                   s.Año == item.IdperiodoNavigation.Inicio.Year &&
                   s.ClaveCuenta == item.Clave &&
                   s.IdCuenta == item.Idcuenta &&
                   s.Importe == item.Debito + item.Credito &&
                   s.Mes == item.IdperiodoNavigation.Inicio.Month
                   ))
                {
                    if (item.Clave.StartsWith("810"))
                    {
                        var mov = new SubMayorCuenta
                        {
                            Año = item.IdperiodoNavigation.Inicio.Year,
                            Mes = item.IdperiodoNavigation.Inicio.Month,
                            IdCuenta = item.Idcuenta,
                            ClaveCuenta = item.Clave,
                            Importe = item.Debito + item.Credito,
                        };
                        _context.Set<SubMayorCuenta>().Add(mov);
                        _context.SaveChanges();
                    }
                }

            }

            foreach (var item in data.OrderBy(s => s.Idperiodo))
            {

                if (!_context.Set<SubMayorCuenta>().Any(s => 
                s.Año == item.IdperiodoNavigation.Inicio.Year &&
                s.ClaveCuenta == item.Clavecuenta &&
                s.IdCuenta == item.Idcuenta &&
                s.Importe == item.Importe &&
                s.Mes == item.IdperiodoNavigation.Inicio.Month
                ))
                {
                    //Seleccionando solo los movimientos dentro del periodo del mes consultado
                    var dataInMes = data.Where(s => s.Idperiodo == item.Idperiodo).ToList();
                    //Seleccionando solo los movimientos dentro del periodo del mes consultado para cuenta 810
                    var data2InMes = data2.Where(s => s.Idperiodo == item.Idperiodo).ToList();

                    if (item.Clavecuenta.StartsWith("912"))
                    {
                        var mov = new SubMayorCuenta
                        {
                            Año = item.IdperiodoNavigation.Inicio.Year,
                            Mes = item.IdperiodoNavigation.Inicio.Month,
                            IdCuenta = item.Idcuenta,
                            ClaveCuenta = item.Clavecuenta,
                            Importe = item.Importe,
                        };

                        _context.Set<SubMayorCuenta>().Add(mov);
                        _context.SaveChanges();

                    }
                    if (item.Clavecuenta.StartsWith("913"))
                    {
                        var mov = new SubMayorCuenta
                        {
                            Año = item.IdperiodoNavigation.Inicio.Year,
                            Mes = item.IdperiodoNavigation.Inicio.Month,
                            IdCuenta = item.Idcuenta,
                            ClaveCuenta = item.Clavecuenta,
                            Importe = item.Importe,
                        };
                        _context.Set<SubMayorCuenta>().Add(mov);
                        _context.SaveChanges();
                    }

                    if (item.Clavecuenta.StartsWith("920") || item.Clavecuenta.StartsWith("921"))
                    {
                        var mov = new SubMayorCuenta
                        {
                            Año = item.IdperiodoNavigation.Inicio.Year,
                            Mes = item.IdperiodoNavigation.Inicio.Month,
                            IdCuenta = item.Idcuenta,
                            ClaveCuenta = item.Clavecuenta,
                            Importe = item.Importe,
                        };
                        _context.Set<SubMayorCuenta>().Add(mov);
                        _context.SaveChanges();
                    }

                    if (item.Clavecuenta.Equals("805"))
                    {
                        var mov = new SubMayorCuenta
                        {
                            Año = item.IdperiodoNavigation.Inicio.Year,
                            Mes = item.IdperiodoNavigation.Inicio.Month,
                            IdCuenta = item.Idcuenta,
                            ClaveCuenta = item.Clavecuenta,
                            Importe = item.Importe,
                        };
                        _context.Set<SubMayorCuenta>().Add(mov);
                        _context.SaveChanges();
                    }
                    if (item.Clavecuenta.StartsWith("810"))
                    {
                        var mov = new SubMayorCuenta
                        {
                            Año = item.IdperiodoNavigation.Inicio.Year,
                            Mes = item.IdperiodoNavigation.Inicio.Month,
                            IdCuenta = item.Idcuenta,
                            ClaveCuenta = item.Clavecuenta,
                            Importe = item.Importe,
                        };
                        _context.Set<SubMayorCuenta>().Add(mov);
                        _context.SaveChanges();
                    }

                    if (item.Clavecuenta.Equals("822"))
                    {
                        var mov = new SubMayorCuenta
                        {
                            Año = item.IdperiodoNavigation.Inicio.Year,
                            Mes = item.IdperiodoNavigation.Inicio.Month,
                            IdCuenta = item.Idcuenta,
                            ClaveCuenta = item.Clavecuenta,
                            Importe = item.Importe,
                        };
                        _context.Set<SubMayorCuenta>().Add(mov);
                        _context.SaveChanges();
                    }

                    if (item.Clavecuenta.StartsWith("835"))
                    {
                        var mov = new SubMayorCuenta
                        {
                            Año = item.IdperiodoNavigation.Inicio.Year,
                            Mes = item.IdperiodoNavigation.Inicio.Month,
                            IdCuenta = item.Idcuenta,
                            ClaveCuenta = item.Clavecuenta,
                            Importe = item.Importe,
                        };
                        _context.Set<SubMayorCuenta>().Add(mov);
                        _context.SaveChanges();
                    }
                    if (item.Clavecuenta.StartsWith("836"))
                    {
                        var mov = new SubMayorCuenta
                        {
                            Año = item.IdperiodoNavigation.Inicio.Year,
                            Mes = item.IdperiodoNavigation.Inicio.Month,
                            IdCuenta = item.Idcuenta,
                            ClaveCuenta = item.Clavecuenta,
                            Importe = item.Importe,
                        };
                        _context.Set<SubMayorCuenta>().Add(mov);
                        _context.SaveChanges();
                    }
                    if (item.Clavecuenta.Equals("845"))
                    {
                        var mov = new SubMayorCuenta
                        {
                            Año = item.IdperiodoNavigation.Inicio.Year,
                            Mes = item.IdperiodoNavigation.Inicio.Month,
                            IdCuenta = item.Idcuenta,
                            ClaveCuenta = item.Clavecuenta,
                            Importe = item.Importe,
                        };
                        _context.Set<SubMayorCuenta>().Add(mov);
                        _context.SaveChanges();
                    }
                    if (item.Clavecuenta.Equals("856"))
                    {
                        var mov = new SubMayorCuenta
                        {
                            Año = item.IdperiodoNavigation.Inicio.Year,
                            Mes = item.IdperiodoNavigation.Inicio.Month,
                            IdCuenta = item.Idcuenta,
                            ClaveCuenta = item.Clavecuenta,
                            Importe = item.Importe,
                        };
                        _context.Set<SubMayorCuenta>().Add(mov);
                        _context.SaveChanges();
                    }
                }

                
            }
        }
    }
}
