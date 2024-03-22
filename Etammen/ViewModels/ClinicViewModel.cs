using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Etammen.CustomValidation;

namespace Etammen.ViewModels
{
    public class ClinicViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required, MaxLength(100), Display(Name = "Clinic Name")]
        public required string Name { get; set; }

        public int DoctorId { get; set; }
        public string? DoctorName { get; set; }

    
        [Required, MaxLength(80), Display(Name = "Street Address")]
        public string StreetAddress { get; set; }

        [Required, MaxLength(80), Display(Name = "Governorate")]
        public string? governorate { get; set; }

        [Required, MaxLength(50)]
        public string City { get; set; }

        [Required, Column(TypeName = "money")]
        public decimal Fees { get; set; }



        [Required, DataType(DataType.Time),TimeRange(1, 0)]
        public TimeOnly ExmainationDuration { get; set; }

        [Required, DataType(DataType.Time)]
        public TimeOnly OpeningHour { get; set; }

        [Required, DataType(DataType.Time),ValidOpenCloseHours]
        public TimeOnly ClosingHour { get; set; }



        [Required]
        public OpeningDays OpeningDays { get; set; }
        public Doctor? Doctor { get; set; }

        public virtual ICollection<ClinicAppointment>? Appointments { get; set; } = new HashSet<ClinicAppointment>();
    }
}
