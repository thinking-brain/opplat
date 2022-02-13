
namespace Opplat.MainApp.Utils;
public class ContratacionContador
{
    public ContratacionContador() { }

    public string ContratosCant(string tipoTramite)
    {
        var cantidad = "";

        if (tipoTramite == "Ofertas")
        {
            cantidad = "0";
        }
        if (tipoTramite == "Contratos")
        {
            cantidad = "0";
        }
        return cantidad;
    }
}
