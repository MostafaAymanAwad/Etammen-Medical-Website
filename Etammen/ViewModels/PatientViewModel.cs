using DataAccessLayerEF.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Etammen.ViewModels
{
    public class PatientViewModel
    {
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string? ApplicationUserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Age { get; set; }
        
        public Address? Address { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }

        

        public virtual ICollection<ClinicAppointment>? Appointments { get; set; } = new HashSet<ClinicAppointment>();
        public virtual ICollection<DoctorReviews>? DoctorReviews { get; set; } = new HashSet<DoctorReviews>();
    }
}
