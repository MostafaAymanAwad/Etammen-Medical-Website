using AutoMapper;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Repositories;
using DataAccessLayerEF.Models;
using DataAccessLayerEF.SettingsModel;
using Etammen.Helpers;
using Etammen.Mapping;
using Etammen.Mapping.DoctorForAdmin;
using Etammen.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tsp;
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;


namespace Etammen.Controllers;
[Route("")]

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
    private readonly ClinicDetailsMapViewModelMapper _clinicMapMapper;
    private readonly ISmsService _smsService;
    List<AppointmentViewModel> totalAppointments;
    private readonly UserManager<ApplicationUser> _userManager;

    public PatientController(IUnitOfWork unitOfWork,
        DoctorsAdminMapper getAllDoctorsMapper,
        IPatientRepository patientRepository,
        IDoctorReviewsRepository doctorReviewsRepository,
        IAppointmentRepository appointmentRepository,
        IApplicationUser applicationUser,
        ClinicDetailsMapViewModelMapper clinicMapMapper,
                    ISmsService smsService

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
        _clinicMapMapper = clinicMapMapper;
        _smsService = smsService;
        totalAppointments = new List<AppointmentViewModel>();

    }
    [Route("")]

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
    [Route("Index")]

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


    [Route("Pagination")]
    public async Task<IActionResult> Pagination(JSONMainViewModelHolder jSONMainViewModelHolder , int pageNumber = 1, int pageSize = 2)
    {
           var mainViewModel = JsonSerializer.Deserialize<MainViewModel>(jSONMainViewModelHolder.JSONdata);

            populateViewModel(mainViewModel);
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

	        ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

            return View("Index",jSONMainViewModelHolder);
        
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

    [Route("Filter")]

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


    [Route("Order")]

    public async Task<IActionResult> Order(JSONMainViewModelHolder jSONMainViewModelHolder)
    {
        var mainViewModel = JsonSerializer.Deserialize<MainViewModel>(jSONMainViewModelHolder.JSONdata);

        mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.OrderByOption(mainViewModel.Order,
            mainViewModel.FilteredOrderedDoctors);

        return await Pagination(jSONMainViewModelHolder);
    }

    [Route("DoctorReviews")]
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

    [Authorize(Roles = "Patient,Admin")]

    [Route("DoctorDetails")]

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

        public async Task<IActionResult> ClinicDetails(int id)
        {
            var clinic = await _clinicRepository.GetClinics(id);
            var clinicMapVm = _clinicMapMapper.ClinicMapper(clinic);

            return View(clinicMapVm);
        }

        public async Task<IActionResult> Profile(int id = 1)
        {
            string[] includes = { "ClinicAppointments", "DoctorReviews", "ApplicationUser" };

            var patients = await _unitOfWork.Patients.FindBy(d => d.Id == id, includes);
            var mappedpatient = _mapper.Map<Patient, PatientViewModel>(patients);
            return View(mappedpatient);
        }

        public async Task<IActionResult> ProfileEdit(int id)
        {
            string[] includes = { "ClinicAppointments", "DoctorReviews", "ApplicationUser" };

            var Patient = await _unitOfWork.Patients.FindBy(d => d.Id == id, includes);
            if (Patient == null)
            {
                return NotFound();
            }
            var mappedPatient = _mapper.Map<Patient, PatientViewModel>(Patient);

            return View(mappedPatient);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileEdit(int id, PatientViewModel model)
        {

            if (ModelState.IsValid)
            {

                var existinuser = await _userManager.FindByIdAsync(model.ApplicationUserId);
                if (existinuser is not null)
                {
                    existinuser.FirstName = model.FirstName;
                    existinuser.LastName = model.LastName;
                    existinuser.Age = model.Age;
                    existinuser.UserName = model.UserName;
                    existinuser.PhoneNumber = model.PhoneNumber;
                    existinuser.Email = model.Email;

                    var result = await _userManager.UpdateAsync(existinuser);

                    if (result.Succeeded)
                    {

                        //_unitOfWork.Doctors.Update(mappeddoctor);
                        string[] includes = { "ClinicAppointments", "DoctorReviews", "ApplicationUser" };

                        var existingPatient = await _unitOfWork.Patients.FindBy(d => d.Id == model.Id, includes);
                        if (existingPatient == null)
                        {
                            return NotFound();
                        }
                        //var mappedpatient = _mapper.Map<PatientViewModel,Patient>(model);
                        //_unitOfWork.Patients.Update(mappedpatient);
                        existingPatient.Id = model.Id;
                        existingPatient.Address.StreetAddress = model.Address.StreetAddress;
                        existingPatient.Address.City = model.Address.City;
                        existingPatient.Address.governorate = model.Address.governorate;



                        await _unitOfWork.Commit();
                        return RedirectToAction(nameof(Profile));
                    }
                    return RedirectToAction(nameof(Profile));
                }
            }

            return View(model);
        }
        private void populateViewModel(MainViewModel mainViewModel)
        {
            mainViewModel.City_areaDict = _doctorRegisterationHelper.CityAreasDictionary;
            mainViewModel.Specialties = _doctorRegisterationHelper.SpecialitySelectList;
        }
        //public async Task<IActionResult> DoctorIndex()
        //{
        //    string[] includes = { "Clinics", "DoctorReviews", "ApplicationUser" };
        //    var doctors = await _unitOfWork.Doctors.GetAll(includes);
        //    var mappedDoctors = _mapper.Map<IEnumerable<Doctor>, IEnumerable<DoctorViewModel>>(doctors);
        //    return View(mappedDoctors);
        //}
        //public async Task<IActionResult> ClinicIndex(int id)
        //{
        //    ViewBag.id = id;
        //    var includes = new Dictionary<Expression<Func<Clinic, object>>, Expression<Func<object, object>>>();
        //    includes.Add(c => c.Doctor, d => ((Doctor)d).ApplicationUser);
        //    var clinicList = await _unitOfWork.Clinics.GetAllWithExpression(includes, c => c.IsDeleted == false && c.DoctorId == id);

        //    var mappedClinics = _mapper.Map<IEnumerable<Clinic>, IEnumerable<ClinicViewModel>>(clinicList);
        //    return View(mappedClinics);
        //}
        public async Task<IActionResult> Book(int id, int doctorId)
        {
            string[] includes = { "Doctor", "ClinicAppointments" };
            ViewBag.patientID = 1;
            ViewBag.DoctorID = doctorId;
            var clinic = await _unitOfWork.Clinics.FindBy(d => d.Id == id, includes);
            if (clinic == null)
            {
                return NotFound();
            }
            var mappedClinic = _mapper.Map<Clinic, BookViewModel>(clinic);
            mappedClinic.Clinic = clinic;
            int indexForClinicNameInviewData = 0;

            
                TimeSpan openingHour = clinic.OpeningHour.ToTimeSpan();
                TimeSpan closingHour = clinic.ClosingHour.ToTimeSpan();
                TimeSpan examinationDuration = clinic.ExmainationDuration.ToTimeSpan();


                TimeSpan clinicDuration = closingHour - openingHour;
                var appointmentlist = new List<TimeOnly?>();
                int examinationPeriods = (int)(clinicDuration.TotalMinutes / examinationDuration.TotalMinutes);
                ViewData[$"clinic{indexForClinicNameInviewData++}"] = examinationPeriods;

                foreach (var appointment in clinic.ClinicAppointments)
                {
                    if (appointment.ReservationPeriodNumber is not null && appointment.Date == DateOnly.FromDateTime(DateTime.Now))
                    {

                        appointmentlist.Add(appointment.ReservationPeriodNumber);
                    }
                }
                mappedClinic.ClinicAppointmentDictionary.Add(clinic.Id, appointmentlist);
            
            return View(mappedClinic);
        }
        [HttpPost]
        public async Task<IActionResult> BookConfirmed(BookViewModel book)
        {
            if (ModelState.IsValid)
            {
                bool IsHomeExisted = await _patientRepository.AnyHomeVisit(book.patientId, book.DoctorId, book.Date,book.ISHomeAppointmentDeleted,book.IsAttended);
                bool IsClinicExisted = await _patientRepository.AnyAppointment(book.patientId, book.ClinicId, book.Date, book.ISClinicAppointmentDeleted, book.IsAttended);
                if (book.ClinicId is null)
                {
                    if (!IsHomeExisted)
                    {
                        var appointmentbooked = _mapper.Map<BookViewModel, HomeAppointment>(book);
                        await _unitOfWork.HomeAppointment.Add(appointmentbooked);
                    }
                    else { 
                    TempData["BookMessage"] = $"You already booked for today, See your appointment here";
                    return RedirectToAction(nameof(AppointmentIndex));
                    }
                }
                else
                {
                    if (!IsClinicExisted)
                    {
                        var appointmentbooked = _mapper.Map<BookViewModel, ClinicAppointment>(book);
                    appointmentbooked.ReservationPeriodNumber = book.ReservtionPeriodNumber;
                    appointmentbooked.Clinic = null;
                        await _unitOfWork.ClinicAppointments.Add(appointmentbooked);
                    }
                    else
                    {
                        TempData["BookMessage"] = $"You already booked for today, See your appointment here";
                        return RedirectToAction(nameof(AppointmentIndex));
                    }
                }
                var count = await _unitOfWork.Commit();
                if (count > 0 && book.ClinicId is null)
                    TempData["BookMessage"] = $"Home Appointment Was booked succssfully";
                else
                    TempData["BookMessage"] = $"Clinic Appointment Was booked succssfully";

                if (book.IsPaidOnline==true&& book.ClinicId is null)
                    return RedirectToAction("CheckoutSession","Payment", new {fees = book.HomeVisitFees,name = "Home Visit Appointment"});
                else if (book.IsPaidOnline == true && book.ClinicId is not null)
                    return RedirectToAction("CheckoutSession", "Payment", new { fees = book.HomeVisitFees, name = book.ClinicName });

                return RedirectToAction(nameof(AppointmentIndex));
            }
            return View(nameof(Book), book);
        } 
        public async Task<IActionResult> AppointmentIndex(int id = 1)
            {

            var includes = new Dictionary<Expression<Func<ClinicAppointment, object>>, List<Expression<Func<object, object>>>>()
            {
               {
                     a => a.Clinic,
                    new List<Expression<Func<object, object>>>
                    {
                           c => ((Clinic)c).Doctor,
                           d => ((Doctor)d).ApplicationUser
                    }
               },
               {
                     a => a.Patient,
                    new List<Expression<Func<object, object>>>
                    {
                           d => ((Patient)d).ApplicationUser
                    }
               },
            };
            var Homeincludes = new Dictionary<Expression<Func<HomeAppointment, object>>, List<Expression<Func<object, object>>>>()
            {
               {
                    c => c.Doctor,
                    new List<Expression<Func<object, object>>>
                    {

                           d => ((Doctor)d).ApplicationUser
                    }
               },
               {
                     a => a.Patient,
                    new List<Expression<Func<object, object>>>
                    {
                           d => ((Patient)d).ApplicationUser
                    }
               },
            };
            var appointment = await _unitOfWork.ClinicAppointments.FindByWithTwoThenIncludes(a => a.IsDeleted == false && a.patientId == id, includes);
            var homeappointment = await _unitOfWork.HomeAppointment.FindByWithTwoThenIncludes(a => a.IsDeleted == false && a.PatientId == id, Homeincludes);
            var mappedappointmwnts = _mapper.Map<IEnumerable<ClinicAppointment>, IEnumerable<AppointmentViewModel>>(appointment);
            totalAppointments.AddRange(mappedappointmwnts);
            mappedappointmwnts = _mapper.Map<IEnumerable<HomeAppointment>, IEnumerable<AppointmentViewModel>>(homeappointment);
            totalAppointments.AddRange(mappedappointmwnts);

            return View(totalAppointments);

        }
        public async Task<IActionResult> CancelAppointment(int id, TimeOnly? ReservationPeriodNumber)
        {

            var includes = new Dictionary<Expression<Func<ClinicAppointment, object>>, List<Expression<Func<object, object>>>>()
            {
                 {
                     a => a.Clinic,
                    new List<Expression<Func<object, object>>>
                     {
                           c => ((Clinic)c).Doctor,
                           d => ((Doctor)d).ApplicationUser
                      }
                 },
                    {
                     a => a.Patient,
                    new List<Expression<Func<object, object>>>
                     {
                           d => ((Patient)d).ApplicationUser
                      }
                    },
            };
            var Homeincludes = new Dictionary<Expression<Func<HomeAppointment, object>>, List<Expression<Func<object, object>>>>()
            {
               {
                    c => c.Doctor,
                    new List<Expression<Func<object, object>>>
                    {

                           d => ((Doctor)d).ApplicationUser
                    }
               },
               {
                     a => a.Patient,
                    new List<Expression<Func<object, object>>>
                    {
                           d => ((Patient)d).ApplicationUser
                    }
               },
            };

            var appointment = await _unitOfWork.ClinicAppointments.FindByWithExpression(a => a.Id == id, includes);
            var homeappointment = await _unitOfWork.HomeAppointment.FindByWithExpression(a => a.Id == id, Homeincludes);
            var mappedClinicAppointmwnts = _mapper.Map<ClinicAppointment, AppointmentViewModel>(appointment);
            var mappedHomeAppointmwnts = _mapper.Map<HomeAppointment, AppointmentViewModel>(homeappointment);

            if (ReservationPeriodNumber is null)
            {
                return View(mappedHomeAppointmwnts);
            }
            else
            {
                return View(mappedClinicAppointmwnts);
            }
        }
        [HttpPost, ActionName("CancelAppointment")]
        public async Task<IActionResult> CancelAppointmentConfirmed(AppointmentViewModel model)
        {
            var includes = new Dictionary<Expression<Func<ClinicAppointment, object>>, List<Expression<Func<object, object>>>>()
            {
                 {
                     a => a.Clinic,
                    new List<Expression<Func<object, object>>>
                     {
                           c => ((Clinic)c).Doctor,
                           d => ((Doctor)d).ApplicationUser
                      }
                 },
                    {
                     a => a.Patient,
                    new List<Expression<Func<object, object>>>
                     {
                           d => ((Patient)d).ApplicationUser
                      }
                    },
            };
            var Homeincludes = new Dictionary<Expression<Func<HomeAppointment, object>>, List<Expression<Func<object, object>>>>()
            {
               {
                    c => c.Doctor,
                    new List<Expression<Func<object, object>>>
                    {

                           d => ((Doctor)d).ApplicationUser
                    }
               },
               {
                     a => a.Patient,
                    new List<Expression<Func<object, object>>>
                    {
                           d => ((Patient)d).ApplicationUser
                    }
               },
            };

            var appointment = await _unitOfWork.ClinicAppointments.FindByWithExpression(a => a.Id == model.Id, includes);
            var homeappointment = await _unitOfWork.HomeAppointment.FindByWithExpression(a => a.Id == model.Id, Homeincludes);


            //var PatientFullName = $"{appointment.Patient.ApplicationUser.FirstName} {appointment.Patient.ApplicationUser.LastName}";
            //var smsmessage = new SMSMessage()
            //{
            //    PhoneNumber = appointment.Patient.ApplicationUser.PhoneNumber,
            //    body = $"Dear Mr {PatientFullName} : your appointment was canceld successfully"
            //};

            if (model.ReservationPeriodNumber is null&& homeappointment is not null)
            {
                homeappointment.IsDeleted = true;
                _unitOfWork.HomeAppointment.Update(homeappointment);
            }
            else if(model.ReservationPeriodNumber is not null && appointment is not null)
            {
                appointment.IsDeleted = true;
                _unitOfWork.ClinicAppointments.Update(appointment);
            }
            await _unitOfWork.Commit();

            //var messageSent = _smsService.Send(smsmessage);
            //if (messageSent is not null)
            //    TempData["MessageWasSent"] = $"{PatientFullName} You canceled your appointment ";
            return RedirectToAction(nameof(AppointmentIndex));

        }
    }
}
