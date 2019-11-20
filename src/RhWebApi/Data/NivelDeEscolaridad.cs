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
        SinDefinir,        
        [Display( Name = "csv", Description ="Nivel Superior" )]
        NivelSuperior,
        [Display(Name = "Técnico Médio")]
        TecnicoMedio,
        [Display(Name = "12 Grado(Medio Sup.)")]
        DoceGrado,
        [Display(Name = "9no Grado(Medio)")]
        NovenoGrado,
        [Display(Name = " 6to Grado(Básico)")]
        SextoGrado,
        [Display(Name = "Menos de 6to Grado")]
        MenosDeSextoGrado

    }
}