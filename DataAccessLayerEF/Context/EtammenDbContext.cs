using DataAccessLayerEF.Models;
using DataAccessLayerEF.ModelsConfigurations;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace DataAccessLayerEF.Context;

public class EtammenDbContext:IdentityDbContext<ApplicationUser>
{
    //private readonly IConfiguration _configuration;
    public virtual DbSet<Doctor> Doctors { get; set;}
    public virtual DbSet<Clinic> Clinics { get; set;}
    public virtual DbSet<Patient> Patients { get; set;}
    public virtual DbSet<DoctorReviews> DoctorReviews { get; set;}
    public virtual DbSet<Appointment> Appointments { get; set;}

    public EtammenDbContext(DbContextOptions options):base(options)
    {
    }
    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //  => optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
    protected override void OnModelCreating(ModelBuilder builder)
    {
       // builder.ApplyConfiguration(new DoctorConfigurtion());
        //builder.ApplyConfiguration(new PatientConfiguration());
        builder.ApplyConfiguration(new DoctorReviewsConfiguration());
        base.OnModelCreating(builder);
    }
}
