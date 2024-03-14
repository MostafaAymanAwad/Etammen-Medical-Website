using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels;

public class ResetPasswordViewModel
{
    [Required, DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password), Compare("Password", ErrorMessage = "password confirmation doesn't match")]
    public string ConfirmPassword { get; set; }

    public string Token { get; set; }
    public string UserId { get; set; }
}
