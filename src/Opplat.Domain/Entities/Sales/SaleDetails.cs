namespace Opplat.Domain.Entities.Sales;


public enum CustomerType
{
    Undefined,
    Girl,
    Boy,
    Woman,
    Man
}

public class OrderDetail
{
    public Guid SaleDetailId { get; set; }

    public SaleDetail? SaleDetail { get; set; }

    public int Position { get; set; }

    public CustomerType CustomerType { get; set; }

    public required string Observations { get; set; }

    public ICollection<Annotation> Annotations { get; set; } = [];
}

public class Annotation : BaseEntity
{
    public required string Acronym { get; set; }

    public required string Description { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; } = [];
}


public class SaleDetail : BaseEntity
{
    public Guid ProductId { get; set; }

    public ProductForSale? Product { get; set; }

    public ICollection<AddedTopping> Toppings { get; set; } = [];

    public Guid SaleId { get; set; }

    public Sale? Sale { get; set; }

    public int Quatity { get; set; }

    /// <summary>Line total including tax and after line discount.</summary>
    public decimal Amount { get; set; }

    /// <summary>Snapshots taken at sale time so later catalog edits never change a ticket.</summary>
    public string? ProductName { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TaxRate { get; set; }

    public decimal DiscountAmount { get; set; }

    public OrderDetail? OrderDetail { get; set; }
}
