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

    public virtual ICollection<Appointment>? Appointments { get; set; } = new HashSet<Appointment>();
    public virtual ICollection<DoctorReviews>? DoctorReviews { get; set; } = new HashSet<DoctorReviews>();

   

}
