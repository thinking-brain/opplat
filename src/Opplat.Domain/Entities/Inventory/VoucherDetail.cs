namespace Opplat.Domain.Entities.Inventory;

public class VoucherDetail
{
    public Guid ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public Guid VoucherId { get; set; }

    public virtual ReleaseVoucher Voucher { get; set; } = null!;

    public decimal Quantity { get; set; }
}
