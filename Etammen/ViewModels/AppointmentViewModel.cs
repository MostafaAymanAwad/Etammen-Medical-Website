using DataAccessLayerEF.Models;
using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Clinic Name")]
        public string ClinicName { get; set; }
        [Display(Name = "Doctor Name")]
        public string DoctorFirstName { get; set; }
        public string DoctorLastName { get; set; }
        [Display(Name = "Patient Name")]
        public string? PatientFirstName { get; set; }
        public string? PatientLastName { get; set; }

        public int ClinicId { get; set; }
        public int patientId { get; set; }
        public int DoctorId { get; set; }
        public virtual Clinic? Clinic { get; set; }
        public virtual Doctor? Doctor { get; set; }
        public virtual Patient? Patient { get; set; }
        [Display(Name = "Reservation Date")]
        public DateOnly Date { get; set; }
        [Display(Name = "Reservation Time")]
        public TimeOnly? ReservationPeriodNumber { get; set; }
        [Display(Name = "Attended")]
        public bool IsAttended { get; set; }
        
      
        [Display(Name = "Accepted")]

        public bool IsAccepted { get; set; }
        [Display(Name = "Online Payment")]

        public bool IsPaidOnline { get; set; }


    }
}
