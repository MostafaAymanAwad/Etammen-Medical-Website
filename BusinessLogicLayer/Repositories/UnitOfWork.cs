using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Context;
using DataAccessLayerEF.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _isDisposed;
        private readonly EtammenDbContext _context;
        public IGenericRepository<Clinic> Clinics { get; private set; }
        public IDoctorRepository Doctors {  get; private set; }

        public IGenericRepository<Patient> Patients { get; private set; }

        public IGenericRepository<Appointment> Appointments { get; private set; }

        public IGenericRepository<DoctorReviews> DoctorReviews { get; private set; }
        public UnitOfWork(EtammenDbContext context)
        {
            _context = context;
            Clinics = new GenericRepository<Clinic>(_context);
            Doctors = new DoctorRepository(_context);
            Patients= new GenericRepository<Patient>(_context);
            Appointments = new GenericRepository<Appointment>(_context);
            DoctorReviews = new GenericRepository<DoctorReviews>(_context);
        }

        public async Task<int> Commit()
        {
            return await _context.SaveChangesAsync();
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool isDisposing)
        {
            if(!_isDisposed)
               _context.Dispose();
        }
    }
}
