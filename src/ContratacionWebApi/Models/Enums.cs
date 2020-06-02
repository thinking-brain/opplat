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

    }

    public enum Estado {
        Nuevo,
        Circulando,
        Aprobado,
        No_Aprobado,
        Vigente,
        Cancelado,
        Vencido,
        Revision,
        SinEstado
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
        Bandec,
        BPA,
        Banco_Metropolitano,
        BFI
    }
    public enum Moneda {
        CUC,
        MN,
        USD
    }
    public enum Sector {
        Sin_Definir,
        Estatal,
        Cooperativo,
        TCP,
        PYME
    }
}