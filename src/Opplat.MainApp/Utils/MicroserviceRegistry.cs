namespace Opplat.MainApp.Utils;

public class MicroserviceRegistry
{
    public static ICollection<Microservice> Services { get; set; } = new List<Microservice>();
}
