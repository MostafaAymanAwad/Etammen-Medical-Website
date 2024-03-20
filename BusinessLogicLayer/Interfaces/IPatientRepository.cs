using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Doctor>> PatientsPaginationNextAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Doctor>> PatientsPaginationNextAsyncFilter(int pageNumber, int pageSize, Expression<Func<Doctor, bool>> criteria);
        Task<IEnumerable<Doctor>> GetAllDoctors(Expression<Func<Doctor, bool>> criteria);
        Task<IEnumerable<Doctor>> GetAllDoctorszzz();
        int NumberOfRows { get; }
        IEnumerable<Doctor> GetAllDoctorsFilter(string[] degrees, decimal[] feess, int? gender, int pageNumber);
        int? GetSumOfRates(int id);
        int NumberOfRates(int id);

        Task<Doctor> GetDoctorDetails(int id);
    }
}
