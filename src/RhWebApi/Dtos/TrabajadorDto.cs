using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using RhWebApi.Models;

namespace RhWebApi.Dtos {
  public class TrabajadorDto {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellidos { get; set; }
    public string CI { get; set; }
    public string TelefonoFijo { get; set; }
    public string TelefonoMovil { get; set; }
    public virtual Sexo Sexo { get; set; }
    public string Direccion { get; set; }
    public int? MunicipioId { get; set; }
    public string MunicipioProv { get; set; }    
    public virtual NivelDeEscolaridad NivelDeEscolaridad { get; set; }  
    public string EstadoTrabajador { get; set; }
     [NotMapped]
        private string _nombreCompleto;

        [NotMapped]
        public string NombreCompleto {
            get { return Nombre + " " + Apellidos ; }
            set { _nombreCompleto = value; }
        } 
  }
}