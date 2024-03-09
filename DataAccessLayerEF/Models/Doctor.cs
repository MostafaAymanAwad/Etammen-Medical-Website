using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayerEF.Models;

public class Doctor:BaseModel
{
    [Required]
    public string ApplicationUserId { get; set; }

    [Required]
    public string AboutTheDoctor { get; set; }

    [Required]
    public string ProfilePicture {  get; set; }
    [Required]
    public string Speciality {  get; set; }

    [Required]
    public string Degree {  get; set; }

    public int? TotalRatings { get; set; }

    [Column(TypeName ="money")]
    public decimal? ActualRting { get; set; }

    [Required]
    public string YearsOfExperience { get; set; }
    [Column(TypeName = "money")]
    public decimal? HomeVisitFees { get; set; }

    [Required]
    public bool IsVisitHome { get; set; }
    public virtual ICollection<Clinic>? Clinics { get; set; } = new HashSet<Clinic>();
    public virtual ApplicationUser ApplicationUser { get; set; }
    public virtual ICollection<DoctorReviews>? DoctorReviews { get; set; } = new HashSet<DoctorReviews>();
}
