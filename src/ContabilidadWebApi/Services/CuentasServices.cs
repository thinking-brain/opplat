using System;
using System.Collections.Generic;
using System.Linq;
using ContabilidadWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadWebApi.Services
{
    public class CuentasServices
    {
        public DbContext _db;
        private PeriodoContableService _periodoContableService;

        public CuentasServices(DbContext context)
        {
            _db = context;
            _periodoContableService = new PeriodoContableService(context);
        }

        /// <summary>
        /// Busca cuenta por el nombre de la misma
        /// </summary>
        /// <param name="nombreDeCuenta">Nombre de la cuenta que se desea buscar</param>
        /// <returns>Cuenta que tenga el nombre por el que se busco o null si no existe</returns>
        public Cuenta FindCuentaByNombre(string nombreDeCuenta)
        {
            return _db.Set<Cuenta>().Include(c => c.Disponibilidad).SingleOrDefault(c => c.Nivel.Nombre == nombreDeCuenta);
        }

        /// <summary>
        /// Modifica la disponibilidad de una cuenta
        /// </summary>
        /// <param name="cuentaId">El id de la cuenta a cambiar disponibilidad</param>
        /// <param name="importe">Importe por el que se va a modificar la cuenta</param>
        /// <param name="tipoDeOperacion">El tipo de operacion a realizar (debito o credito)</param>
        /// <param name="fecha">Fecha de la operacion</param>
        /// <returns>True si se puede efectuar la operacion o False de lo contrario</returns>
        public bool ModificarDisponibilidad(int cuentaId, decimal importe, TipoDeOperacion tipoDeOperacion, DateTime fecha)
        {
            var cuenta = _db.Set<Cuenta>().Find(cuentaId);
            if (cuenta == null)
            {
                return false;
            }
            var disponibilidad = _db.Set<Disponibilidad>().SingleOrDefault(d => d.CuentaId == cuentaId);
            if (disponibilidad == null)
            {
                disponibilidad = new Disponibilidad() { CuentaId = cuentaId, Fecha = DateTime.Now, Saldo = 0 };
                _db.Set<Disponibilidad>().Add(disponibilidad);
            }

            if (cuenta.Naturaleza == Naturaleza.Acreedora && tipoDeOperacion == TipoDeOperacion.Credito)
            {
                disponibilidad.Saldo += importe;
            }
            if (cuenta.Naturaleza == Naturaleza.Acreedora && tipoDeOperacion == TipoDeOperacion.Debito)
            {
                disponibilidad.Saldo -= importe;
            }
            if (cuenta.Naturaleza == Naturaleza.Deudora && tipoDeOperacion == TipoDeOperacion.Credito)
            {
                disponibilidad.Saldo -= importe;
            }
            if (cuenta.Naturaleza == Naturaleza.Deudora && tipoDeOperacion == TipoDeOperacion.Debito)
            {
                disponibilidad.Saldo += importe;
            }
            disponibilidad.Fecha = fecha;
            return true;
        }

        /// <summary>
        /// Devuelve movimientos de una cuenta (por el nombre)
        /// </summary>
        /// <param name="nombreCuenta">Nombre de la cuenta</param>
        /// <returns>Movimientos de cuenta</returns>
        public ICollection<Movimiento> GetMovimientosDeCuenta(string nombreCuenta)
        {
            var id = FindCuentaByNombre(nombreCuenta).Id;
            return GetMovimientosDeCuenta(id);
        }

        /// <summary>
        /// Devuelve movimientos de una una cuenta (por el id)
        /// </summary>
        /// <param name="cuentaId">El id de la cuenta que se desea los movimientos</param>
        /// <returns>Los Movimientos de la cuenta</returns>
        public ICollection<Movimiento> GetMovimientosDeCuenta(int cuentaId)
        {
            return _db.Set<Movimiento>()
                .Include(m => m.Asiento)
                .Include(m => m.Asiento.DiaContable)
                .Include(m => m.Cuenta.Nivel)
                .Where(m => m.CuentaId == cuentaId).ToList();
        }

        /// <summary>
        /// Crea movimiento de cuenta
        /// </summary>
        /// <param name="cuentaId"></param>
        /// <param name="importe"></param>
        /// <param name="tipoDeOperacion"></param>
        /// <returns></returns>
        private Movimiento CrearMovimiento(int cuentaId, decimal importe, TipoDeOperacion tipoDeOperacion)
        {
            var mov = new Movimiento()
            {
                CuentaId = cuentaId,
                Importe = importe,
                TipoDeOperacion = tipoDeOperacion
            };
            return mov;
        }

        /// <summary>
        /// Crea un asiento contable sin agregarlo al contexto
        /// </summary>
        /// <param name="cuentaCreditoId"></param>
        /// <param name="cuentaDebitoId"></param>
        /// <param name="importe"></param>
        /// <param name="fecha"></param>
        /// <param name="detalle"></param>
        /// <param name="usuarioId"></param>
        /// <returns></returns>
        public Asiento CrearAsientoContable(int cuentaCreditoId, int cuentaDebitoId, decimal importe, DateTime fecha, string detalle, string usuarioId)
        {
            var diaContable = _periodoContableService.GetDiaContableActual();
            if (diaContable == null)
            {
                return null;
            }
            var asiento = new Asiento()
            {
                DiaContableId = diaContable.Id,
                Fecha = fecha,
                Detalle = detalle,
                UsuarioId = usuarioId
            };
            var mov1 = CrearMovimiento(cuentaCreditoId, importe, TipoDeOperacion.Credito);
            var mov2 = CrearMovimiento(cuentaDebitoId, importe, TipoDeOperacion.Debito);
            asiento.Movimientos.Add(mov1);
            asiento.Movimientos.Add(mov2);
            ModificarDisponibilidad(cuentaCreditoId, importe, TipoDeOperacion.Credito, fecha);
            ModificarDisponibilidad(cuentaDebitoId, importe, TipoDeOperacion.Debito, fecha);
            return asiento;
        }

        /// <summary>
        /// Crea un asiento contable y lo agrega al contexto
        /// </summary>
        /// <param name="cuentaCreditoId"></param>
        /// <param name="cuentaDebitoId"></param>
        /// <param name="importe"></param>
        /// <param name="fecha"></param>
        /// <param name="detalle"></param>
        /// <param name="usuarioId"></param>
        public void AgregarOperacion(int cuentaCreditoId, int cuentaDebitoId, decimal importe, DateTime fecha, string detalle, string usuarioId)
        {
            _db.Set<Asiento>().Add(CrearAsientoContable(cuentaCreditoId, cuentaDebitoId, importe, fecha, detalle, usuarioId));
        }
    }
}