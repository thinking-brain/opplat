using System;

namespace TallerWebApi.Models {
    public class OrdenReparacionDto {
        public int Id { get; set; }
        public int Cliente { get; set; }
        public int TecnicoRxEquipo { get; set; }
        public int Tecnico { get; set; }
        public int Equipo { get; set; }
        public string Defecto { get; set; }
        public string Causa { get; set; }
        public string Accion { get; set; }
        public string DefectoCliente { get; set; }
        public double TiempoEmpleado { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime FechaFinalizado { get; set; }
        public DateTime FechaCerrado { get; set; }
        public DateTime FechaPrometido { get; set; }
        public DateTime Garantía { get; set; }
        public EstadoOrdenReparacion EstadoOrden { get; set; }
        public int Taller { get; set; }
        public int? Presupuesto { get; set; }
        public string InformeTecnico { get; set; }
        public LugarReparacion LugarReparacion { get; set; }
        public bool Activo { get; set; }

    }
}