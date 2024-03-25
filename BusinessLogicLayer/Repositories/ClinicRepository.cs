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
    public class ClinicRepository : IClinicRepository
    {
        private readonly EtammenDbContext _context;
        public ClinicRepository(EtammenDbContext context)
        {
            _context = context;
        }
        public IEnumerable<Clinic> GetClinicsNames(int id)
        {
            var clinics = _context.Clinics
                                   .Where(e => e.DoctorId == id)
                                   .Select(c => new Clinic { Name = c.Name, Address = c.Address, Fees = c.Fees, Id = c.Id })
                                   .ToList();
            return clinics;
        }

        public async Task<Clinic> GetClinics(int id)
     => await
     _context.Clinics
     .Include(e => e.Doctor)
     .ThenInclude(e => e.ApplicationUser)
     .FirstOrDefaultAsync(e => e.Id == id);

    }
}
