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
        List<Doctor> PatientsPaginationNextAsync(List<Doctor> doctors, int pageNumber, int pageSize);
        int NumberOfRows { get; }
        int? GetSumOfRates(int id);
        int NumberOfRates(int id);
        Task<Doctor> GetDoctorDetails(int id);
    }
}
