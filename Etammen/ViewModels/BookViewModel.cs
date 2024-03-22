using DataAccessLayerEF.Models;
using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Doctor's First Name")]
        public string? ClinicName { get; set; }
        public string? DoctorFirstName { get; set; }
        public string? DoctorLastName { get; set; }
        public Dictionary<int, List<TimeOnly?>> ClinicAppointmentDictionary { get; set; } = new Dictionary<int, List<TimeOnly?>>();
        public int? ClinicId { get; set; }
        public int patientId { get; set; }
        public int DoctorId { get; set; }
        public virtual Clinic? Clinic { get; set; } 
        public virtual Doctor? Doctor { get; set; }
        [Display(Name = "Reservation Date")]
        public DateOnly Date { get; set; }
        [Required,Display(Name = "Reservation Time")]
        public TimeOnly? ReservtionPeriodNumber { get; set; }
        [Display(Name = "Attended")]
        public bool IsAttended { get; set; }
        public bool IsVisitHome { get; set; }
        public decimal? HomeVisitFees { get; set; }
        public decimal? ClinicFees { get; set; }
        public string? ProfilePicture { get; set; } = "";
        public string? YearsOfExperience { get; set; }

        public string? Speciality { get; set; }
        public string? Degree { get; set; }
        public bool ISHomeVisit { get; set; } 
        public bool IsWantToPayOnline {  get; set; }
        public bool ISHomeAppointmentDeleted { get; set; } 
        public bool ISClinicAppointmentDeleted { get; set; } 
        [Display(Name = "Pay Online")]

        public bool IsPaidOnline { get; set; }

    }
}
