using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Inventory.Entities;

public class VoucherDetail
{
    [Key]
    [Column(Order = 1)]
    public Guid ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    [Key]
    [Column(Order = 2)]
    public Guid VoucherId { get; set; }

    public virtual ReleaseVoucher Voucher { get; set; } = null!;

    public decimal Quantity { get; set; }
}
