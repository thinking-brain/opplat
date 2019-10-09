using System;
using System.Collections.Generic;
using System.Linq;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Dtos;
using ContabilidadWebApi.Helpers;
using ContabilidadWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContabilidadWebApi.Services
{
    public class CuentasServices
    {
        private ContabilidadDbContext _db;
        private CuentasHelper _cuentaHelper;
        private ILogger _logger;

        public CuentasServices(ContabilidadDbContext context, ILogger<CuentasServices> logger)
        {
            _db = context;
            _logger = logger;
            _cuentaHelper = new CuentasHelper(context);
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
            try
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

                disponibilidad.Saldo += _cuentaHelper.ImporteMovimiento(cuenta.Naturaleza, tipoDeOperacion, importe);
                disponibilidad.Fecha = fecha;
                _db.SaveChanges();
                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Devuelve los movimientos de una cuenta en un rango de fechas
        /// </summary>
        /// <param name="cuentaId"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        public List<Movimiento> MovimientosDeCuenta(int cuentaId, DateTime fechaInicio, DateTime fechaFin)
        {
            var movimientos = new List<Movimiento>();
            var movs = _db.Set<Movimiento>()
                    .Include(c => c.Asiento.DiaContable)
                    .Where(c => c.CuentaId == cuentaId && c.Asiento.DiaContable.Fecha >= fechaInicio.Date && c.Asiento.DiaContable.Fecha.Date <= fechaFin.Date)
                    .ToList();
            return movimientos;
        }

        /// <summary>
        /// Devuelve los movimientos de una cuenta y sus cuentas descendientes en un rango de fechas
        /// </summary>
        /// <param name="cuentaId"></param>
        /// <param name="fechaInicio"></param>
        /// <param name="fechaFin"></param>
        /// <returns></returns>
        public List<Movimiento> MovimientosDeCuentaYDescendientes(int cuentaId, DateTime fechaInicio, DateTime fechaFin)
        {
            var movimientos = new List<Movimiento>();
            var ctas = _cuentaHelper.CuentasHijas(cuentaId);
            foreach (var cta in ctas)
            {
                var movs = MovimientosDeCuenta(cta.Id, fechaInicio, fechaFin);
                if (movs.Any())
                {
                    movimientos.AddRange(movs);
                }
            }
            return movimientos;
        }


        /// <summary>
        /// Devuelve los movimientos de una cuenta en un periodo contable
        /// </summary>
        /// <param name="cuentaId"></param>
        /// <param name="periodoId"></param>
        /// <returns></returns>
        public List<Movimiento> MovimientosDeCuenta(int cuentaId, int periodoId)
        {
            var periodo = _db.Set<PeriodoContable>().Find(periodoId);
            return MovimientosDeCuenta(cuentaId, periodo.FechaInicio, periodo.FechaFin);
        }

        /// <summary>
        /// Devuelve los movimientos de una cuenta y sus descendientes en un periodo contable
        /// </summary>
        /// <param name="cuentaId"></param>
        /// <param name="periodoId"></param>
        /// <returns></returns>
        public List<Movimiento> MovimientosDeCuentaYDescendientes(int cuentaId, int periodoId)
        {
            var periodo = _db.Set<PeriodoContable>().Find(periodoId);
            return MovimientosDeCuentaYDescendientes(cuentaId, periodo.FechaInicio, periodo.FechaFin);
        }
    }
}