using DataAccessLayerEF.Models;
using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Clinic Name")]
        public string ClinicName { get; set; }
        [Display(Name = "Doctor's First Name")]
        public string DoctorFirstName { get; set; }
        [Display(Name = "Doctor's Last Name")]
        public string DoctorLastName { get; set; }

        public int ClinicId { get; set; }
        public int patientId { get; set; }
        public virtual Clinic? Clinic { get; set; }
        public virtual Patient? Patient { get; set; }
        [Display(Name = "Reservation Date")]
        public DateOnly Date { get; set; }
        [Display(Name = "Reservation Time")]
        public int ReservtionPeriodNumber { get; set; }
        [Display(Name = "Attended")]
        public bool IsAttended { get; set; }
    }
}
