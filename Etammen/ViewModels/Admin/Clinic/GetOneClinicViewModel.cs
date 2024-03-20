using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;

namespace Etammen.ViewModels.Admin.Clinic
{
    public class GetOneClinicViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? DoctorFirstName { get; set; }
        public string? DoctorLastName { get; set; }
        public int DoctorId { get; set; }
        public required DataAccessLayerEF.Models.Address Address { get; set; }
        public TimeOnly OpeningHour { get; set; }

        public TimeOnly ClosingHour { get; set; }

        public OpeningDays OpeningDays { get; set; }

        public decimal Fees { get; set; }

    }
}
