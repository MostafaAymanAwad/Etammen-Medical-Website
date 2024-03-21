using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels
{
    public class ClinicDetailsMapViewModel
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public  string Name { get; set; }
        public  string DoctorFirstName { get; set; }
        public  string DoctorLastName { get; set; }

        public int DoctorId { get; set; }
        public required Address Address { get; set; }

        public decimal Fees { get; set; }
        public TimeOnly ExmainationDuration { get; set; }

        public TimeOnly OpeningHour { get; set; }

        public TimeOnly ClosingHour { get; set; }

        public OpeningDays OpeningDays { get; set; }

        public virtual ICollection<ClinicAppointment>? Appointments { get; set; } = new HashSet<ClinicAppointment>();
    }
}
