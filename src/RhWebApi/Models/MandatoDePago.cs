using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    [Table("mandatos_de_pagos")]
    public class MandatoDePago
    {
        public int Id { get; set; }

        public virtual ICollection<DetallesMandato> Detalles { get; set; }
        public virtual Contrato Contrato { get; set; }
        public int ContratoId { get; set; }

        public DateTime Fecha { get; set; }
        public String Codigo { get; set; }
    }
}