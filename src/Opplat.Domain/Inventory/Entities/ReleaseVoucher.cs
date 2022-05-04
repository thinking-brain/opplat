
using Opplat.Shared.Entities;

namespace Opplat.Domain.Inventory.Entities;

public class ReleaseVoucher: Entity
{
    public DateTime Date { get; set; }

    public Guid OriginId { get; set; }

    public virtual Storage Origin { get; set; }

    public Guid? DestinationId { get; set; }

    public virtual Storage Destination { get; set; }

    public string Description { get; set; }

    public string CreatedBy { get; set; }

    public string AuthorizedBy { get; set; }

    public virtual ICollection<VoucherDetail> Products { get; set; }

    public ReleaseVoucher()
    {
        Date = DateTime.UtcNow;
        Origin = null!;
        Description = String.Empty;
        CreatedBy = String.Empty;
        AuthorizedBy = String.Empty;
        Products = new HashSet<VoucherDetail>();
    }
}
