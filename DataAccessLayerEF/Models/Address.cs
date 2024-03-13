using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayerEF.Models;

[ComplexType]

public class Address
{
    [Required,MaxLength(80),Display(Name ="Street Address")]
    public string StreetAddress { get; set; }

    [Required,MaxLength(80), Display(Name = "Governorate")]
    public string? governorate { get; set; }

    [Required,MaxLength(50)]
    public string City { get; set; }
}