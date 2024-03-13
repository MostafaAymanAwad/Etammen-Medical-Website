using DataAccessLayerEF.Models;
using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }
        
        public string ClinicName { get; set; }
        public string DoctorFirstName { get; set; }
        public string DoctorLastName { get; set; }

        public int ClinicId { get; set; }
        public int patientId { get; set; }
        public virtual Clinic? Clinic { get; set; }
        public virtual Patient? Patient { get; set; }
        public DateOnly Date { get; set; }
        public int ReservtionPeriodNumber { get; set; }
        public bool IsAttended { get; set; }
    }
}
