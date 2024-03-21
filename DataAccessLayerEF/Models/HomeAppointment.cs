using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayerEF.Models
{
    public class HomeAppointment
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        [ForeignKey("Doctor")]

        public int DoctorId { get; set; }
        public DateOnly? Date { get; set; }
        public bool IsAttended { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsPaidOnline { get; set; }
        public virtual Patient? Patient { get; set; }
        public string? PaymentIntentId { get; set; }
        public Doctor? Doctor { get; set; }

    }
}
