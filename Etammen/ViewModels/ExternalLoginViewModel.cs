using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Etammen.ViewModels;

public class ExternalLoginViewModel

{
    [Required, MaxLength(30, ErrorMessage = "username is too long, maximum characters length is 30")]
    public string UserName { get; set; }

    [Required, Range(18,100,ErrorMessage ="please enter a valid age")]
    public int Age { get; set; }

    [Required, MaxLength(80, ErrorMessage = "address is too long, maximum characters length is 80")]
    public string StreetAddress { get; set; }

    [Required]
    public string? governorate { get; set; }

    [Required]
    public string City { get; set; }

    public Dictionary<string, List<string>> GovCitiesDict { get; set; }

    public string Provider { get; set; }
    public ClaimsPrincipal Principal { get; set; }
}
