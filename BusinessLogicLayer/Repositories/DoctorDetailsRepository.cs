using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Context;
using DataAccessLayerEF.Models;
using Etammen.Mapping;
using Etammen.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Repositories
{
    public class DoctorDetailsRepository : IDoctorDetailsRepository
    {
        private readonly EtammenDbContext _context;
        private readonly IPatientRepository _patientRepository;
        private readonly DoctorReviewMapping _doctorReviewMapper;

        public DoctorDetailsRepository(EtammenDbContext context, IPatientRepository patientRepository, DoctorReviewMapping doctorReviewMapper)
        {
            _context = context;
            _patientRepository = patientRepository;
            _doctorReviewMapper = doctorReviewMapper;   
        }
        public Task<string> FirstName(int id)
        {
            var doctorFirstName = _context.Users
                .Where(e => e.Doctor.Id == id)
                .Select(e => e.FirstName)
                .FirstOrDefaultAsync();
            return doctorFirstName;
        }

        public Task<string> LastName(int id)
        {
            var doctorLastName = _context.Users
                .Where(e => e.Doctor.Id == id)
                .Select(e => e.LastName)
                .FirstOrDefaultAsync();
            return doctorLastName;
        }

        public async Task<Clinic> GetClinicsDetails(int id)
        {
            return await _context.Clinics
                        .Include(e => e.Doctor)
                        .ThenInclude(e => e.ApplicationUser)
                        .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Clinic>> GetClinics(int id)
        {
            var clinics = await _context.Clinics
                                   .Where(e => e.DoctorId == id)
                                   .Select(c => new Clinic { Name = c.Name, Address = c.Address, Fees = c.Fees, Id = c.Id })
                                   .ToListAsync();
            return clinics;
        }

        public async Task<bool> IsAppointmentsAvailable(int id)
        {
            var IsAttend = await _context.Appointments
                .Where(e => e.patientId == id)
                .AnyAsync(e => e.IsAttended == true);
            return IsAttend;
        }

        public async Task<bool> IsReviewedBy(int DoctorId, int PatientID)
        {
            var existingReview = await _context.DoctorReviews
             .AnyAsync(r => r.DoctorId == DoctorId && r.PatientId == PatientID);

            return existingReview;
        }

        public async Task<DoctorDetailsViewModel> GetDoctorDetailsViewModel(int doctorId, int patientId)
        {
            var doctor = await _patientRepository.GetDoctorDetails(doctorId);
            var clinicDetails = await GetClinics(doctorId);

            var doctorDetailsViewModel = _doctorReviewMapper.MapToDoctorDetails(doctor);
            doctorDetailsViewModel.PatientId = patientId;
            doctorDetailsViewModel.Clinics = _doctorReviewMapper.MapToClinicDetailsInDoctorPageViewModel(clinicDetails);
            doctorDetailsViewModel.FirstName = await FirstName(doctorId);
            doctorDetailsViewModel.LastName = await LastName(doctorId);
            doctorDetailsViewModel.IsAttended = await IsAppointmentsAvailable(patientId);
            doctorDetailsViewModel.IsReview = await IsReviewedBy(doctorId, patientId);

            return doctorDetailsViewModel;
        }
    }
}
