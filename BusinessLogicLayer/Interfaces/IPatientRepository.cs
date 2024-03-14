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
        Task<IEnumerable<Doctor>> PatientsPaginationNextAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Doctor>> PatientsPaginationPreviousAsync();
        int NumberOfRows { get; }
    }
}
