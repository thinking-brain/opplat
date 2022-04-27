using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opplat.Contabilidad.Domain.Entities;
    public class DepreciacionAft:Asiento
    {
        public int AftId { get; set; }

        public virtual Aft Aft { get; set; }
    }

