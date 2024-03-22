using AutoMapper;
using BusinessLogicLayer.Helpers;
using DataAccessLayerEF.Models;
using Etammen.ViewModels;

namespace Etammen.Mapping_Profiles
{
    public class DoctorProfile:Profile
    {
        public DoctorProfile()
        {
            CreateMap<Doctor, DoctorViewModel>()
                .ForMember(d=>d.FirstName, o=>o.MapFrom(s=>s.ApplicationUser.FirstName))
                .ForMember(d=>d.LastName, o=>o.MapFrom(s=>s.ApplicationUser.LastName))
                .ForMember(d=>d.Age, o=>o.MapFrom(s=>s.ApplicationUser.Age))
                .ReverseMap();



            CreateMap<Clinic,ClinicViewModel>()
                .ForMember(d=>d.StreetAddress, o=>o.MapFrom(s=>s.Address.StreetAddress))
                .ForMember(d=>d.City, o=>o.MapFrom(s=>s.Address.City))
                .ForMember(d=>d.governorate, o=>o.MapFrom(s=>s.Address.governorate))
                .ReverseMap();
            CreateMap<ClinicAppointment, AppointmentViewModel>().
                ForMember(D => D.ClinicName, O => O.MapFrom(s => s.Clinic.Name))
                .ForMember(d => d.DoctorFirstName, o => o.MapFrom(s => s.Clinic.Doctor.ApplicationUser.FirstName))
                .ForMember(d => d.DoctorLastName, o => o.MapFrom(s => s.Clinic.Doctor.ApplicationUser.LastName))
                .ReverseMap();
            CreateMap<MainViewModel, DoctorFilterOptions>().ReverseMap();
            CreateMap<HomeAppointment, AppointmentViewModel>()
                .ForMember(d => d.DoctorFirstName, o => o.MapFrom(s => s.Doctor.ApplicationUser.FirstName))
                .ForMember(d => d.DoctorLastName, o => o.MapFrom(s => s.Doctor.ApplicationUser.LastName))
                .ReverseMap();

        }
    }
}
