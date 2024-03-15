using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Etammen.ViewModels.Admin.Doctor
{
    public class GetDoctorByIdViewModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
 
        public int Age { get; set; }
        public Gender Gender { get; set; }

        public string? ApplicationUserId { get; set; }

        public string? AboutTheDoctor { get; set; }

        public string? ProfilePicture { get; set; }

        public string? Certificate { get; set; }

        public string? Speciality { get; set; }

        public string? Degree { get; set; }

        public int? TotalRatings { get; set; }
        public string? YearsOfExperience { get; set; }
        public IEnumerable<DataAccessLayerEF.Models.Clinic>? Clinics { get; set; } = new HashSet<DataAccessLayerEF.Models.Clinic>();

        public ApplicationUser? ApplicationUser { get; set; }

    }
}
