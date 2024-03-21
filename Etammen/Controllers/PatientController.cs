using AutoMapper;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Models;
using DataAccessLayerEF.SettingsModel;
using Etammen.Helpers;
using Etammen.Mapping.DoctorForAdmin;
using Etammen.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tsp;
using System;
using System.Globalization;
using System.Linq.Expressions;

namespace Etammen.Controllers
{
    public class PatientController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPatientRepository _patientRepository;
        private readonly DoctorsAdminMapper _doctorsMapper;
        private readonly DoctorRegisterationHelper _doctorRegisterationHelper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISmsService _smsService;
        List<AppointmentViewModel> totalAppointments;

        public PatientController(IUnitOfWork unitOfWork,
               DoctorsAdminMapper getAllDoctorsMapper,
               IPatientRepository patientRepository,
               DoctorRegisterationHelper doctorRegisterationHelper,
               UserManager<ApplicationUser> userManager,
               IMapper mapper,
               ISmsService smsService)
        {
            _unitOfWork = unitOfWork;
            _doctorsMapper = getAllDoctorsMapper;
            _patientRepository = patientRepository;
            _doctorRegisterationHelper = doctorRegisterationHelper;
            _userManager = userManager;
            _mapper = mapper;
            _smsService = smsService;
            totalAppointments = new List<AppointmentViewModel>();
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
            var searchedDoctors = await _unitOfWork.Doctors.Search(mainViewModel.specialty, mainViewModel.city,
                mainViewModel.area, mainViewModel.doctorName, mainViewModel.clinicName);

            mainViewModel.SearchedDoctors = searchedDoctors.ToList();

            DoctorFilterOptions filterOptions = _mapper.Map<MainViewModel, DoctorFilterOptions>(mainViewModel);
            mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.FilterByOptions(filterOptions,
                mainViewModel.SearchedDoctors);
            mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.OrderByOption(mainViewModel.Order,
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
                    return RedirectToAction("Index", new { mainViewModel, pageNumber = totalPages, pageSize });
                }

                var doctors = _patientRepository.PatientsPaginationNextAsync(mainViewModel.FilteredOrderedDoctors, pageNumber, pageSize);
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
        public async Task<IActionResult> Filter(MainViewModel mainViewModel)
        {
            DoctorFilterOptions filterOptions = _mapper.Map<MainViewModel, DoctorFilterOptions>(mainViewModel);
            mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.FilterByOptions(filterOptions,
                mainViewModel.SearchedDoctors);
            mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.OrderByOption(mainViewModel.Order,
                mainViewModel.FilteredOrderedDoctors);
            return RedirectToAction("Pagination", mainViewModel);
        }
        public async Task<IActionResult> Order(MainViewModel mainViewModel)
        {
            mainViewModel.FilteredOrderedDoctors = _unitOfWork.Doctors.OrderByOption(mainViewModel.Order,
                mainViewModel.FilteredOrderedDoctors);
            return RedirectToAction("Pagination", mainViewModel);
        }

        public async Task<IActionResult> Profile(int id = 1)
        {
            string[] includes = { "Appointments", "DoctorReviews", "ApplicationUser" };

            var patients = await _unitOfWork.Patients.FindBy(d => d.Id == id, includes);
            var mappedpatient = _mapper.Map<Patient, PatientViewModel>(patients);
            return View(mappedpatient);
        }

        public async Task<IActionResult> ProfileEdit(int id)
        {
            string[] includes = { "Appointments", "DoctorReviews", "ApplicationUser" };

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
                        string[] includes = { "Appointments", "DoctorReviews", "ApplicationUser" };

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

        public async Task<IActionResult> DoctorIndex()
        {
            string[] includes = { "Clinics", "DoctorReviews", "ApplicationUser" };
            var doctors = await _unitOfWork.Doctors.GetAll(includes);
            var mappedDoctors = _mapper.Map<IEnumerable<Doctor>, IEnumerable<DoctorViewModel>>(doctors);
            return View(mappedDoctors);
        }
        public async Task<IActionResult> ClinicIndex(int id)
        {
            ViewBag.id = id;
            var includes = new Dictionary<Expression<Func<Clinic, object>>, Expression<Func<object, object>>>();
            includes.Add(c => c.Doctor, d => ((Doctor)d).ApplicationUser);
            var clinicList = await _unitOfWork.Clinics.GetAllWithExpression(includes, c => c.IsDeleted == false && c.DoctorId == id);

            var mappedClinics = _mapper.Map<IEnumerable<Clinic>, IEnumerable<ClinicViewModel>>(clinicList);
            return View(mappedClinics);
        }
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
                        await _unitOfWork.HomeAppointment.AddAsync(appointmentbooked);
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
                        await _unitOfWork.ClinicAppointments.AddAsync(appointmentbooked);
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

                return RedirectToAction(nameof(DoctorIndex));
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
