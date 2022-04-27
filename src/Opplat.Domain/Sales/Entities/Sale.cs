using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Opplat.Shared.Entities;

namespace Opplat.Domain.Sales.Entities;

public class Discount: IEntity<int>
{
    public int Id { get; set; }

    public Guid SaleId { get; set; }

    public Sale Sale { get; set; }

    public decimal Amount { get; set; }

    public bool IsPercent { get; set; }
    public string User { get; set; }
}

public class SaleDetail
{
    public Guid ProductId { get; set; }

    public Product Product { get; set; }

    public Guid SaleId { get; set; }

    public Sale Sale { get; set; }

    public int Quatity { get; set; }

    public decimal Amount { get; set; }
}

public class Sale: Entity
{
    public DateTime Date { get; set; }

    public string CreatedBy { get; set; }

    public ICollection<SaleDetail> Products { get; set; }

    public decimal TotalAmount { get; set; }

    public ICollection<Discount> Discounts { get; set; }

    public Sale()
    {
        Products = new HashSet<SaleDetail>();
        Discounts = new HashSet<Discount>();
    }

    private decimal ApplyAllDiscount(decimal amount)
    {
        var percent = 0m;
        var discountAmount = 0m;
        foreach (var discount in Discounts)
        {
            if(discount.IsPercent){
                percent += discount.Amount;
            }
            else
            {
                discountAmount += discount.Amount;
            }
        }
        if (percent > 0)
        {
            amount = amount * percent / 100;
        }
        amount -= discountAmount;
        return amount;
    }

    private decimal ApplyIndividualDiscount(decimal amount, Discount discount)
    {
        if (discount.IsPercent)
        {
            return amount * discount.Amount / 100;
        }
        return amount - discount.Amount;
    }

    public decimal CalculateTotalAmount()
    {
        var total = Products.Sum( p => p.Amount);
        total = ApplyAllDiscount(total);
        return total;
    }
}
