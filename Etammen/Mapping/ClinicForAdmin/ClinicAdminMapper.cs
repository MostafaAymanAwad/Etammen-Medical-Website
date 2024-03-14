using DataAccessLayerEF.Models;
using Etammen.ViewModels.Admin.Clinic;

namespace Etammen.Mapping.ClinicForAdmin
{
    public class ClinicAdminMapper
    {
        public IEnumerable<GetAllClinicsViewModel> MapClinicsToGetAllViewModel(IEnumerable<Clinic> clinics)
        {
            var clinicViewModels = new List<GetAllClinicsViewModel>();
            foreach (var clinic in clinics)
            {
                var clinicViewModel = new GetAllClinicsViewModel()
                {
                    Id = clinic.Id,
                    Address = clinic.Address,
                    Name = clinic.Name,
                    DoctorId = clinic.DoctorId,
                };
                clinicViewModels.Add(clinicViewModel);
            }
            return clinicViewModels;
        }

        public GetOneClinicViewModel MapOneClinicToViewModel(Clinic clinic)
        {
            var clinicViewModel = new GetOneClinicViewModel()
            {
                Id = clinic.Id,
                Address = clinic.Address,
                Name = clinic.Name,
                DoctorId = clinic.DoctorId,
                OpeningHour = clinic.OpeningHour,
                ClosingHour = clinic.ClosingHour,
                OpeningDays = clinic.OpeningDays,
                Fees = clinic.Fees,
            };
            return clinicViewModel;
        }
    }
}
