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
using System.Text.Json;

namespace Etammen.Controllers
{
    public class PatientController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPatientRepository _patientRepository;
        private readonly DoctorsAdminMapper _doctorsMapper;
        private readonly DoctorRegisterationHelper _doctorRegisterationHelper;
        public PatientController(IUnitOfWork unitOfWork, DoctorsAdminMapper getAllDoctorsMapper, IPatientRepository patientRepository, DoctorRegisterationHelper doctorRegisterationHelper, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _doctorsMapper = getAllDoctorsMapper;
            _patientRepository = patientRepository;
            _doctorRegisterationHelper = doctorRegisterationHelper;
            _mapper = mapper;
        }
        public async Task<IActionResult> Search(JSONMainViewModelHolder jSONMainViewModelHolder)
        {
            MainViewModel mainViewModel = new();
            if(jSONMainViewModelHolder.JSONdata != null)
            {
			    mainViewModel = JsonSerializer.Deserialize<MainViewModel>(jSONMainViewModelHolder.JSONdata);
            }
			populateViewModel(mainViewModel);

            if (mainViewModel.SearchedDoctors == null)
                return View((JSONMainViewModelHolder)new() { JSONdata = JsonSerializer.Serialize(mainViewModel) });
            else
                return await Index(new() { JSONdata = JsonSerializer.Serialize(mainViewModel)});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(JSONMainViewModelHolder jSONMainViewModelHolder)
		{
			var mainViewModel = JsonSerializer.Deserialize<MainViewModel>(jSONMainViewModelHolder.JSONdata);
			populateViewModel(mainViewModel);
            var searchedDoctors= await _unitOfWork.Doctors.Search(mainViewModel.specialty, mainViewModel.city,
                 mainViewModel.area, mainViewModel.doctorName, mainViewModel.clinicName);

            mainViewModel.SearchedDoctors = searchedDoctors.ToList();
            DoctorFilterOptions filterOptions = _mapper.Map<DoctorFilterOptions>(mainViewModel);
            mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.FilterByOptions(filterOptions,
                mainViewModel.SearchedDoctors);
            mainViewModel.FilteredOrderedDoctors =  _unitOfWork.Doctors.OrderByOption(mainViewModel.Order,
                mainViewModel.FilteredOrderedDoctors);

            jSONMainViewModelHolder = new JSONMainViewModelHolder();
            jSONMainViewModelHolder.JSONdata = JsonSerializer.Serialize(mainViewModel);

            return Pagination(jSONMainViewModelHolder);
		}
        public IActionResult Pagination(JSONMainViewModelHolder jSONMainViewModelHolder , int pageNumber = 1, int pageSize = 2)
        {
            var mainViewModel = JsonSerializer.Deserialize<MainViewModel>(jSONMainViewModelHolder.JSONdata);

            populateViewModel(mainViewModel);
            try
            {
                var numberOfRows = mainViewModel.FilteredOrderedDoctors.Count;
                var totalPages = (int)Math.Ceiling((double)numberOfRows / pageSize);
                ViewBag.CurrentPageDoctors =  _patientRepository.PatientsPaginationNextAsync(mainViewModel.FilteredOrderedDoctors,pageNumber, pageSize);

                if (totalPages == 0 || pageNumber <= 0)
                {
                    return View("Index", jSONMainViewModelHolder);
                }
                else if (pageNumber > totalPages)
                {
                    return Pagination(jSONMainViewModelHolder, totalPages, pageSize);
                }

				jSONMainViewModelHolder.JSONdata = JsonSerializer.Serialize(mainViewModel);
				//var viewModel = _doctorsMapper.MapDoctorEntitiesToDoctorViewModel(doctors);

				ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = totalPages;

                return View("Index",jSONMainViewModelHolder);
            }
            catch (Exception ex)
            {
                // Log exception
                throw; // Rethrow the exception for further investigation
            }
        }
        public async Task<IActionResult> Filter(MainViewModel mainViewModel)
        {
            DoctorFilterOptions filterOptions = _mapper.Map<MainViewModel, DoctorFilterOptions>(mainViewModel);
            mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.FilterByOptions(filterOptions,
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
        private void populateViewModel(MainViewModel mainViewModel)
        {
            mainViewModel.city_areaDict = _doctorRegisterationHelper.CityAreasDictionary;
            mainViewModel.Specialties = _doctorRegisterationHelper.SpecialitySelectList;
        }
    }
}
