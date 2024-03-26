using AutoMapper;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Repositories;
using BusinessLogicLayer.Services.SMS;
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
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Tsp;
using Stripe;
using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Messaging;
using Twilio.Types;

namespace Etammen.Controllers;
[Route("")]
public class PatientController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorDetailsRepository _doctorDetailsRepository;
    private readonly DoctorsAdminMapper _doctorsMapper;
    private readonly DoctorReviewMapping _doctorReviewMapper;
    private readonly DoctorRegisterationHelper _doctorRegisterationHelper;
    private readonly ISmsService _smsService;
    List<AppointmentViewModel> totalAppointments;
    private readonly UserManager<ApplicationUser> _userManager;

    public PatientController(IUnitOfWork unitOfWork,
        DoctorsAdminMapper getAllDoctorsMapper,
        IPatientRepository patientRepository,
        ISmsService smsService,
        DoctorRegisterationHelper doctorRegisterationHelper,
        IMapper mapper,
        DoctorReviewMapping doctorReviewMapper,
        UserManager<ApplicationUser> userManager,
        IDoctorDetailsRepository doctorDetailsRepository
        )
    {
        _unitOfWork = unitOfWork;
        _doctorsMapper = getAllDoctorsMapper;
        _patientRepository = patientRepository;
        _doctorRegisterationHelper = doctorRegisterationHelper;
        _mapper = mapper;
        _userManager = userManager;
        _doctorReviewMapper = doctorReviewMapper;
        _smsService = smsService;
        totalAppointments = new List<AppointmentViewModel>();
        _doctorDetailsRepository = doctorDetailsRepository;

    }
    [HttpGet("")]
    public async Task<IActionResult> Search(JSONMainViewModelHolder jSONMainViewModelHolder)
    {
        MainViewModel mainViewModel = new();
        if (jSONMainViewModelHolder.JSONdata != null)
        {
            mainViewModel = JsonSerializer.Deserialize<MainViewModel>(jSONMainViewModelHolder.JSONdata);
        }
        populateViewModel(mainViewModel);

        if (mainViewModel.SearchedDoctors == null)
            return View((JSONMainViewModelHolder)new() { JSONdata = JsonSerializer.Serialize(mainViewModel) });
        else
            return await Index(new() { JSONdata = JsonSerializer.Serialize(mainViewModel) });
    }

    [HttpPost, Route("alldoctors")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(JSONMainViewModelHolder jSONMainViewModelHolder)
    {
        var mainViewModel = JsonSerializer.Deserialize<MainViewModel>(jSONMainViewModelHolder.JSONdata);
        populateViewModel(mainViewModel);
        var searchedDoctors = await _unitOfWork.Doctors.Search(mainViewModel.Specialty, mainViewModel.City,
             mainViewModel.Area, mainViewModel.DoctorName, mainViewModel.ClinicName);

        mainViewModel.SearchedDoctors = searchedDoctors.ToList();

        DoctorFilterOptions filterOptions = _mapper.Map<DoctorFilterOptions>(mainViewModel);
        mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.FilterByOptions(filterOptions,
            mainViewModel.SearchedDoctors);

        mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.OrderByOption(mainViewModel.Order,
            mainViewModel.FilteredOrderedDoctors);


        jSONMainViewModelHolder = new JSONMainViewModelHolder();
        jSONMainViewModelHolder.JSONdata = JsonSerializer.Serialize(mainViewModel);

        return await Pagination(jSONMainViewModelHolder);
    }


    [Route("searchedDoctors")]
    public async Task<IActionResult> Pagination(JSONMainViewModelHolder jSONMainViewModelHolder, int pageNumber = 1, int pageSize = 5)
    {
        var mainViewModel = JsonSerializer.Deserialize<MainViewModel>(jSONMainViewModelHolder.JSONdata);

        populateViewModel(mainViewModel);
        var numberOfRows = mainViewModel.FilteredOrderedDoctors.Count;
        var totalPages = (int)Math.Ceiling((double)numberOfRows / pageSize);
        mainViewModel.CurrentPageDoctors = _patientRepository.PatientsPaginationNextAsync(mainViewModel.FilteredOrderedDoctors, pageNumber, pageSize);

        mainViewModel.DoctorFullnames = await populateViewModel(mainViewModel.CurrentPageDoctors);
        jSONMainViewModelHolder.JSONdata = JsonSerializer.Serialize(mainViewModel);
        if (totalPages == 0 || pageNumber <= 0)
        {
            return View("Index", jSONMainViewModelHolder);
        }
        else if (pageNumber > totalPages)
        {
            return await Pagination(jSONMainViewModelHolder, totalPages, pageSize);
        }
        ViewBag.CurrentPage = pageNumber;
        ViewBag.TotalPages = totalPages;

        return View("Index", jSONMainViewModelHolder);

    }
    private async Task<List<string>> populateViewModel(List<Doctor> doctors)
    {
        List<string> Fullnames = new();
        foreach (var doctor in doctors)
        {
            ApplicationUser Appuser = await _userManager.FindByIdAsync(doctor.ApplicationUserId);
            string fullname = string.Join(" ", Appuser.FirstName, Appuser.LastName);
            Fullnames.Add(fullname);
        }
        return Fullnames;
    }


    [HttpPost, Route("filteredDoctor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Filter(JSONMainViewModelHolder jSONMainViewModelHolder)
    {

        var mainViewModel = JsonSerializer.Deserialize<MainViewModel>(jSONMainViewModelHolder.JSONdata);

        DoctorFilterOptions filterOptions = _mapper.Map<MainViewModel, DoctorFilterOptions>(mainViewModel);

        mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.FilterByOptions(filterOptions,
            mainViewModel.SearchedDoctors);
        mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.OrderByOption(mainViewModel.Order,
            mainViewModel.FilteredOrderedDoctors);

        jSONMainViewModelHolder.JSONdata = JsonSerializer.Serialize(mainViewModel);

        return await Pagination(jSONMainViewModelHolder);
    }
    [Route("orderedDoctor")]
    public async Task<IActionResult> Order(JSONMainViewModelHolder jSONMainViewModelHolder)
    {
        var mainViewModel = JsonSerializer.Deserialize<MainViewModel>(jSONMainViewModelHolder.JSONdata);

        mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.OrderByOption(mainViewModel.Order,
            mainViewModel.FilteredOrderedDoctors);

        jSONMainViewModelHolder.JSONdata = JsonSerializer.Serialize(mainViewModel);
        return await Pagination(jSONMainViewModelHolder);
    }

    [HttpPost, Route("Reviews")]
    public async Task<IActionResult> DoctorReviews(DoctorReviewViewModel doctorViewModel)
    {
        if (ModelState.IsValid)
        {
            var doctor = await _unitOfWork.Doctors.FindBy(e => e.Id == doctorViewModel.DoctorId);

            if (doctor is not null)
            {
                var doctorReview = _doctorReviewMapper.MapFromViewModelToEntity(doctorViewModel);
                await _unitOfWork.DoctorReviews.Add(doctorReview);
                await _unitOfWork.Commit();

                var sumOfRates = _patientRepository.GetSumOfRates(doctorViewModel.DoctorId);


                if (doctor.TotalRatings == null)
                    doctor.TotalRatings = 1;
                else
                    doctor.TotalRatings += 1;
                decimal maxRating = 5M;


                decimal calculatedRating = ((decimal)sumOfRates / (decimal)(doctor.TotalRatings * maxRating));

                doctor.ActualRting = Math.Round(calculatedRating * maxRating, 1);

                _unitOfWork.Doctors.Update(doctor);
                await _unitOfWork.Commit();
            }
            else
            {
                return RedirectToAction("StatusCodeError", new { statusCode = 404 });
            }
        }
        return RedirectToAction("Search", "Patient");
    }
   
    [Route("doctorDetails/{id:int:min(1)}")]
    public async Task<IActionResult> Details(int id)
    {
        string applicationUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


        var patientId = _unitOfWork.Patients.GetPatientIdByUserId(applicationUserId);
        var doctorDetailsViewModel = await _doctorDetailsRepository.GetDoctorDetailsViewModel(id, patientId);
     
        return View(doctorDetailsViewModel);
    }
    [Route("ClinicDetails/{id:int:min(1)}")]

    public async Task<IActionResult> ClinicDetails(int id)
    {
        var clinic = await _doctorDetailsRepository.GetClinicsDetails(id);
        var clinicMapVm = _doctorReviewMapper.ClinicMapper(clinic);

        return View(clinicMapVm);
    }
    [Route("Account")]
    public async Task<IActionResult> Profile()
    {
        string[] includes = { "ClinicAppointments", "DoctorReviews", "ApplicationUser" };
        string applicationUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


        var patientId = _unitOfWork.Patients.GetPatientIdByUserId(applicationUserId);
        var patients = await _unitOfWork.Patients.FindBy(d => d.Id == patientId, includes);
        var mappedpatient = _mapper.Map<Patient, PatientViewModel>(patients);
        return View(mappedpatient);
    }
    [Route("AccountEdit/{id:int:min(1)}")]
    public async Task<IActionResult> ProfileEdit(int id)
    {
        string[] includes = { "ClinicAppointments", "DoctorReviews", "ApplicationUser" };

        var Patient = await _unitOfWork.Patients.FindBy(d => d.Id == id, includes);
        if (Patient == null)
        {
            return RedirectToAction("StatusCodeError", new { statusCode = 404 });
        }
        var mappedPatient = _mapper.Map<Patient, PatientViewModel>(Patient);

        return View(mappedPatient);
    }

    [HttpPost, Route("AccountEdit/{id:int:min(1)}")]
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

                    string[] includes = { "ClinicAppointments", "DoctorReviews", "ApplicationUser" };

                    var existingPatient = await _unitOfWork.Patients.FindBy(d => d.Id == model.Id, includes);
                    if (existingPatient == null)
                    {

                        return RedirectToAction("StatusCodeError", new { statusCode = 404 });
                    }
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

    [Route("BookAppointment/{id:int:min(1)}")]
    [Authorize(Roles = "Patient,Admin")]
    public async Task<IActionResult> Book(int id)
    {
            string[] includes = { "Doctor", "ClinicAppointments" };
            var clinic = await _unitOfWork.Clinics.FindBy(d => d.Id == id, includes);
            if (clinic == null)
            {
                return RedirectToAction("StatusCodeError", new { statusCode = 404 });
            }
            var mappedClinic = _mapper.Map<Clinic, BookViewModel>(clinic);
            mappedClinic.Clinic = clinic;
            TempData["ClinicId"] = clinic.Id;
            TempData["DoctorId"] = clinic.Doctor.Id;

            TimeSpan openingHour = clinic.OpeningHour.ToTimeSpan();
            TimeSpan closingHour = clinic.ClosingHour.ToTimeSpan();
            TimeSpan examinationDuration = clinic.ExmainationDuration.ToTimeSpan();


            TimeSpan clinicDuration = closingHour - openingHour;
            var appointmentlist = new List<TimeOnly?>();
            int examinationPeriods = (int)(clinicDuration.TotalMinutes / examinationDuration.TotalMinutes);
            ViewData[$"{clinic.Name}"] = examinationPeriods;


            foreach (var appointment in clinic.ClinicAppointments)
            {
                if (appointment.ReservationPeriodNumber is not null && appointment.Date == DateOnly.FromDateTime(DateTime.Now) && appointment.IsDeleted == false)
                {

                    appointmentlist.Add(appointment.ReservationPeriodNumber);
                }
            }
            mappedClinic.ClinicAppointmentDictionary.Add(clinic.Id, appointmentlist);
            string applicationUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            mappedClinic.patientId = _unitOfWork.Patients.GetPatientIdByUserId(applicationUserId);
            return View(mappedClinic);
    }
    


        [HttpPost, Route("BookingConfirm")]
        public async Task<IActionResult> BookConfirmed(BookViewModel book)
        {
            if (book.ReservtionPeriodNumber is null && book.IsVisitHome == true)
                ModelState.Remove("ReservtionPeriodNumber");
            if (ModelState.IsValid)
            {
                int appointmentId = 0;
                int addedAppointmentId = 0;
                bool IsHomeExisted = await _patientRepository.AnyHomeVisit(book.patientId, book.DoctorId, book.Date, book.ISHomeAppointmentDeleted, book.IsAttended);
                bool IsClinicExisted = await _patientRepository.AnyAppointment(book.patientId, book.ClinicId, book.Date, book.ISClinicAppointmentDeleted, book.IsAttended);
                if (book.ClinicId is null)
                {
                    if (!IsHomeExisted)
                    {
                        var appointmentbooked = _mapper.Map<BookViewModel, HomeAppointment>(book);
                        await _unitOfWork.HomeAppointment.Add(appointmentbooked);
                        var count = await _unitOfWork.Commit();
                        appointmentId = appointmentbooked.Id;

                    }
                    else
                    {
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
                        var count = await _unitOfWork.Commit();
                        appointmentId = appointmentbooked.Id;
                    }
                    else
                    {
                        TempData["BookMessage"] = $"You already booked for today, See your appointment here";
                        return RedirectToAction(nameof(AppointmentIndex));
                    }
                }

                TempData["BookMessage"] = $"Appointment Was booked succssfully";

                if (book.IsWantToPayOnline == true && book.ClinicId is null)
                    return RedirectToAction("CheckoutSession", "Payment", new { fees = book.HomeVisitFees, clinicName = "HomeVisit", appointmentId = appointmentId });

                else if (book.IsWantToPayOnline == true && book.ClinicId is not null)
                    return RedirectToAction("CheckoutSession", "Payment", new { fees = book.ClinicFees, clinicName = book.ClinicName, appointmentId = appointmentId });


                else if (book.IsWantToPayOnline == true && book.ClinicId is not null)
                    return RedirectToAction("CheckoutSession", "Payment", new { fees = book.ClinicFees, clinicName = book.ClinicName, appointmentId = appointmentId });


                return RedirectToAction(nameof(AppointmentIndex));
            }
            return RedirectToAction(nameof(Book), new { id = TempData["ClinicId"], doctorId = TempData["DoctorId"] });
        }


        [Route("MyAppointments")]
        public async Task<IActionResult> AppointmentIndex()
        {
            string applicationUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            int patientId = _unitOfWork.Patients.GetPatientIdByUserId(applicationUserId);


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
            var appointment = await _unitOfWork.ClinicAppointments.FindByWithTwoThenIncludes(a => a.IsDeleted == false && a.patientId == patientId, includes);
            var homeappointment = await _unitOfWork.HomeAppointment.FindByWithTwoThenIncludes(a => a.IsDeleted == false && a.PatientId == patientId, Homeincludes);
            var mappedappointmwnts = _mapper.Map<IEnumerable<ClinicAppointment>, IEnumerable<AppointmentViewModel>>(appointment);
            totalAppointments.AddRange(mappedappointmwnts);
            mappedappointmwnts = _mapper.Map<IEnumerable<HomeAppointment>, IEnumerable<AppointmentViewModel>>(homeappointment);
            totalAppointments.AddRange(mappedappointmwnts);


            return View(totalAppointments);


        }
        [Route("CancelAppointment")]
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
        [HttpPost, Route("CancelAppointmentConfirmed")]
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
            if (model.ReservationPeriodNumber is null && homeappointment is not null)
            {
                homeappointment.IsDeleted = true;
                _unitOfWork.HomeAppointment.Update(homeappointment);

            }
            else if (model.ReservationPeriodNumber is not null && appointment is not null)
            {
                appointment.IsDeleted = true;
                _unitOfWork.ClinicAppointments.Update(appointment);
            }
            await _unitOfWork.Commit();

            string applicationUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            string userPhoneNumber = _userManager.Users.FirstOrDefault(u => u.Id == applicationUserId).PhoneNumber;
            if (!string.IsNullOrEmpty(userPhoneNumber))
            {
                string toPhoneNumber = $"+2{userPhoneNumber}";
                string Smsbody = $"we would like to confirm with you that your appointment was cancelld successfully, best wishes, Etammen team.";
                MessageResource result = await _smsService.SendSmsAsync(toPhoneNumber, Smsbody);
            }

            TempData["MessageWasSent"] = $" appointment is cancelled successfully";

            if (homeappointment is not null && homeappointment.PaymentIntentId is not null)
            {
                return RedirectToAction("RefundPayment", "Payment", new { appointmentId = homeappointment.Id });
            }
            if (appointment is not null && appointment.PaymentIntentId is not null)
            {
                return RedirectToAction("RefundPayment", "Payment", new { appointmentId = appointment.Id });
            }
            return RedirectToAction(nameof(AppointmentIndex));
        }
}


