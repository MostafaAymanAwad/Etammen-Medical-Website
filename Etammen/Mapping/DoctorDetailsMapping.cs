using DataAccessLayerEF.Models;
using Etammen.ViewModels;

namespace Etammen.Mapping
{
    public class DoctorDetailsMapping
    {
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
    }
}
