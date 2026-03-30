using System.ComponentModel.DataAnnotations;

namespace Opplat.Application.Dtos;

public class ResetPassword
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
