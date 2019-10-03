using ContabilidadWebApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ContabilidadWebApi.Services
{
    public class CobroService
    {
        DbContext _db;
        PeriodoContableService periodoContableService;
        CuentasServices _cuentaService;

        public CobroService(DbContext context)
        {
            _db = context;
            periodoContableService = new PeriodoContableService(context);
            _cuentaService = new CuentasServices(context);
        }

        public Cobro CobrarEnEfectivo(decimal importe, string detalle, string usuarioId)
        {

            var cuentaCaja = _cuentaService.FindCuentaByNombre("Caja");
            var cuentaGasto = _cuentaService.FindCuentaByNombre("Gastos");

            var asiento = _cuentaService.CrearAsientoContable(cuentaGasto.Id, cuentaCaja.Id, importe, DateTime.Now, detalle, usuarioId);

            var cobro = new Cobro()
            {
                Importe = importe,
                UsuarioId = usuarioId,
                DiaContableId = periodoContableService.GetDiaContableActual().Id,
                Fecha = DateTime.Now,
                Asiento = asiento,
            };
            return cobro;
        }
    }
}
