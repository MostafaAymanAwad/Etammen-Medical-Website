using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Context;
using DataAccessLayerEF.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IEnumerable<Doctor>> PatientsPaginationNextAsync(int pageNumber, int pageSize)
        {
            var numberOfRows = await _context.Doctors.CountAsync();
            var totalPages = (int)Math.Ceiling((double)numberOfRows / pageSize);

            if (pageNumber < 1 || pageNumber > totalPages)
            {
                throw new ArgumentException("Invalid page number");
            }
            var query = _context.Doctors.Include(user => user.ApplicationUser).Include(clinic => clinic.Clinics);
            var patients = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return patients;
        }


        public Task<IEnumerable<Doctor>> PatientsPaginationPreviousAsync()
        {



            throw new NotImplementedException();
        }
    }
}
