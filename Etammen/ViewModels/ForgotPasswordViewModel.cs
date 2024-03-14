using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels;

    public class ForgotPasswordViewmodel
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber), MaxLength(13)]
        [RegularExpression(@"^(010|011|012)\d{4}\d{4}$", ErrorMessage = "phone number is not valid")]
        public string PhoneNumber { get; set; }

        [Required]
        public string ResetOption { get; set; }
    }

