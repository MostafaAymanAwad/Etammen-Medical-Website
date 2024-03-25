using DataAccessLayerEF.Models;

namespace Etammen.ViewModels
{
    public class ClinicDetailsInDoctorPageViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public decimal Fees { get; set; }
    }
}
