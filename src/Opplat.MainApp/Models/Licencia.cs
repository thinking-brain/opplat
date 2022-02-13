namespace Opplat.MainApp.Models;
public class Licencia
{
    public int Id { get; set; }
    public string Aplicacion { get; set; } = String.Empty;
    public string Subscriptor { get; set; } = String.Empty;
    public DateTime Vencimiento { get; set; }
    public byte[] Hash { get; set; } = null!;
}
