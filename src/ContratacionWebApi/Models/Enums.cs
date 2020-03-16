namespace ContratacionWebApi.Models
{
    public enum Tipo
    {
        Marco,
        Compra,
        Venta,
        Servicio,
    }
    
    public enum Estado
    {
        Nuevo,
        Circulando,
        Aprobado,
        AprobadoEconomico,
        AprobadoJuridico,
        NoAprobado,
        NoAprobadoEconomico,
        NoAprobadoJuridico,
        Vigente,
        Cancelado,
        Vencido,
        Revision,
        SinEstado
    }
}