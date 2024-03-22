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
    public interface IPatientRepository:IGenericRepository<Patient>
    {
        List<Doctor> PatientsPaginationNextAsync(List<Doctor> doctors, int pageNumber, int pageSize);
        Task<bool> AnyAppointment(int patientId, int? clinicId, DateOnly date, bool IsDeleted, bool IsAttended);
        Task<bool> AnyHomeVisit(int patientId, int doctorId, DateOnly date,bool IsDeleted, bool IsAttended);

        int NumberOfRows { get; }
        int? GetSumOfRates(int id);
        int NumberOfRates(int id);
        Task<Doctor> GetDoctorDetails(int id);
        int GetPatientIdByUserId(string applicationUserID);

    }
}
