using DataAccessLayerEF.Models;
using Etammen.ViewModels;

namespace Etammen.Mapping
{
    public class ClinicDetailsMapViewModelMapper
    {
        public ClinicDetailsMapViewModel ClinicMapper(Clinic clinic)
        {
            var clinicVM = new ClinicDetailsMapViewModel()
            {
                Address = clinic.Address,
                Name = clinic.Name,
                Id = clinic.Id,
                Appointments = clinic.ClinicAppointments,
                ClosingHour = clinic.ClosingHour,
                DoctorId = clinic.DoctorId,
                ExmainationDuration = clinic.ExmainationDuration,
                Fees = clinic.Fees,
                OpeningDays = clinic.OpeningDays,
                OpeningHour = clinic.OpeningHour,
                DoctorFirstName = clinic.Doctor.ApplicationUser.FirstName,
                DoctorLastName = clinic.Doctor.ApplicationUser.LastName,

            };
            return clinicVM;
        }
    }
}
