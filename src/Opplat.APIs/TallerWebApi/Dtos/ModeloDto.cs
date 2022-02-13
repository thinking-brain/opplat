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
    public class ModeloDto {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Marca { get; set; }
        public bool Activo { get; set; }

    }
}