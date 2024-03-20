using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Repositories
{
    public class ApplicationUserRepository : IApplicationUser
    {
        private readonly EtammenDbContext _context;
        public ApplicationUserRepository(EtammenDbContext context)
        {
            _context = context;    
        }
        public  string FirstName(int id)
        {
            var doctorFirstName =  _context.Users.Where(e=> e.Doctor.Id == id).Select(e => e.FirstName).FirstOrDefault();
            return doctorFirstName;
        }
        public string LastName(int id)
        {
            var doctorLastName = _context.Users.Where(e => e.Doctor.Id == id).Select(e => e.LastName).FirstOrDefault();
            return doctorLastName;
        }
    }
}
