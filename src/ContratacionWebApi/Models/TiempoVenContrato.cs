using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ContratacionWebApi.Models;

public class TiempoVenContrato {
    public int Id { get; set; }
    public int ContratoTiempo { get; set; }
    public int ContratosProxVencerDesde { get; set; }
    public int ContratosProxVencerHasta { get; set; }
    public int ContratosCasiVencDesde { get; set; }
    public int ContratosCasiVencHasta { get; set; }
    public int ContratosVencidos { get; set; }
}