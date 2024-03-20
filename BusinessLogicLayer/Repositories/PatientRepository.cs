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

namespace BusinessLogicLayer.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly EtammenDbContext _context;
        public int NumberOfRows { get; private set; }
        public PatientRepository(EtammenDbContext context)
        {
            _context = context;
            NumberOfRows = _context.Doctors.Count();
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctors(Expression<Func<Doctor, bool>> criteria)
        {

            var doctors = await _context.Doctors.Include(user => user.ApplicationUser).Include(clinic => clinic.Clinics).Where(criteria).ToListAsync();
            return doctors;
        }

        public IEnumerable<Doctor> GetAllDoctorsFilter(string[] degrees, decimal[] feess, int? gender, int pageNumber)
        {
            var doctors = _context.Doctors
               .Include(user => user.ApplicationUser)
               .Include(clinic => clinic.Clinics).ToList();

            if (gender != null)
            {
                doctors.Where(d => d.ApplicationUser.Gender == 0);
            }
            if (degrees.Length > 0)
            {
                doctors.Where(d => degrees.Contains(d.Degree));
            }
            //if (degrees.Length > 0)
            //{
            //    doctors.Where(d => degrees.Contains(d.Degree));
            //}
            //if (feess.Length > 0)
            //{
            //    doctors = doctors.Where(d => d.Clinics.Any(c => feess.Contains(c.Fees)));
            //}


            return doctors
                .Skip((pageNumber - 1) * 5)
                .Take(5);
        }
        public async Task<IEnumerable<Doctor>> PatientsPaginationNextAsync(int pageNumber, int pageSize)
        {
            var numberOfRows = await _context.Doctors.CountAsync();
            var totalPages = (int)Math.Ceiling((double)numberOfRows / pageSize);

            if (pageNumber < 1)
                pageNumber = 1;

            var query = _context.Doctors.Include(user => user.ApplicationUser).Include(clinic => clinic.Clinics);
            var patients = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return patients;
        }

        public async Task<IEnumerable<Doctor>> PatientsPaginationNextAsyncFilter(int pageNumber, int pageSize, Expression<Func<Doctor, bool>> criteria)
        {
            var numberOfRows = await _context.Doctors.CountAsync();
            var totalPages = (int)Math.Ceiling((double)numberOfRows / pageSize);

            if (pageNumber < 1)
                pageNumber = 1;

            var query = _context.Doctors.Include(user => user.ApplicationUser).Include(clinic => clinic.Clinics).Where(criteria);
            var patients = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return patients;
        }

        public int? GetSumOfRates(int id)
        {
            var doctorReviews = _context.DoctorReviews.Where(e => e.DoctorId == id).ToList();
            var sumOfRates = doctorReviews.Select(r => r.Rate).Sum();
            return sumOfRates;
        }

        public int NumberOfRates(int id)
        {
            var doctorReviews = _context.DoctorReviews.Where(e => e.DoctorId == id).ToList();
            var sumOfRates = doctorReviews.Select(r => r.Rate).Count();
            return sumOfRates;
        }

        public Task<IEnumerable<Doctor>> GetAllDoctorszzz()
        {
            throw new NotImplementedException();
        }



        public async Task<Doctor> GetDoctorDetails(int id)
        {
            return await _context.Doctors
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
