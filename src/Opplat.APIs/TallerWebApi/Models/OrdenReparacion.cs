using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TallerWebApi.Models {
    public class OrdenReparacion {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public int TecnicoRxEquipoId { get; set; }
        public Tecnico TecnicoRxEquipo { get; set; }
        public int TecnicoId { get; set; }
        public Tecnico Tecnico { get; set; }
        public int EquipoId { get; set; }
        public Equipo Equipo { get; set; }
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
        public int TallerId { get; set; }
        public Taller Taller { get; set; }
        public int? PresupuestoId { get; set; }
        public Presupuesto Presupuesto { get; set; }
        public string InformeTecnico { get; set; }
        public LugarReparacion LugarReparacion { get; set; }
        public bool Activo { get; set; }

    }
}