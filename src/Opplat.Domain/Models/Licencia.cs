namespace Opplat.Domain.Models;
public class Licencia
{
    public int Id { get; set; }
    public string Aplicacion { get; set; } = string.Empty;
    public string Subscriptor { get; set; } = string.Empty;
    public DateTime Vencimiento { get; set; }
    public byte[] Hash { get; set; } = null!;
}
