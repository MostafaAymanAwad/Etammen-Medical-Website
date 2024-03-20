using AutoMapper;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Repositories;
using DataAccessLayerEF.Models;
using Etammen.Helpers;
using Etammen.Mapping;
using Etammen.Mapping.DoctorForAdmin;
using Etammen.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly IClinicRepository _clinicRepository;
        private readonly IApplicationUser _applicationUser;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorReviewsRepository _doctorReviewsRepository;
        private readonly DoctorsAdminMapper _doctorsMapper;
        private readonly DoctorReviewMapping _doctorReviewMapper;
        private readonly DoctorDetailsMapping _doctorDetailsMapping;
        private readonly ClinicDetailsForDoctorPageMapper _clinicMapper;
        private readonly DoctorRegisterationHelper _doctorRegisterationHelper;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientController(IUnitOfWork unitOfWork,
            DoctorsAdminMapper getAllDoctorsMapper,
            IPatientRepository patientRepository,
            IDoctorReviewsRepository doctorReviewsRepository,
            IAppointmentRepository appointmentRepository,
            IApplicationUser applicationUser,
            IClinicRepository clinicRepository,
            DoctorRegisterationHelper doctorRegisterationHelper,
            IMapper mapper,
            ClinicDetailsForDoctorPageMapper clinicMapper,
            DoctorDetailsMapping doctorDetailsMapping,
            DoctorReviewMapping doctorReviewMapper,
            UserManager<ApplicationUser> userManager
            )
        {
            _unitOfWork = unitOfWork;
            _doctorsMapper = getAllDoctorsMapper;
            _patientRepository = patientRepository;
            _doctorRegisterationHelper = doctorRegisterationHelper;
            _mapper = mapper;
            _userManager = userManager;
            _clinicMapper = clinicMapper;
            _applicationUser = applicationUser;
            _appointmentRepository = appointmentRepository;
            _clinicRepository = clinicRepository;
            _doctorDetailsMapping = doctorDetailsMapping;
            _doctorReviewMapper = doctorReviewMapper;
            _doctorReviewsRepository = doctorReviewsRepository;

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
                return await Index( new() { JSONdata = JsonSerializer.Serialize(mainViewModel)});
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(JSONMainViewModelHolder jSONMainViewModelHolder)
		{
			var mainViewModel = JsonSerializer.Deserialize<MainViewModel>(jSONMainViewModelHolder.JSONdata);
			populateViewModel(mainViewModel);
            var searchedDoctors= await _unitOfWork.Doctors.Search(mainViewModel.Specialty, mainViewModel.City,
                 mainViewModel.Area, mainViewModel.DoctorName, mainViewModel.ClinicName);

            mainViewModel.SearchedDoctors = searchedDoctors.ToList();

            DoctorFilterOptions filterOptions = _mapper.Map<DoctorFilterOptions>(mainViewModel);
            mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.FilterByOptions(filterOptions,
                mainViewModel.SearchedDoctors);

            mainViewModel.FilteredOrderedDoctors =  _unitOfWork.Doctors.OrderByOption(mainViewModel.Order,
                mainViewModel.FilteredOrderedDoctors);


            jSONMainViewModelHolder = new JSONMainViewModelHolder();
            jSONMainViewModelHolder.JSONdata = JsonSerializer.Serialize(mainViewModel);

            return await Pagination(jSONMainViewModelHolder);
		}
        public async Task<IActionResult> Pagination(JSONMainViewModelHolder jSONMainViewModelHolder , int pageNumber = 1, int pageSize = 2)
        {
            var mainViewModel = JsonSerializer.Deserialize<MainViewModel>(jSONMainViewModelHolder.JSONdata);

            populateViewModel(mainViewModel);
            try
            {
                var numberOfRows = mainViewModel.FilteredOrderedDoctors.Count;
                var totalPages = (int)Math.Ceiling((double)numberOfRows / pageSize);
                mainViewModel.CurrentPageDoctors =  _patientRepository.PatientsPaginationNextAsync(mainViewModel.FilteredOrderedDoctors,pageNumber, pageSize);

                mainViewModel.DoctorFullnames = await populateViewModel(mainViewModel.CurrentPageDoctors);
                if (totalPages == 0 || pageNumber <= 0)
                {
                    return View("Index", jSONMainViewModelHolder);
                }
                else if (pageNumber > totalPages)
                {
                    return await Pagination(jSONMainViewModelHolder, totalPages, pageSize);
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
        private async Task<List<string>> populateViewModel(List<Doctor>doctors)
        {
            List<string> Fullnames = new();
            foreach(var doctor in doctors)
            {
                ApplicationUser Appuser = await _userManager.FindByIdAsync(doctor.ApplicationUserId);
                string fullname = string.Join(" ", Appuser.FirstName, Appuser.LastName);
                Fullnames.Add(fullname);
            }
            return Fullnames;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Filter(JSONMainViewModelHolder jSONMainViewModelHolder)
        {

            var mainViewModel = JsonSerializer.Deserialize<MainViewModel>(jSONMainViewModelHolder.JSONdata);

            DoctorFilterOptions filterOptions = _mapper.Map<MainViewModel, DoctorFilterOptions>(mainViewModel);

            mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.FilterByOptions(filterOptions,
                mainViewModel.SearchedDoctors);
            mainViewModel.FilteredOrderedDoctors =  _unitOfWork.Doctors.OrderByOption(mainViewModel.Order,
                mainViewModel.FilteredOrderedDoctors);

            jSONMainViewModelHolder.JSONdata = JsonSerializer.Serialize(mainViewModel);

            return await Pagination(jSONMainViewModelHolder);
        }

        public async Task<IActionResult> Order(JSONMainViewModelHolder jSONMainViewModelHolder)
        {
            var mainViewModel = JsonSerializer.Deserialize<MainViewModel>(jSONMainViewModelHolder.JSONdata);

            mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.OrderByOption(mainViewModel.Order,
                mainViewModel.FilteredOrderedDoctors);

            return await Pagination(jSONMainViewModelHolder);
        }

        [HttpPost]
        public async Task<IActionResult> DoctorReviews(DoctorReviewViewModel doctorViewModel)
        {
            if (ModelState.IsValid)
            {
                //1->> Get the reviewed doctor 
                var doctor = await _unitOfWork.Doctors.FindBy(e => e.Id == doctorViewModel.DoctorId);

                //2->> Check of null
                if (doctor is not null)
                {
                    //3->> Mapping from doctorViewModel to Entity cuz EF methods only take Entities
                    var doctorReview = _doctorReviewMapper.MapFromViewModelToEntity(doctorViewModel);
                    //4->> Add Doctor Review to DB
                    await _unitOfWork.DoctorReviews.Add(doctorReview);
                    //5->> Save Changes
                    await _unitOfWork.Commit();

                    //6->> Now I need to update doctor's total rates and actual rate so:
                    //6-1>> I need to ge the Sum of rates >> I made a method in the repo to get that
                    var sumOfRates = _patientRepository.GetSumOfRates(doctorViewModel.DoctorId);
                    //6-2>> I need to ge the Number of rates >> I made a method in the repo to get that
                    //var numberOfRates = _patientRepository.NumberOfRates(doctorViewModel.DoctorId);

                    doctor.TotalRatings += 1;
                    decimal maxRating = 5M; // Maximum rating allowed

                    // Calculate the actual rating
                    decimal calculatedRating = ((decimal)sumOfRates / (decimal)(doctor.TotalRatings * maxRating));

                    // Ensure the rating does not exceed the maximum allowed rating
                    doctor.ActualRting = Math.Round(calculatedRating * maxRating, 1);
                    //doctor.ActualRting = Math.Min(maxRating, Math.Round(calculatedRating * maxRating, 1));

                    //10->> Update the doctor
                    _unitOfWork.Doctors.Update(doctor);
                    await _unitOfWork.Commit();
                }
                else
                {
                    return NotFound();
                }
            }
            return View(doctorViewModel);
        }
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _patientRepository.GetDoctorDetails(id);

            var doctorDetailsViewModel = _doctorDetailsMapping.MapToDoctorDetails(doctor);
            doctorDetailsViewModel.PatientId = 5;
            var clinicDetails = _clinicRepository.GetClinicsNames(id);
            doctorDetailsViewModel.Clinics = _clinicMapper.MapToClinicDetailsInDoctorPageViewModel(clinicDetails);
            doctorDetailsViewModel.FirstName = _applicationUser.FirstName(id);
            doctorDetailsViewModel.LastName = _applicationUser.LastName(id);
            doctorDetailsViewModel.IsAttended = _appointmentRepository.IsAppointmentsAvailable(5);
            doctorDetailsViewModel.IsReview = _doctorReviewsRepository.IsReviewdBy(id, 5);
            return View(doctorDetailsViewModel);
        }
        private void populateViewModel(MainViewModel mainViewModel)
        {
            mainViewModel.City_areaDict = _doctorRegisterationHelper.CityAreasDictionary;
            mainViewModel.Specialties = _doctorRegisterationHelper.SpecialitySelectList;
        }

    }
}
