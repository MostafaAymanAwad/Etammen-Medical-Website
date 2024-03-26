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
        public DoctorDetailsViewModel MapToDoctorDetails(Doctor doctor)
        {

            var doctorDetails = new DoctorDetailsViewModel()
            {
                DoctorId = doctor.Id,
                AboutTheDoctor = doctor.AboutTheDoctor,
                Degree = doctor.Degree,
                ProfilePicture = doctor.ProfilePicture,
                TotalRatings = doctor.TotalRatings,
                Speciality = doctor.Speciality,
                YearsOfExperience = doctor.YearsOfExperience,
                ActualRting = doctor.ActualRting,

            };

            return doctorDetails;
        }
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
