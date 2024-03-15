using DataAccessLayerEF.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Etammen.ViewModels;

public class PatientRegisterViewModel
{
    [Required, MaxLength(20, ErrorMessage = "first name is too long, maximum length is 20 characters")]
    public string FirstName { get; set; }

    [Required, MaxLength(20, ErrorMessage = "last name is too long, maximum length is 20 characters")]
    public string LastName { get; set; }

    [Required, MaxLength(20, ErrorMessage = "user name is too long, maximum length is 20 characters")]
    public string UserName { get; set; }

    [Required, Range(25, 70, ErrorMessage = "Please, enter a valid age")]
    public int Age { get; set; }

    [Required, EnumDataType(typeof(Gender))]
    public Gender Gender { get; set; }

    [Required, DataType(DataType.Password)]
    public string Password { get; set; }

    [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "password doesn't match the original password")]
    public string ConfirmPassword { get; set; }

    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^(010|011|012)\d{4}\d{4}$", ErrorMessage = "phone number is not valid")]
    public string PhoneNumber { get; set; }


    [Required, MaxLength(80, ErrorMessage = "address is too long, maximum characters length is 80")]
    public string StreetAddress { get; set; }

    [Required]
    public string? governorate { get; set; }

    [Required]
    public string City { get; set; }

    public Dictionary<string,List<string>> GovCitiesDict { get; set; }
}