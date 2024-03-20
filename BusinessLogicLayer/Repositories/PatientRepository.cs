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
        public  List<Doctor> PatientsPaginationNextAsync(List<Doctor> doctors,int pageNumber, int pageSize)
        {
            var Doctors =  doctors
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize).ToList();
            
            return Doctors;
        }
        public async Task<Doctor> GetDoctorDetails(int id)
        {
            return await _context.Doctors
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
