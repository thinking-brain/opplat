namespace ContratacionWebApi.Models {
    public enum Tipo {
        Marco,
        Compra,
        Venta,
        Donación,
        Suministro,
        Deposito,
        Prestación_de_Servicio,
        Agencia,
        Comisión,
        Consignación,
        Arrendamiento,
        Transporte,
        Suplemento
    }

    public enum EstadoOrden {
        SinEstado,
        Nuevo,
        Circulando,
        Aprobado,
        No_Aprobado,
        Vigente,
        Cancelado,
        Vencido,
        Por_Dictaminar,
    }
    public enum EstadoSolicitud {
        Nueva,
        Aprobada,
        NoAprobada,
        Cancelada,
        Pagada
    }
    public enum FormaDePago {
        Transferencia_Bancaria,
        Cheque_Bancario,
        Efectivo

    }
    public enum NombreSucursal {
        Sin_Definir,
        Bandec,
        BPA,
        Banco_Metropolitano,
        BFI
    }
    public enum Moneda {
        Sin_Definir,
        USD,
        CUP,
        CUC,
        MLC
    }
    public enum Sector {
        Sin_Definir,
        Estatal,
        Cooperativo,
        TCP,
        PYME
    }
}