using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Opplat.Domain.Entities.Accounting;

[Table("contb_formas_de_pago")]
public class FormaDePago : BaseEntity
{
    [Required]
    [RegularExpression("[a-z A-Z,ñ,Ñ,í,ó,á,é,ú,Í,Ó,Á,É,Ú, ]*", ErrorMessage = "Solo letras")]
    public required string Nombre { get; set; }
}
