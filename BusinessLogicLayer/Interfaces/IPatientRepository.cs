using DataAccessLayerEF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IPatientRepository
    {
        List<Doctor> PatientsPaginationNextAsync(List<Doctor> doctors, int pageNumber, int pageSize);


        int NumberOfRows { get; }
    }
}
