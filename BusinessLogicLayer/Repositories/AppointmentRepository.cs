using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly EtammenDbContext _context;
        public AppointmentRepository(EtammenDbContext context) 
        {
            _context = context;
        }

        public bool IsAppointmentsAvailable(int id)
        {
            var IsAttend = _context.Appointments.Where(e => e.patientId == id).Any(e => e.IsAttended == true);
            return IsAttend;
        }
    }
}
