using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models
{
    public enum NivelDeEscolaridad
    {
        [Display(Name = "Sin Definir")]
        SinDefinir = 0,
        [Display(Name = "Nivel Superior")]
        NivelSuperior = 1,
        [Display(Name = "Técnico Médio")]
        TecnicoMedio = 2,
        [Display(Name = "12 Grado(Medio Sup.)")]
        DoceGrado = 3,
        [Display(Name = "9no Grado(Medio)")]
        NovenoGrado = 4,
        [Display(Name = " 6to Grado(Básico)")]
        SextoGrado = 5,
        [Display(Name = "Menos de 6to Grado")]
        MenosDeSextoGrado = 6

    }
}