using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace RhWebApi.Models
{
    [Table("actividades_laborales")]
    public class ActividadLaboral
    {
        public virtual int Id { get; set; }

        public virtual String Codigo { get; set; }

    }
}