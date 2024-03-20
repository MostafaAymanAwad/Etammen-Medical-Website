using DataAccessLayerEF.Models;

namespace Etammen.ViewModels
{
    public class DoctorReviewViewModel
    {
        public int DoctorId { get; set; }

        public int PatientId { get; set; }

        public int? Rate { get; set; }

        public string? Comment { get; set; }
    }
}
