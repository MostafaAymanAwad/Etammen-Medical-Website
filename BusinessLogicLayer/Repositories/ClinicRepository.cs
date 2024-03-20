using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Context;
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
        public IEnumerable<string> GetClinicsNames(int id)
        {
            var doctorsName = _context.Clinics.Where(e => e.DoctorId == id)
                .Select(c => c.Name)
                .ToList();
           return doctorsName;
        }
    }
}
