namespace Opplat.Domain.Entities.Accounting;

public class DepreciacionAft : Asiento
{
    public int AftId { get; set; }

    public virtual Aft? Aft { get; set; }
}

