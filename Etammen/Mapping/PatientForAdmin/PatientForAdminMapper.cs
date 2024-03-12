using DataAccessLayerEF.Models;
using Etammen.ViewModels.Admin.Doctor;
using Etammen.ViewModels.Admin.Patient;

namespace Etammen.Mapping.PatientForAdmin
{
    public class PatientForAdminMapper
    {
        public List<GetAllPatientViewModel> MapDoctorsToViewModel(IEnumerable<Patient> patients)
        {
            var viewModels = new List<GetAllPatientViewModel>();

            foreach (var ptient in patients)
            {
                var viewModel = new GetAllPatientViewModel
                {
                    Id = ptient.Id,
                    ApplicationUserId = ptient.ApplicationUserId,
                    FirstName = ptient.ApplicationUser.FirstName,
                    LastName = ptient.ApplicationUser.LastName,
                    Age = ptient.ApplicationUser.Age,
                    Gender = ptient.ApplicationUser.Gender,
                };

                viewModels.Add(viewModel);
            }

            return viewModels;
        }

        public GetOnePatientViewModel MapPatientToViewModel(Patient patient)
        {
            var patientViewModel = new GetOnePatientViewModel()
            {
                Id = patient.Id,
                ApplicationUserId = patient.ApplicationUserId,
                Age = patient.ApplicationUser.Age,
                Gender = patient.ApplicationUser.Gender,
                Address = patient.Address,
                FirstName = patient.ApplicationUser.FirstName,
                LastName = patient.ApplicationUser.LastName,
                
            };
            return patientViewModel;
        }

    }
}
