using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using RhWebApi.Data;
using RhWebApi.Dtos;
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
    public byte[] Foto { get; set; }
    public string Direccion { get; set; }
    public string Correo { get; set; }
    public ColorDeOjos ColorDeOjos { get; set; }
    public ColorDePiel ColorDePiel { get; set; }
    public TallaDeCamisa TallaDeCamisa { get; set; }
    public string TallaPantalon { get; set; }
    public double TallaCalzado { get; set; }
    public string OtrasCaracteristicas { get; set; }
    public int? MunicipioId { get; set; }
    public virtual NivelDeEscolaridad NivelDeEscolaridad { get; set; }
    public string EstadoTrabajador { get; set; }
    public DateTime Fecha { get; set; }
    public string Perfil_Ocupacional { get; set; }
    public string Nombre_Referencia { get; set; }

    [NotMapped]
    private string _nombreCompleto;

    [NotMapped]
    public string NombreCompleto {
      get { return Nombre + " " + Apellidos; }
      set { _nombreCompleto = value; }
    }
  }
}