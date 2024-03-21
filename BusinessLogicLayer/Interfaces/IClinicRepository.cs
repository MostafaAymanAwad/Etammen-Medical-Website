using DataAccessLayerEF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IClinicRepository
    {
        IEnumerable<Clinic> GetClinicsNames(int id);
        Task<Clinic> GetClinics(int id);
    }
}
