using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayerEF.Models;

public class Patient:BaseModel
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("ApplicationUser")]
    public string? ApplicationUserId { get; set; } 

    public virtual ApplicationUser? ApplicationUser { get; set;}

    public Address Address { get; set; }

    public virtual ICollection<ClinicAppointment>? ClinicAppointments { get; set; } = new HashSet<ClinicAppointment>();
    public virtual ICollection<HomeAppointment>? HomeAppointments { get; set; } = new HashSet<HomeAppointment>();
    public virtual ICollection<DoctorReviews>? DoctorReviews { get; set; } = new HashSet<DoctorReviews>();

   

}
