using Etammen.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Etammen.CustomValidation;

public class ValidOpenCloseHours:ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value is TimeOnly closingHour && validationContext.ObjectInstance is ClinicViewModel clinicvm)
        {
            if (closingHour > clinicvm.OpeningHour)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Closing hour must be after opening hour.");
            }
        }
        return new ValidationResult("Invalid time format.");
    }
}
