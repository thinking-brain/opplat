namespace Opplat.Domain.Dtos.Accounting;
public record DenominacionViewModel
{
    public decimal Valor { get; set; }

    public bool Cup { get; set; }

    public bool Cuc { get; set; }

    public int CantidadCup { get; set; }

    public int CantidadCuc { get; set; }

    public int CantidadCupExtraccion { get; set; }

    public int CantidadCucExtraccion { get; set; }
}
