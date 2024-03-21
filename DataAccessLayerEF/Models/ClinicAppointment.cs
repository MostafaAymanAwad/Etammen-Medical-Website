using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerEF.Models
{
    public class ClinicAppointment
    {
        [Key]
        public int Id {  get; set; }

        public int? ClinicId { get; set; }
        public int patientId { get; set; }
        public virtual Clinic? Clinic { get; set; }
        public virtual Patient? Patient { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly? ReservationPeriodNumber { get; set; } 
        public bool IsAttended { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPaidOnline { get; set; }
        public bool IsAccepted { get; set; }
        public string? PaymentIntentId { get; set; }


    }
}
