using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using RhWebApi.Data;
using RhWebApi.Dtos;
using RhWebApi.Models;

namespace RhWebApi.Dtos {
  public class UserTrabajador {
    public string Username { get; set; }
    public int TrabajadorId { get; set; }
    public string NombreCompleto { get; set; }
  }
}