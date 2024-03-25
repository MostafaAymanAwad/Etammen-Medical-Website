using DataAccessLayerEF.Models;
using DataAccessLayerEF.ModelsConfigurations;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Reflection.Emit;

namespace DataAccessLayerEF.Context;

public class EtammenDbContext:IdentityDbContext<ApplicationUser>
{

    public virtual DbSet<Doctor> Doctors { get; set;}
    public virtual DbSet<Clinic> Clinics { get; set;}
    public virtual DbSet<Patient> Patients { get; set;}
    public virtual DbSet<DoctorReviews> DoctorReviews { get; set;}
    public virtual DbSet<ClinicAppointment> Appointments { get; set;}
    public virtual DbSet<HomeAppointment> HomeAppointments { get; set;}

    public EtammenDbContext(DbContextOptions options):base(options)
    {
    }
  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DoctorReviewsConfiguration());

       
        base.OnModelCreating(modelBuilder);
    }
    
}
