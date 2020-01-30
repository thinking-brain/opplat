using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RhWebApi.Data {
    public enum CausaDeBaja {

        SinDefinir,

        Salarial,

        Solicitud_Propia,

        Jubilación,

        Fallecimiento,

        [Display (Name = "Invalidez Total")]
        InvalidezTotal,

        [Display (Name = "Invalidez Parcial")]
        InvalidezParcial,

        [Display (Name = "Privación de libertad")]
        PrivacionDeLibertad,

        [Display (Name = "Paso a formas no estatales")]
        PasoAFormasNoEstatales,

        [Display (Name = "Proceso de disponibilidad")]
        ProcesoDeDisponibilidad,

        [Display (Name = "Sanción administrativa")]
        SancionAdministrativa,

        [Display (Name = "Salida del país")]
        SalidaDelPais,

        [Display (Name = "Otras causas")]
        OtrasCausas,
    }
}