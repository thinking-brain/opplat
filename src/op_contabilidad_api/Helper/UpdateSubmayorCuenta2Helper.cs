using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using op_contabilidad_api.ViewModels;
using op_contabilidad_api.Models;
using op_contabilidad_api.Data;
using Microsoft.EntityFrameworkCore;
//using op_contabilidad_api.VersatModels;
using op_contabilidad_api.VersatModels2;

namespace op_contabilidad_api.Helper
{
    public class UpdateSubmayorCuenta2Helper
    {

        private readonly DbContext _context;
        private readonly VersatDbContext2 _v2context;

        public UpdateSubmayorCuenta2Helper(VersatDbContext2 _v2context, ApiDbContext _context)
        {
            this._context = _context;
            this._v2context = _v2context;
        }
        public UpdateSubmayorCuenta2Helper(VersatDbContext2 _v2context, DbContext _context)
        {
            this._context = _context;
            this._v2context = _v2context;
        }
        public void LlenarSubMayor2()
        {
            var submayorCuenta = _context.Set<SubMayorCuenta>().ToList();

            //Tabla de Movimientos en el Versat (SUBMAYOR)
            var data = _v2context.Set<OptCuentaPeriodo>().Include(s=>s.IdperiodoNavigation).Where(s=>s.Idperiodo<43).ToList();

            foreach (var item in data.OrderBy(s => s.Idperiodo))
            {

                //Seleccionando solo los movimientos dentro del periodo del mes consultado
                var dataInMes = data.Where(s => s.Idperiodo == item.Idperiodo).ToList();

                if (item.Clave.StartsWith("912"))
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
                if (item.Clave.StartsWith("913"))
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

                if (item.Clave.StartsWith("920") || item.Clave.StartsWith("921"))
                {
                    var mov = new SubMayorCuenta
                    {
                        Año = item.IdperiodoNavigation.Inicio.Year,
                        Mes = item.IdperiodoNavigation.Inicio.Month,
                        IdCuenta = item.Idcuenta,
                        ClaveCuenta = item.Clave,
                        Importe = item.Credito,
                    };
                     _context.Set<SubMayorCuenta>().Add(mov);
                    _context.SaveChanges();
                }
                if (item.Clave.Equals("805"))
                {
                    var mov = new SubMayorCuenta
                    {
                        Año = item.IdperiodoNavigation.Inicio.Year,
                        Mes = item.IdperiodoNavigation.Inicio.Month,
                        IdCuenta = item.Idcuenta,
                        ClaveCuenta = item.Clave,
                        Importe = item.Debito,
                    };
                     _context.Set<SubMayorCuenta>().Add(mov);
                    _context.SaveChanges();
                }
                if (item.Clave.StartsWith("810"))
                {
                    var mov = new SubMayorCuenta
                    {
                        Año = item.IdperiodoNavigation.Inicio.Year,
                        Mes = item.IdperiodoNavigation.Inicio.Month,
                        IdCuenta = item.Idcuenta,
                        ClaveCuenta = item.Clave,
                        Importe = item.Debito +item.Credito,
                    };
                     _context.Set<SubMayorCuenta>().Add(mov);
                    _context.SaveChanges();
                }
                if (item.Clave.Equals("822"))
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

                if (item.Clave.StartsWith("835"))
                {
                    var mov = new SubMayorCuenta
                    {
                        Año = item.IdperiodoNavigation.Inicio.Year,
                        Mes = item.IdperiodoNavigation.Inicio.Month,
                        IdCuenta = item.Idcuenta,
                        ClaveCuenta = item.Clave,
                        Importe = item.Debito,
                    };
                     _context.Set<SubMayorCuenta>().Add(mov);
                    _context.SaveChanges();
                }
                if (item.Clave.StartsWith("836"))
                {
                    var mov = new SubMayorCuenta
                    {
                        Año = item.IdperiodoNavigation.Inicio.Year,
                        Mes = item.IdperiodoNavigation.Inicio.Month,
                        IdCuenta = item.Idcuenta,
                        ClaveCuenta = item.Clave,
                        Importe = item.Debito,
                    };
                     _context.Set<SubMayorCuenta>().Add(mov);
                    _context.SaveChanges();
                }
                if (item.Clave.Equals("845"))
                {
                    var mov = new SubMayorCuenta
                    {
                        Año = item.IdperiodoNavigation.Inicio.Year,
                        Mes = item.IdperiodoNavigation.Inicio.Month,
                        IdCuenta = item.Idcuenta,
                        ClaveCuenta = item.Clave,
                        Importe = item.Debito,
                    };
                     _context.Set<SubMayorCuenta>().Add(mov);
                    _context.SaveChanges();
                }
                if (item.Clave.StartsWith("856"))
                {
                    var mov = new SubMayorCuenta
                    {
                        Año = item.IdperiodoNavigation.Inicio.Year,
                        Mes = item.IdperiodoNavigation.Inicio.Month,
                        IdCuenta = item.Idcuenta,
                        ClaveCuenta = item.Clave,
                        Importe = item.Debito,
                    };
                     _context.Set<SubMayorCuenta>().Add(mov);
                    _context.SaveChanges();
                }
            }
        }
    }
}
