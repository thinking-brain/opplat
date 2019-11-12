using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RhWebApi.Models
{
    public enum CausaDeBaja
    {
        [Display(Name = "Sin Definir")]
        SinDefinir = 0,

        Salarial = 1,

        Jubilación = 2,

        Fallecimiento = 3,

        [Display(Name = "Invalidez Total")]
        InvalidezTotal = 4,

        [Display(Name = "Invalidez Parcial")]
        InvalidezParcial = 5,

        [Display(Name = "Privación de libertad")]
        PrivacionDeLibertad = 6,

        [Display(Name = "Paso a formas no estatales")]
        PasoAFormasNoEstatales = 7,

        [Display(Name = "Proceso de disponibilidad")]
        ProcesoDeDisponibilidad = 8,

        [Display(Name = "Sanción administrativa")]
        SancionAdministrativa = 9,

        [Display(Name = "Salida del país")]
        SalidaDelPais = 10,

        [Display(Name = "Otras causas")]
        OtrasCausas = 11,
    }
}