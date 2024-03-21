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
        Task<bool> AnyAppointment(int patientId, int? clinicId, DateOnly date, bool IsDeleted, bool IsAttended);
        Task<bool> AnyHomeVisit(int patientId, int doctorId, DateOnly date,bool IsDeleted, bool IsAttended);

        int NumberOfRows { get; }
    }
}
