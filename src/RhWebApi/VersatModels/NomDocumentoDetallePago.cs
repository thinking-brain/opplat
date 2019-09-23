using System;
using System.Collections.Generic;

namespace RhWebApi.VersatModels
{
    public partial class NomDocumentoDetallePago
    {
      

        public int Idlinea { get; set; }
        public int Iddocumento { get; set; }
        public int Idpuestotrabajo { get; set; }
        public int Idconcepto { get; set; }
        public int Idtrabajador { get; set; }
        public int IdbasetiempoFondotiempo { get; set; }
        public decimal? NBasico { get; set; }
        public decimal? NFondodetiempo { get; set; }
        public decimal? NJornadalaboral { get; set; }
        public decimal? NTiempoentrado { get; set; }
        public decimal? NTarifasalarial { get; set; }
        public decimal? NDias { get; set; }
        public decimal? NCobrar { get; set; }
        public decimal? NTotalBonificaciones { get; set; }
        public decimal? NTotalCa { get; set; }
        public decimal? NTotalPenalizaciones { get; set; }
        public decimal? NTotalDescuentos { get; set; }
        public decimal? NTotalImpSal { get; set; }
        public decimal? NTotalRetenciones { get; set; }
        public decimal? NLiquidado { get; set; }
        public decimal? NTiempoacumulado { get; set; }
        public decimal? NImporteAcumulado { get; set; }
        public int? Idlineaajustada { get; set; }
        public int? Idcuenta { get; set; }
        public int? Idcentrocosto { get; set; }
        public int? Idarea { get; set; }
        public int? Idcriterio { get; set; }
        public decimal NDiasnoabsent { get; set; }
        public DateTime? Fechaalta { get; set; }
        public int? Idcentrotrab { get; set; }
        public bool? Cancelado { get; set; }

        public virtual ConCuenta IdcuentaNavigation { get; set; }
        public virtual NomDocumento IddocumentoNavigation { get; set; }
    }
}
