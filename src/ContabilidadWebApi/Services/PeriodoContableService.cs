using System;
using System.Collections.Generic;
using System.Linq;
using ContabilidadWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadWebApi.Services
{
    public class PeriodoContableService
    {
        public DbContext _db;

        public PeriodoContableService(DbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Buscar un dia contable que tenga una fecha especifica
        /// </summary>
        /// <param name="fecha">Fecha del dia que se desea</param>
        /// <returns>El dia contable que tenga la fecha del parametro o null si no existe alguna</returns>
        public DiaContable BuscarDiaContable(DateTime fecha)
        {
            return _db.Set<DiaContable>().SingleOrDefault(d => d.Fecha.Day == fecha.Day && d.Fecha.Month == fecha.Month && d.Fecha.Year == fecha.Year);
        }

        /// <summary>
        /// Buscar dia por id
        /// </summary>
        /// <param name="id">id del dia contable</param>
        /// <returns>Dia contable con el id solicitado o null si no existe</returns>
        public DiaContable BuscarDiaContable(int id)
        {
            return _db.Set<DiaContable>().Find(id);
        }

        /// <summary>
        /// Devuelve el dia contable activo
        /// </summary>
        /// <returns></returns>
        public DiaContable GetDiaContableActual()
        {
            return _db.Set<DiaContable>().SingleOrDefault(d => d.Abierto);
        }


        /// <summary>
        /// Cierra el dia contable actual (Salva la BD)
        /// </summary>
        public void CerrarDiaContable()
        {
            var diaContb = GetDiaContableActual();
            diaContb.Abierto = false;
            diaContb.HoraEnQueCerro = DateTime.Now;
            _db.Entry(diaContb).State = EntityState.Modified;
            _db.SaveChanges();
        }

        /// <summary>
        /// Empieza un dia contable (Salva la BD), si ya existe un dia activo lo devuelve
        /// </summary>
        /// <param name="fecha">Fecha que va a tener el dia contable</param>
        /// <returns>El nuevo dia contable o el dia contable actual si ya existe uno abierto</returns>
        public DiaContable EmpezarDiaContable(DateTime fecha)
        {
            if (NoHayDiaAbierto())
            {
                var dia = new DiaContable() { Fecha = fecha, Abierto = true };
                _db.Set<DiaContable>().Add(dia);
                _db.SaveChanges();
                return dia;
            }
            return GetDiaContableActual();
        }

        private bool NoHayDiaAbierto()
        {
            return !_db.Set<DiaContable>().Any(d => d.Abierto);
        }
    }
}