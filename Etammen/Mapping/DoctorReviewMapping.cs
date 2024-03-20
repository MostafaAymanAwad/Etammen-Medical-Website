using DataAccessLayerEF.Models;
using Etammen.ViewModels;

namespace Etammen.Mapping
{
    public class DoctorReviewMapping
    {
        public DoctorReviewViewModel MapFromEntityToViewModel(DoctorReviews doctorReviews)
        {
            var doctorReviewsViewModel = new DoctorReviewViewModel()
            {
                DoctorId = doctorReviews.DoctorId,
                PatientId = doctorReviews.PatientId,
                Comment = doctorReviews.Comment,
                Rate = doctorReviews.Rate,
            };

            return doctorReviewsViewModel;
        }

        public DoctorReviews MapFromViewModelToEntity(DoctorReviewViewModel doctorReviewsViewModel)
        {
            var doctorReviews = new DoctorReviews()
            {
                DoctorId = doctorReviewsViewModel.DoctorId,
                PatientId = doctorReviewsViewModel.PatientId,
                Comment = doctorReviewsViewModel.Comment,
                Rate = doctorReviewsViewModel.Rate,
            };

            return doctorReviews;
        }
    }
}
