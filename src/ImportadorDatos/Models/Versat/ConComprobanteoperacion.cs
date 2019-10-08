using System;
using System.Collections.Generic;

namespace ImportadorDatos.Models.Versat
{
    public partial class ConComprobanteoperacion
    {
        public ConComprobanteoperacion()
        {

        }

        public int Idcomprobante { get; set; }
        public byte Idsubsistema { get; set; }
        public int Idusuario { get; set; }
        public DateTime Fecha { get; set; }
        public int Numero { get; set; }
        public int Idestado { get; set; }
        public string Comentario { get; set; }
        public int Idperiodo { get; set; }
        public virtual ConComprobante IdcomprobanteNavigation { get; set; }
        public virtual GenPeriodo IdperiodoNavigation { get; set; }
        public virtual GenUsuario IdusuarioNavigation { get; set; }
    }
}
