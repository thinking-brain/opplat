using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("Area")]
    public class Area
    {
       public int Id { get; set; }
       public string Nombre { get; set; }

       public int DepartamentoId { get; set; }
       public virtual Departamento Departamento { get; set; }
    }
}