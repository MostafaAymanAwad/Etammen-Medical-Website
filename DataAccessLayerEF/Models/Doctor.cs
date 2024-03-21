using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayerEF.Models;

public class Doctor:BaseModel
{
    [Key]
    public int Id { get; set; }
   
    [ForeignKey("ApplicationUser")]
    public string? ApplicationUserId { get; set; }

    [Required]
    public string AboutTheDoctor { get; set; }

    [Required]
    public string ProfilePicture {  get; set; }

    public string? Certificate {  get; set; }

    public string? Speciality {  get; set; }

    public string? Degree {  get; set; }

    public int? TotalRatings { get; set; }

    [Column(TypeName ="money")]
    public decimal? ActualRting { get; set; }

    [Required]
    public string YearsOfExperience { get; set; }
    [Column(TypeName = "money")]
    public decimal? HomeVisitFees { get; set; }

    [Required]
    public bool IsVisitHome { get; set; }
    
    public bool IsRegistered { get; set; }
    public virtual ICollection<Clinic>? Clinics { get; set; } = new HashSet<Clinic>();
    public virtual ApplicationUser? ApplicationUser { get; set; }
    public virtual ICollection<DoctorReviews>? DoctorReviews { get; set; } = new HashSet<DoctorReviews>();
    public virtual ICollection<HomeAppointment>? HomeAppointments { get; set; } = new HashSet<HomeAppointment>();

}
