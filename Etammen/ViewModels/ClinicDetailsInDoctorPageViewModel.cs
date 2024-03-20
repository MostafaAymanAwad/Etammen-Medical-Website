using DataAccessLayerEF.Models;

namespace Etammen.ViewModels
{
    public class ClinicDetailsInDoctorPageViewModel
    {
        public int id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public decimal Fees { get; set; }
    }
}
