namespace TallerWebApi.Models {

    public enum EstadoOrdenReparacion {
        Sin_Reparar,
        En_Reparacion,
        Entregado,
    }
    public enum EstadoEquipo {
        Ninguno,
        Dañado,
        Sin_Reparacion,
        Ok
    }
    public enum LugarReparacion {
        Taller,
        A_Domicilio
    }
    public enum EstadoPresupuesto {
        Sin_Presupuestar,
        Presupuestado,
        Aceptado,
        No_Aceptado
    }
    public enum SexoCliente {
        M,
        F
    }
}