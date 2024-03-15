using AutoMapper;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Models;
using Etammen.Helpers;
using Etammen.Mapping.DoctorForAdmin;
using Etammen.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Etammen.Controllers
{
    public class PatientController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPatientRepository _patientRepository;
        private readonly DoctorsAdminMapper _doctorsMapper;
        private readonly DoctorRegisterationHelper _doctorRegisterationHelper;
        public PatientController(IUnitOfWork unitOfWork, DoctorsAdminMapper getAllDoctorsMapper, IPatientRepository patientRepository, DoctorRegisterationHelper doctorRegisterationHelper)
        {
            _unitOfWork = unitOfWork;
            _doctorsMapper = getAllDoctorsMapper;
            _patientRepository = patientRepository;
            _doctorRegisterationHelper = doctorRegisterationHelper;
        }
        public IActionResult Search()
        {
            var vm = new SerachViewModel();
            vm.city_areaDict = _doctorRegisterationHelper.CityAreasDictionary;
            vm.Specialties = _doctorRegisterationHelper.SpecialitySelectList;
            return View(vm);
        }
		public async Task<IActionResult> Index(MainViewModel mainViewModel)
		{
			 var searchedDoctors= await _unitOfWork.Doctors.Search(mainViewModel.specialty, mainViewModel.city,
                 mainViewModel.area, mainViewModel.doctorName, mainViewModel.clinicName);

            mainViewModel.SearchedDoctors = searchedDoctors.ToList();

            DoctorFilterOptions filterOptions = _mapper.Map<MainViewModel,DoctorFilterOptions>(mainViewModel);
            mainViewModel.FilteredOrderedDoctors = await _unitOfWork.Doctors.FilterByOptions(filterOptions,
                mainViewModel.SearchedDoctors);
            mainViewModel.FilteredOrderedDoctors =  _unitOfWork.Doctors.OrderByOption(mainViewModel.Order,
                mainViewModel.FilteredOrderedDoctors);

            return RedirectToAction("Pagination", mainViewModel);
		}
        public IActionResult Pagination(MainViewModel mainViewModel, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var numberOfRows = mainViewModel.FilteredOrderedDoctors.Count;
                var totalPages = (int)Math.Ceiling((double)numberOfRows / pageSize);

                if (pageNumber < 1 || pageNumber > totalPages)
                {
                    return RedirectToAction("Index", new {mainViewModel ,pageNumber = totalPages, pageSize });
                }

                var doctors =  _patientRepository.PatientsPaginationNextAsync(mainViewModel.FilteredOrderedDoctors,pageNumber, pageSize);
                var viewModel = _doctorsMapper.MapDoctorEntitiesToDoctorViewModel(doctors);

                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = totalPages;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log exception
                throw; // Rethrow the exception for further investigation
            }
        }
        public async  Task<IActionResult> Filter(MainViewModel mainViewModel)
        {
            DoctorFilterOptions filterOptions = _mapper.Map<MainViewModel, DoctorFilterOptions>(mainViewModel);
            mainViewModel.FilteredOrderedDoctors = await _unitOfWork.Doctors.FilterByOptions(filterOptions,
                mainViewModel.SearchedDoctors);
            mainViewModel.FilteredOrderedDoctors =  _unitOfWork.Doctors.OrderByOption(mainViewModel.Order,
                mainViewModel.FilteredOrderedDoctors);
            return RedirectToAction("Pagination", mainViewModel);
        }
        public async Task<IActionResult> Order(MainViewModel mainViewModel)
        {
            mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.OrderByOption(mainViewModel.Order,
                mainViewModel.FilteredOrderedDoctors);
            return RedirectToAction("Pagination", mainViewModel);
        }
    }

}
