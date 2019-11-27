using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RhWebApi.Models;

namespace RhWebApi.Models {
    public enum NivelDeEscolaridad {
        [Description ("Sin Definir")]
        SinDefinir,

        [Description ("NivelSuperior")]
        NivelSuperior,

        [Description ("Tecnico Medio")]
        TecnicoMedio,

        [Description ("Doce Grado")]
        DoceGrado,

        [Description ("Noveno Grado")]
        NovenoGrado,

        [Description ("Sexto Grado")]
        SextoGrado,

        [Description ("Menos De Sexto Grado")]
        MenosDeSextoGrado
    }
}