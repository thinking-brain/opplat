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
        NoAprobado,
        Vigente,
        Cancelado,
        Vencido,
        Revision,
        SinEstado
    }
}