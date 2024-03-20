using DataAccessLayerEF.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Etammen.ViewModels;

public class DoctorRegisterViewModel
{
    [Required, MaxLength(20, ErrorMessage = "first name is too long, maximum length is 20 characters")]
    public string FirstName { get; set; }

    [Required, MaxLength(20, ErrorMessage = "last name is too long, maximum length is 20 characters")]
    public string LastName { get; set; }

    [Required, MaxLength(20, ErrorMessage = "user name is too long, maximum length is 20 characters")]
    public string UserName { get; set; }

    [Required, Range(25,70,ErrorMessage ="Please, enter a valid age")]
    public int Age { get; set; }

    [Required,EnumDataType(typeof(Gender))]
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

    [Required]
    public string AboutTheDoctor { get; set; }

    [Required]
    public string Speciality { get; set; }
    public List<string> SpecialityList { get; set; }


    [Required]
    public string Degree { get; set; }
    public SelectList DegreeList { get; set; }


    [Required, Range(0,40,ErrorMessage = "The maximum years of Experience expected is 40 years.")]
    public string YearsOfExperience { get; set; }

    [Required]
    public bool IsVisitHome { get; set; }

    [Column(TypeName = "money"),Range(0,5000, ErrorMessage = "the maximum fees expected is 5000")]
    public decimal? HomeVisitFees { get; set; }
}
