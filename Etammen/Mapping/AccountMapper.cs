using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;
using Etammen.ViewModels;
using System.Security.Claims;

namespace Etammen.Mapping;

public class AccountMapper
{
    public ApplicationUser GetUserFromVM(DoctorRegisterViewModel doctorRegisterViewModel)
    {
        ApplicationUser user = new ApplicationUser()
        {
            FirstName = doctorRegisterViewModel.FirstName,
            LastName = doctorRegisterViewModel.LastName,
            UserName = doctorRegisterViewModel.UserName,
            Age = doctorRegisterViewModel.Age,
            Gender = doctorRegisterViewModel.Gender,
            Email = doctorRegisterViewModel.Email,
            PhoneNumber = doctorRegisterViewModel.PhoneNumber
        };
        return user;
    }
    public Doctor GetDoctorFromVM(DoctorRegisterViewModel doctorRegisterViewModel, string applicationUserId, List<string> doctorImages)
    {
        Doctor doctor = new Doctor()
        {
            ApplicationUserId = applicationUserId,
            AboutTheDoctor = doctorRegisterViewModel.AboutTheDoctor,
            Speciality = doctorRegisterViewModel.Speciality,
            Degree = doctorRegisterViewModel.Degree,
            YearsOfExperience = doctorRegisterViewModel.YearsOfExperience,
            IsVisitHome = doctorRegisterViewModel.IsVisitHome,
            HomeVisitFees = doctorRegisterViewModel.HomeVisitFees,
            Certificate = doctorImages[0],
            ProfilePicture = doctorImages[1]
        };
        return doctor;
    }


    public ApplicationUser GetUserFromVM(PatientRegisterViewModel patientRegisterViewModel)
    {
        ApplicationUser user = new ApplicationUser()
        {
            FirstName = patientRegisterViewModel.FirstName,
            LastName = patientRegisterViewModel.LastName,
            UserName = patientRegisterViewModel.UserName,
            Age = patientRegisterViewModel.Age,
            Gender = patientRegisterViewModel.Gender,
            Email = patientRegisterViewModel.Email,
            PhoneNumber = patientRegisterViewModel.PhoneNumber
        };
        return user;
    }
    public Patient GetPatientFromVM(PatientRegisterViewModel patientRegisterViewModel, string applicationUserId)
    {
        Patient patient = new Patient()
        {
            ApplicationUserId = applicationUserId,
            Address = new Address()
            {
                StreetAddress = patientRegisterViewModel.StreetAddress,
                governorate = patientRegisterViewModel.governorate,
                City = patientRegisterViewModel.City
            }
        };
        return patient;
    }

    public ApplicationUser GetUserFromExternalLoginViewModel(ExternalLoginViewModel externalloginViewModel)
    {
        ApplicationUser applicationUser = new ApplicationUser()
        {
            FirstName = externalloginViewModel.Principal.FindFirstValue(ClaimTypes.GivenName),
            LastName = externalloginViewModel.Principal.FindFirstValue(ClaimTypes.Surname),
            UserName = externalloginViewModel.UserName,
            Age = externalloginViewModel.Age,
            Gender = String.Compare(externalloginViewModel.Principal.FindFirstValue(ClaimTypes.Gender), "male", StringComparison.OrdinalIgnoreCase) == 0? Gender.Male:Gender.Female,
            Email = externalloginViewModel.Principal.FindFirstValue(ClaimTypes.Email),
            PhoneNumber = externalloginViewModel.Principal.FindFirstValue(ClaimTypes.MobilePhone),
            EmailConfirmed = true
        };
        return applicationUser;
    }

    public Patient GetPatientFromExternalLoginViewModel(ExternalLoginViewModel externalloginViewModel, string applicationUserId)
    {
        Patient newPatient = new Patient()
        {
            ApplicationUserId = applicationUserId,
            Address = new Address()
            { 
                StreetAddress = externalloginViewModel.StreetAddress,
                governorate = externalloginViewModel.governorate,
                City = externalloginViewModel.City
            }
        };
        return newPatient;
    }
    public void GetReactivateAccountViewModel(ApplicationUser user, Doctor doctor, ReactivateAccountViewModel reactivateAccountViewModel)
    {
        reactivateAccountViewModel.ApplicationUserId = user.Id;
        reactivateAccountViewModel.Email = user.Email;
        reactivateAccountViewModel.Username = user.UserName;
        reactivateAccountViewModel.ProfilePic = doctor.ProfilePicture;
    }

}
