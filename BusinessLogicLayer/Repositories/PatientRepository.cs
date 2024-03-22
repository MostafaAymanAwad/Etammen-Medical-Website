using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Context;
using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Repositories;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    private readonly EtammenDbContext _context;
    public int NumberOfRows { get; private set; }
    public PatientRepository(EtammenDbContext context):base(context)
    {
        _context = context;
        NumberOfRows = _context.Doctors.Count();
    }
    public  List<Doctor> PatientsPaginationNextAsync(List<Doctor> doctors,int pageNumber, int pageSize)
    {
        var Doctors =  doctors
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToList();
        
        return Doctors;
    }
    public int? GetSumOfRates(int id)
    {
        var doctorReviews = _context.DoctorReviews.Where(e => e.DoctorId == id).ToList();
        var sumOfRates = doctorReviews.Select(r => r.Rate).Sum();
        return sumOfRates;
    }

    public async Task<bool> AnyAppointment(int patientId, int? clinicId, DateOnly date, bool IsDeleted, bool IsAttended)
    {

        var query = await _context.Appointments.AnyAsync(a =>
        a.patientId == patientId &&
        a.ClinicId == clinicId &&
        a.Date == date&&
        a.IsAttended == IsAttended&&
        a.IsDeleted == IsDeleted
            );
        return query;
    }
    public async Task<bool> AnyHomeVisit(int patientId, int doctorId, DateOnly date, bool IsDeleted, bool IsAttended)
    {

        var query = await _context.HomeAppointments.AnyAsync(a =>
        a.PatientId == patientId &&
        a.DoctorId == doctorId &&
        a.Date == date&&
        a.IsDeleted == IsDeleted&&
        a.IsAttended == IsAttended

            );
        return query;
    }

    public int NumberOfRates(int id)
    {
        var doctorReviews = _context.DoctorReviews.Where(e => e.DoctorId == id).ToList();
        var sumOfRates = doctorReviews.Select(r => r.Rate).Count();
        return sumOfRates;
    }
    public async Task<Doctor> GetDoctorDetails(int id)
    {
        return await _context.Doctors
            .FirstOrDefaultAsync(e => e.Id == id);
    }
    public int GetPatientIdByUserId(string applicationUserID)
    {
        return  _context.Patients.Where(p => p.ApplicationUserId == applicationUserID)
                                 .Select(p => p.Id ).FirstOrDefault();
    }
}
