using System;
using System.Collections.Generic;
using System.Linq;
using ContabilidadWebApi.Data;
using ContabilidadWebApi.Dtos;
using ContabilidadWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadWebApi.Helpers
{
    /// <summary>
    /// Clase con metodos para ayudar a navegar por el arbol de cuentas y otras funcionalidades asociadas a las cuentas contables
    /// </summary>
    public class CuentasHelper
    {
        private ContabilidadDbContext _db;

        public CuentasHelper(ContabilidadDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Calcula si el importe es negativo o positivo 
        /// </summary>
        /// <param name="naturaleza">Si la cuenta es Acreedora o Deudora</param>
        /// <param name="tipoDeOperacion">si la operacion es de credito o debito</param>
        /// <param name="importe">el importe del que se quiere saber si es positivo o negativo</param>
        /// <returns>el importe calculado</returns>
        public decimal ImporteMovimiento(Naturaleza naturaleza, TipoDeOperacion tipoDeOperacion, decimal importe)
        {
            if (naturaleza == Naturaleza.Acreedora && tipoDeOperacion == TipoDeOperacion.Credito)
            {
                return importe;
            }
            if (naturaleza == Naturaleza.Acreedora && tipoDeOperacion == TipoDeOperacion.Debito)
            {
                return -importe;
            }
            if (naturaleza == Naturaleza.Deudora && tipoDeOperacion == TipoDeOperacion.Credito)
            {
                return -importe;
            }
            if (naturaleza == Naturaleza.Deudora && tipoDeOperacion == TipoDeOperacion.Debito)
            {
                return importe;
            }
            return importe;
        }

        /// <summary>
        /// Listado de cuentas en estructura de arbol
        /// </summary>
        /// <returns>Arbol de cuentas</returns>
        public List<CuentaDto> Cuentas()
        {
            //todo: devuelve la lista completa de cuentas y la estructura en arbol, esto de debe separar
            var cuentas = _db.Set<Cuenta>()
                .Include(c => c.CuentaSuperior)
                .Include(c => c.Subcuentas)
                .OrderByDescending(c => c.CuentaSuperiorId)
                .ToList();

            var ctas = new List<CuentaDto>();
            foreach (var cta in cuentas)
            {
                if (!ctas.Any(c => c.Id == cta.Id))
                {
                    ctas.Add(new CuentaDto() { Id = cta.Id, Numero = cta.Numero, Descripcion = cta.Nombre });
                }
            }
            return ctas;
        }

        /// <summary>
        /// Busca una cuenta por su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>DTO de la cuenta</returns>
        public CuentaDto GetCuenta(int id)
        {
            var cuenta = _db.Set<Cuenta>()
                .Include(c => c.CuentaSuperior.CuentaSuperior.CuentaSuperior.CuentaSuperior)
                .Include(c => c.Subcuentas)
                .SingleOrDefault(c => c.Id == id);
            var cta = new CuentaDto { Id = cuenta.Id, Numero = cuenta.Numero, Naturaleza = cuenta.Naturaleza, Descripcion = cuenta.Nombre };
            return cta;
        }

        /// <summary>
        /// Busca las cuentas descendientes de una cuenta
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Lista de cuentas descendientes incluyendo la cuenta que se solicita</returns>
        public List<CuentaDto> CuentasHijas(int id)
        {
            var ctas = new List<CuentaDto>();
            var cola = new Queue<int>();
            cola.Enqueue(id);
            while (cola.Any())
            {
                var cuentaId = cola.Dequeue();
                var cta = _db.Set<Cuenta>()
                    .Include(c => c.CuentaSuperior)
                    .Include(c => c.Subcuentas)
                    .SingleOrDefault(c => c.Id == cuentaId);

                ctas.Add(new CuentaDto { Id = cta.Id, Numero = cta.Numero, Descripcion = cta.Nombre, Naturaleza = cta.Naturaleza });
                foreach (var item in cta.Subcuentas)
                {
                    cola.Enqueue(item.Id);
                }
            }
            return ctas;
        }

    }
}