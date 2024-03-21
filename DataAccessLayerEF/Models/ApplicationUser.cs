using DataAccessLayerEF.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace DataAccessLayerEF.Models;

public class ApplicationUser:IdentityUser
{
    [Required, MaxLength(30)]
    public string FirstName {  get; set; }

    [Required, MaxLength(30)]
    public string LastName { get; set; }

    [Required]
    public int Age { get; set; }

    [Required]
    public Gender Gender { get; set; }
    [JsonIgnore]
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
}
