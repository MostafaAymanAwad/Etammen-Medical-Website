using DataAccessLayerEF.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels.Admin.Doctor
{
    public class ApproveDoctorViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string AboutTheDoctor { get; set; }

        [Required]
        public string ProfilePicture { get; set; }

        [Required]
        public string Certificate { get; set; }

        [Required]
        public string Speciality { get; set; }

        [Required]
        public string Degree { get; set; }

        [Required]
        public string YearsOfExperience { get; set; }
        [Column(TypeName = "money")]
        public decimal? HomeVisitFees { get; set; }

        [Required]
        public bool IsVisitHome { get; set; }

    }
}
