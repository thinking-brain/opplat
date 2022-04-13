using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Opplat.Contabilidad.Domain.Entities;
    public class Asiento
    {
        public int Id { get; set; }

        public int DiaContableId { get; set; }

        public virtual DiaContable DiaContable { get; set; }

        public DateTime Fecha { get; set; }

        public virtual ICollection<Movimiento> Movimientos { get; set; }

        public string Usuario { get; set; }

        public string Detalle { get; set; }

        public Asiento()
        {
            Movimientos = new HashSet<Movimiento>();
        }

        [NotMapped]
        public bool EsValido
        {
            get
            {
                return Movimientos.Where(m => m.TipoDeOperacion == TipoDeOperacion.Credito).Sum(c => c.Importe) == Movimientos.Where(m => m.TipoDeOperacion == TipoDeOperacion.Debito).Sum(d => d.Importe);
            }
        }
    }
