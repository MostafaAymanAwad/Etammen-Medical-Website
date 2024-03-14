using AutoMapper;
using DataAccessLayerEF.Models;
using Etammen.ViewModels;

namespace Etammen.MappingProfile
{
	public class DoctorWithNameProfile:Profile
	{
        public DoctorWithNameProfile()
        {
			CreateMap<Doctor, DoctorWithNameViewModel>()
			.ForMember(d=>d.FirstName, o => o.MapFrom(s => s.ApplicationUser.FirstName))
			.ForMember(d => d.LastName, o => o.MapFrom(s => s.ApplicationUser.LastName))
			.ReverseMap();
		}
    }
}
