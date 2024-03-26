using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;

namespace Etammen.ViewModels
{
    public class DoctorDetailsViewModel
    {
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public string? AboutTheDoctor { get; set; }
        public string? ProfilePicture { get; set; }
        public decimal? ActualRting { get; set; }
        public string? Speciality { get; set; }
        public string? Degree { get; set; }
        public int? TotalRatings { get; set; }
        public string? YearsOfExperience { get; set; }

        public int PatientId { get; set; }

        public int? Rate { get; set; }

        public string? Comment { get; set; }

        public bool? IsAttended { get; set; }
        public bool? IsReview { get; set; }
        public IEnumerable<ClinicDetailsInDoctorPageViewModel>? Clinics { get; set; }
        public IEnumerable<OpeningDays> OpeningDays { get; set; }
    }
}
