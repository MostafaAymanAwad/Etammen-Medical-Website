using DataAccessLayerEF.Models;
using System.ComponentModel.DataAnnotations;

namespace Etammen.ViewModels.Admin.Clinic
{
    public class GetAllClinicsViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int DoctorId { get; set; }
        public required DataAccessLayerEF.Models.Address Address { get; set; }
    }
}
