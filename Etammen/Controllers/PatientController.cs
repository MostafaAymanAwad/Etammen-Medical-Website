using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Etammen.ViewModels;
using AutoMapper;


namespace Etammen.Controllers
{
    [AllowAnonymous]
    public class PatientController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
		public PatientController(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		public IActionResult Search()
        {
            return View(new SerachViewModel());
        }
		public async Task<IActionResult> Index(string specialty, string city, string area,string doctorName,string clinicName)
		{
			 var searchedDoctors= await _unitOfWork.Doctors.Search(specialty, city, area, doctorName, clinicName);
			 var mappedDoctors=_mapper.Map<IEnumerable<Doctor>, IEnumerable<DoctorWithNameViewModel>>(searchedDoctors);
			 return View(mappedDoctors);
		}
	}
}
