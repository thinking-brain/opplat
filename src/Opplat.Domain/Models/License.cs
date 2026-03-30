namespace Opplat.Domain.Models;
public class License
{
    public int Id { get; set; }
    public string Application { get; set; } = string.Empty;
    public string Subscriber { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public byte[] Hash { get; set; } = null!;
}
