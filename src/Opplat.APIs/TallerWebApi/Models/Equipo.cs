using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using TallerWebApi.Data;
using TallerWebApi.Models;

namespace TallerWebApi.Models {
    public class Equipo {
        public int Id { get; set; }
        public int NumeroSerie { get; set; }
        public DateTime FechaFabricacion { get; set; }
        public int TipoEquipoId { get; set; }
        public TipoEquipo TipoEquipo { get; set; }

        public int MarcaId { get; set; }
        public Marca Marca { get; set; }

        public int ModeloId { get; set; }
        public Modelo Modelo { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public IEnumerable<HistoricoEquipo> HistoricoEquipo { get; set; }
        public string Observaciones { get; set; }
        public IEnumerable<DocumentoEquipo> Documentos { get; set; }
        public EstadoEquipo EstadoEquipo { get; set; }
        public bool Activo { get; set; }
    }
}