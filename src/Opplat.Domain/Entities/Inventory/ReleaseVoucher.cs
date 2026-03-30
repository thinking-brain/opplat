
namespace Opplat.Domain.Entities.Inventory;

public class ReleaseVoucher: BaseEntity
{
    public DateTime Date { get; set; }

    public Guid OriginId { get; set; }

    public virtual Storage? Origin { get; set; }

    public Guid? DestinationId { get; set; }

    public virtual Storage? Destination { get; set; }

    public string Description { get; set; }

    public string AuthorizedBy { get; set; }

    public virtual ICollection<VoucherDetail> Products { get; set; }

    public ReleaseVoucher()
    {
        Date = DateTime.UtcNow;
        Origin = null!;
        Description = string.Empty;
        CreatedBy = string.Empty;
        AuthorizedBy = string.Empty;
        Products = new HashSet<VoucherDetail>();
    }
}
