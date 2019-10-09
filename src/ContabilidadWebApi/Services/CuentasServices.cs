using System;
using System.Collections.Generic;
using System.Linq;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Dtos;
using ContabilidadWebApi.Helpers;
using ContabilidadWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadWebApi.Services
{
    public class CuentasServices
    {
        public ContabilidadDbContext _db;
        private PeriodoContableService _periodoContableService;

        public CuentasServices(ContabilidadDbContext context)
        {
            _db = context;
            _periodoContableService = new PeriodoContableService(context);
        }

        /// <summary>
        /// Crea una cuenta nueva y devuelve el DTO con los datos de la misma
        /// </summary>
        /// <param name="nuevaCuenta"></param>
        /// <returns></returns>
        public CuentaDto CrearCuenta(string numero, string nombre, Naturaleza naturaleza)
        {
            // using (var transaction = _db.Database.BeginTransaction())
            // {
            //     transaction.Commit();
            // }
            var cuenta = FindCuentaByNumero(numero);
            if (cuenta == null)
            {
                var numeros = numero.Split('-').ToList();
                var numeroParcial = numeros.Last();
                Cuenta cuentaSuperior = null;
                if (numeros.Count > 1)
                {
                    numeros.Remove(numeroParcial);
                    var numeroSuperior = String.Join("-", numeros);
                    cuentaSuperior = FindCuentaByNumero(numeroSuperior);
                    if (cuentaSuperior == null)
                    {
                        throw new Exception("No existe la cuenta a la que pertenece la cuenta que esta creando, cree la cuenta superior primero.");
                    }
                }
                cuenta = new Cuenta { NumeroParcial = numeroParcial, Nombre = nombre, Naturaleza = naturaleza };
                if (cuentaSuperior != null)
                {
                    cuenta.CuentaSuperiorId = cuentaSuperior.Id;
                }
                _db.Add(cuenta);
                _db.SaveChanges();
            }
            var ctaHelper = new CuentasHelper(_db);
            return ctaHelper.GetCuenta(cuenta.Id);
        }

        /// <summary>
        /// Crea una cuenta nueva y devuelve el DTO con los datos de la misma
        /// </summary>
        /// <param name="nuevaCuenta"></param>
        /// <returns></returns>
        public CuentaDto EditarCuenta(int id, string numero, string nombre, Naturaleza naturaleza)
        {
            var cuenta = _db.Set<Cuenta>().Find(id);
            if (cuenta == null)
            {
                throw new Exception($"No existe ninguna cuenta con el id {id}.");
            }
            var numeros = numero.Split('-').ToList();
            var numeroParcial = numeros.Last();
            var cuentaSuperior = new Cuenta();
            if (numeros.Count > 1)
            {
                numeros.Remove(numeroParcial);
                var numeroSuperior = String.Join("-", numeros);
                cuentaSuperior = FindCuentaByNumero(numeroSuperior);
                if (cuentaSuperior == null)
                {
                    throw new Exception("No existe la cuenta a la que pertenece la cuenta que esta modificando, cree la cuenta superior primero.");
                }
            }
            cuenta.NumeroParcial = numeroParcial;
            cuenta.Nombre = nombre;
            cuenta.Naturaleza = naturaleza;
            if (cuentaSuperior != null)
            {
                cuenta.CuentaSuperiorId = cuentaSuperior.Id;
            }
            else
            {
                cuenta.CuentaSuperiorId = null;
            }
            _db.Update(cuenta);
            _db.SaveChanges();

            var ctaHelper = new CuentasHelper(_db);
            return ctaHelper.GetCuenta(cuenta.Id);
        }

        /// <summary>
        /// Busca cuenta por el numero de la misma
        /// </summary>
        /// <param name="numero">Numero de la cuenta que se desea buscar</param>
        /// <returns>Cuenta que tenga el nombre por el que se busco o null si no existe</returns>
        public Cuenta FindCuentaByNumero(string numero)
        {
            var cuentas = _db.Set<Cuenta>()
                .Include(c => c.CuentaSuperior.CuentaSuperior.CuentaSuperior.CuentaSuperior)
                .Include(c => c.Subcuentas);
            var cuenta = cuentas.SingleOrDefault(c => c.Numero == numero);
            return cuenta;
        }

        /// <summary>
        /// Busca cuenta por el nombre de la misma
        /// </summary>
        /// <param name="nombreDeCuenta">Nombre de la cuenta que se desea buscar</param>
        /// <returns>Cuenta que tenga el nombre por el que se busco o null si no existe</returns>
        public Cuenta FindCuentaByNombre(string nombreDeCuenta)
        {
            return _db.Set<Cuenta>().SingleOrDefault(c => c.Nombre == nombreDeCuenta);
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