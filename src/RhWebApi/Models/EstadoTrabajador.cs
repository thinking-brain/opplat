using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RhWebApi.Models
{
    public enum EstadoTrabajador
    {
        Activo,

        Vacaciones,
        Baja,
        Certificado,
        [Display(Name = "Licencia de maternidad")]
        LicenciaMaternidad,

        [Display(Name = "Licencia sin sueldo")]
        LicenciaSinSueldo,

        Fallecido,
        Jubilado,
    }
}