﻿using DataAccessLayerEF.Models;
using DataAccessLayerEF.ModelsConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayerEF.Context;

public class EtammenDbContext:IdentityDbContext<ApplicationUser>
{
    
    public virtual DbSet<Doctor> Doctors { get; set;}
    public virtual DbSet<Clinic> Clinics { get; set;}
    public virtual DbSet<Patient> Patients { get; set;}
    public virtual DbSet<DoctorReviews> DoctorReviews { get; set;}
    public virtual DbSet<Appointment> Appointments { get; set;}

    public EtammenDbContext()
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=EtammenDb;Integrated Security=true;Encrypt=false; Trust Server Certificate=true");
        }
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new DoctorReviewsConfiguration());
        base.OnModelCreating(builder);
    }
}
