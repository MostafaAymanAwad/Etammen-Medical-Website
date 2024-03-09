using System.ComponentModel.DataAnnotations;

namespace DataAccessLayerEF.Models;

public class Patient:BaseModel
{
    [Required]
    public string ApplicationUserId { get; set; } 

    public virtual ApplicationUser? ApplicationUser { get; set;}

    public Address Address { get; set; }

    public virtual ICollection<Appointment>? Appointments { get; set; } = new HashSet<Appointment>();
    public virtual ICollection<DoctorReviews>? DoctorReviews { get; set; } = new HashSet<DoctorReviews>();

   

}
