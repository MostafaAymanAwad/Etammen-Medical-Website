using AutoMapper;
using DataAccessLayerEF.Models;
using Etammen.ViewModels.Admin.Doctor;

namespace Etammen.Mapping.DoctorForAdmin
{
    public class ApproveDoctorMappingProfile : Profile
    {
        public ApproveDoctorMappingProfile()
        {
            CreateMap<Doctor, ApproveDoctorViewModel>()
            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.ApplicationUser.FirstName))
            .ForMember(d => d.LastName, o => o.MapFrom(s => s.ApplicationUser.LastName))
            .ReverseMap();
        }
    }
}
