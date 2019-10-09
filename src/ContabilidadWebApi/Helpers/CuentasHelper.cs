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

        public List<CuentaDto> Cuentas()
        {
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
                    if (cta.CuentaSuperior == null)
                    {
                        ctas.Add(new CuentaDto() { Id = cta.Id, Numero = cta.Numero, Descripcion = cta.Nombre });
                    }
                    else
                    {
                        var ctaVM = ctas.FirstOrDefault(c => c.Id == cta.CuentaSuperiorId);
                        if (ctaVM != null)
                        {
                            var vm = new CuentaDto() { Id = cta.Id, Numero = cta.Numero, Descripcion = cta.Nombre, Padre = ctaVM };
                            ctaVM.Subcuentas.Add(vm);
                            ctas.Add(vm);
                        }
                    }
                }
            }
            return ctas;
        }

        public CuentaDto GetCuenta(int id)
        {
            var cuenta = _db.Set<Cuenta>()
                .Include(c => c.CuentaSuperior.CuentaSuperior.CuentaSuperior.CuentaSuperior)
                .Include(c => c.Subcuentas)
                .SingleOrDefault(c => c.Id == id);
            var cta = new CuentaDto { Id = cuenta.Id, Numero = cuenta.Numero, Naturaleza = cuenta.Naturaleza, Descripcion = cuenta.Nombre };
            return cta;
        }

        public List<CuentaDto> CuentasHijas(int id)
        {
            var ctas = new List<CuentaDto>();
            var cta = Cuentas().SingleOrDefault(c => c.Id == id);
            var cola = new Queue<CuentaDto>();
            cola.Enqueue(cta);
            while (cola.Any())
            {
                var c = cola.Dequeue();
                ctas.Add(c);
                foreach (var item in c.Subcuentas)
                {
                    cola.Enqueue(item);
                }
            }
            return ctas;
        }

    }
}