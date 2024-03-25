using DataAccessLayerEF.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DataAccessLayerEF.Models;

public class Clinic:BaseModel
{
    [Required]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public required string Name { get; set; }

    [Required]
    public int DoctorId { get; set; }
    [JsonIgnore]
    public Doctor? Doctor { get; set; }
    [Required]
    public required Address Address { get; set; }

    [Required, Column(TypeName ="money")]
    public decimal Fees { get; set; }
    [Required]
    public TimeOnly ExmainationDuration { get; set; }

    [Required]
    public TimeOnly OpeningHour { get; set; }

    [Required]
    public TimeOnly ClosingHour { get; set; }

    [Required]
    public OpeningDays OpeningDays { get; set; }

    public virtual ICollection<ClinicAppointment>? ClinicAppointments { get; set; } = new HashSet<ClinicAppointment>();
}

