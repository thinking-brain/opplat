using System.Linq;
using ContabilidadWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ContabilidadWebApi.Services
{
    public class MonedaService
    {
        public DbContext _db;

        public MonedaService(DbContext context)
        {
            _db = context;
        }

        public Moneda MonedaPorDefecto()
        {
            return _db.Set<Moneda>().FirstOrDefault();
        }

        public DbSet<Moneda> ListaDeMonedas()
        {
            return _db.Set<Moneda>();
        }

        public int AgregarMoneda(Moneda moneda)
        {
            _db.Set<Moneda>().Add(moneda);
            return _db.SaveChanges();
        }

        public int EditarMoneda(Moneda moneda)
        {
            _db.Entry(moneda).State = EntityState.Modified;
            return _db.SaveChanges();
        }

        public int EliminarMoneda(Moneda moneda)
        {
            _db.Set<Moneda>().Remove(moneda);
            return _db.SaveChanges();
        }

        public int EliminarMoneda(int monedaId)
        {
            var moneda = _db.Set<Moneda>().Find(monedaId);
            return EliminarMoneda(moneda);
        }
    }
}