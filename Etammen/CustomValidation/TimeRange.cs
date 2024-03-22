using System.ComponentModel.DataAnnotations;

namespace Etammen.CustomValidation;

public class TimeRange:ValidationAttribute
{
    private readonly TimeSpan _maxDuration;

    public TimeRange(int hours, int minutes)
    {
        _maxDuration = new TimeSpan(hours, minutes, 0);
    }
    
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is TimeOnly timeValue)
        {
            TimeSpan timeSpan = new TimeSpan(timeValue.Hour, timeValue.Minute, 0);
            if (timeSpan <= _maxDuration)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Examination Time Cannot Exceed 60 minutes (1 hour)");
            }
        }

        return new ValidationResult("Invalid time format.");
    }
}
