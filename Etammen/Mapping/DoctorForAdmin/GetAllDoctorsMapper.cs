using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;
using Etammen.ViewModels.Admin.Doctor;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etammen.Mapping.DoctorForAdmin
{
    public class DoctorsAdminMapper
    {

        public List<GetAllDoctorsViewModel> MapDoctorsToViewModel(IEnumerable<Doctor> doctors)
        {
            var viewModels = new List<GetAllDoctorsViewModel>();

            foreach (var doctor in doctors)
            {
                var viewModel = new GetAllDoctorsViewModel
                {
                    Id = doctor.Id,
                    ApplicationUserId = doctor.ApplicationUserId,
                    FirstName = doctor.ApplicationUser.FirstName,
                    LastName = doctor.ApplicationUser.LastName,
                    Age = doctor.ApplicationUser.Age,
                    Gender = doctor.ApplicationUser.Gender,
                    Speciality = doctor.Speciality,
                    ApplicationUser = doctor.ApplicationUser,
                };

                viewModels.Add(viewModel);
            }

            return viewModels;
        }

        public GetDoctorByIdViewModel MapOneDoctorToViewModel(Doctor doctor)
        {
            var doctorViewModel = new GetDoctorByIdViewModel
            {
                Id = doctor.Id,
                ApplicationUserId = doctor.ApplicationUserId,
                FirstName = doctor.ApplicationUser.FirstName,
                LastName = doctor.ApplicationUser.LastName,
                Age = doctor.ApplicationUser.Age,
                Gender = doctor.ApplicationUser.Gender,
                Speciality = doctor.Speciality,
                ApplicationUser = doctor.ApplicationUser,
                Degree = doctor.Degree,
                AboutTheDoctor = doctor.AboutTheDoctor,
                Certificate = doctor.Certificate,
                ProfilePicture = doctor.ProfilePicture,
                TotalRatings = doctor.TotalRatings,
                YearsOfExperience = doctor.YearsOfExperience,

            };

            return doctorViewModel;
        }

        

        public IEnumerable<GetDoctorByIdViewModel> MapDoctorEntitiesToDoctorViewModel(IEnumerable<Doctor> doctors)
        {
            var viewModels = new List<GetDoctorByIdViewModel>();

            foreach (var doctor in doctors)
            {
                var doctorViewModel = new GetDoctorByIdViewModel
                {
                    Id = doctor.Id,
                    ApplicationUserId = doctor.ApplicationUserId,
                    FirstName = doctor.ApplicationUser.FirstName,
                    LastName = doctor.ApplicationUser.LastName,
                    Age = doctor.ApplicationUser.Age,
                    Gender = doctor.ApplicationUser.Gender,
                    Speciality = doctor.Speciality,
                    ApplicationUser = doctor.ApplicationUser,
                    Degree = doctor.Degree,
                    AboutTheDoctor = doctor.AboutTheDoctor,
                    Certificate = doctor.Certificate,
                    ProfilePicture = doctor.ProfilePicture,
                    TotalRatings = doctor.TotalRatings,
                    YearsOfExperience = doctor.YearsOfExperience,

                };

                viewModels.Add(doctorViewModel);
            }

            return viewModels;


        }

    }

}
