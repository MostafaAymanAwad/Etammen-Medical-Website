using AutoMapper;
using DataAccessLayerEF.Models;
using Etammen.ViewModels;

namespace Etammen.Mapping_Profiles
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, PatientViewModel>()
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.ApplicationUser != null ? s.ApplicationUser.FirstName : ""))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.ApplicationUser != null ? s.ApplicationUser.LastName : ""))
                .ForMember(d => d.Age, o => o.MapFrom(s => s.ApplicationUser != null ? s.ApplicationUser.Age : 0))
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.ApplicationUser != null ? s.ApplicationUser.UserName : ""))
                .ForMember(d => d.Email, o => o.MapFrom(s => s.ApplicationUser != null ? s.ApplicationUser.Email : ""))
                .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.ApplicationUser != null ? s.ApplicationUser.PhoneNumber : ""))
                .ReverseMap();
        }
    }
}
