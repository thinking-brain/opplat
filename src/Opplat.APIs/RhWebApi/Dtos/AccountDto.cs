using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using RhWebApi.Data;
using RhWebApi.Dtos;
using RhWebApi.Models;

namespace RhWebApi.Dtos {
  public class AccountDto {
    public string UserId { get; set; }
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
    public string Username { get; set; }
  }
}