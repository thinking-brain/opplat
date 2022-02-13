namespace opplatApplication.Utils;

public class Microservice
{
    public string Name { get; set; } = string.Empty;
    public string RootUrl { get; set; } = string.Empty;
    public List<string> Endpoints { get; set; } = new List<string>();
}
