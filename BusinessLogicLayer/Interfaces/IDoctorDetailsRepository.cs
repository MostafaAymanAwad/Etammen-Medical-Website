using DataAccessLayerEF.Models;
using Etammen.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IDoctorDetailsRepository
    {
        Task<IEnumerable<Clinic>> GetClinics(int id);
        Task<Clinic> GetClinicsDetails(int id);
        Task<string> FirstName(int id);
        Task<string> LastName(int id);
        Task<bool> IsAppointmentsAvailable(int id);
        Task<bool> IsReviewedBy(int DoctorId, int PatientID);
        Task<DoctorDetailsViewModel> GetDoctorDetailsViewModel(int doctorId, int patientId);
    }
}
