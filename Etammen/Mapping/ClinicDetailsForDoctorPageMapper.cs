using DataAccessLayerEF.Models;
using Etammen.ViewModels;

namespace Etammen.Mapping
{
    public class ClinicDetailsForDoctorPageMapper
    {
        public IEnumerable<ClinicDetailsInDoctorPageViewModel> MapToClinicDetailsInDoctorPageViewModel(IEnumerable<Clinic> clinics)
        {
            var clinicsVMS = new List<ClinicDetailsInDoctorPageViewModel>();

            foreach (var clinic in clinics)
            {
                var clinicVM = new ClinicDetailsInDoctorPageViewModel()
                {
                    Id = clinic.Id,
                    Name = clinic.Name,
                    Address = clinic.Address,
                    Fees = clinic.Fees,
                };
                clinicsVMS.Add(clinicVM);
            }
            return clinicsVMS;
        }
    }
}
