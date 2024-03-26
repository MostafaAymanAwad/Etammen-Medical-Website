using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccessLayerEF.Context;
using DataAccessLayerEF.Models;
using BusinessLogicLayer.Interfaces;
using Etammen.ViewModels;
using AutoMapper;
using Etammen.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using DataAccessLayerEF.SettingsModel;
using BusinessLogicLayer.Services.SMS;
using Microsoft.AspNetCore.Authorization;
using BusinessLogicLayer.Helpers;
using Org.BouncyCastle.Tls;
using Twilio.Rest.Api.V2010.Account;
using System.Security.Claims;

namespace Etammen.Controllers
{
    [AllowAnonymous, Route("Doctors")]
    public class DoctorsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISmsService _smsService;
        private readonly DoctorRegisterationHelper _registerationHelper;
        private const string UploadedPicturesFolder = "DoctorImages";


        public DoctorsController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager,
            ISmsService smsService, DoctorRegisterationHelper registerationHelper)
        {

            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _smsService = smsService;
            _registerationHelper = registerationHelper;
        }
        [Route("DoctorAccount")]
        public async Task<IActionResult> Profile()
        {
            string[] includes = { "Clinics", "DoctorReviews", "ApplicationUser" };

            string applicationUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            int doctorId = _unitOfWork.Doctors.GetDoctorIdByUserId(applicationUserId);

            var doctor = await _unitOfWork.Doctors.FindBy(d => d.Id == doctorId, includes);
            if (doctor == null)
            {
                return RedirectToAction("StatusCodeError", "Error", new { statusCode = 404 });
            }
            var mappedDoctor = _mapper.Map<Doctor, DoctorViewModel>(doctor);

            return View(mappedDoctor);
        }
        [Route("DoctorAccountEdit/{id:int:min(1)}")]
        public async Task<IActionResult> ProfileEdit(int id)
        {
            string[] includes = { "Clinics", "DoctorReviews", "ApplicationUser" };

            var doctor = await _unitOfWork.Doctors.FindBy(d => d.Id == id, includes);
            if (doctor == null)
            {
                return RedirectToAction("StatusCodeError", "Error", new { statusCode = 404 });
            }

            var mappedDoctor = _mapper.Map<Doctor, DoctorViewModel>(doctor);
            mappedDoctor.OldProfilePicture = doctor.ProfilePicture;
            return View(mappedDoctor);
        }


        [HttpPost,Route("DoctorAccountEdit/{id:int}")]
        public async Task<IActionResult> ProfileEdit(int id,DoctorViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.ProfilePictureFormFile is not null)
                {

                    List<string> imagePAths = _registerationHelper.SaveUploadedImages(new List<IFormFile> { model.ProfilePictureFormFile }, UploadedPicturesFolder);
                    model.ProfilePicture = imagePAths[0];

                }

                var mappeddoctor = _mapper.Map<DoctorViewModel, Doctor>(model);
                var existinuser = await _userManager.FindByIdAsync(model.ApplicationUserId);
                if (existinuser is not null)
                {
                    existinuser.FirstName = model.FirstName;
                    existinuser.LastName = model.LastName;

                    var result = await _userManager.UpdateAsync(existinuser);

                    if (result.Succeeded)
                    {

                        //_unitOfWork.Doctors.Update(mappeddoctor);
                        string[] includes = { "Clinics", "DoctorReviews", "ApplicationUser" };

                        var existingDoctor = await _unitOfWork.Doctors.FindBy(d => d.Id == model.Id, includes);
                        if (existingDoctor == null)
                        {
                            return NotFound();
                        }
                        existingDoctor.Id = model.Id;
                        existingDoctor.AboutTheDoctor = model.AboutTheDoctor;
                        existingDoctor.HomeVisitFees = model.HomeVisitFees;
                        existingDoctor.IsVisitHome = model.IsVisitHome;
                        existingDoctor.AboutTheDoctor = model.AboutTheDoctor;
                        existingDoctor.ApplicationUserId = model.ApplicationUserId;
                        existingDoctor.YearsOfExperience = model.YearsOfExperience;
                        existingDoctor.ProfilePicture = model.ProfilePicture;

                        await _unitOfWork.Commit();
                        return RedirectToAction(nameof(Profile));
                    }
                    return RedirectToAction(nameof(Profile));
                }
            }

            return View(model);
        }
        [Route("myClinics")]
        public async Task<IActionResult> ClinicIndex()
        {
            string applicationUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            int doctorId = _unitOfWork.Doctors.GetDoctorIdByUserId(applicationUserId);

            var includes = new Dictionary<Expression<Func<Clinic, object>>, Expression<Func<object, object>>>();
            includes.Add(c => c.Doctor, d => ((Doctor)d).ApplicationUser);
            var clinicList = await _unitOfWork.Clinics.GetAllWithExpression(includes, c => c.IsDeleted == false && c.DoctorId == doctorId);

            var mappedClinics = _mapper.Map<IEnumerable<Clinic>, IEnumerable<ClinicViewModel>>(clinicList);

            ViewBag.doctorId = doctorId;
            return View(mappedClinics);
        }
        [Route("AddClinic/{id:int:min(1)}")]

        public async Task<IActionResult> CreateClinic(int id)
        {
            ViewBag.doctorId = id;
            return View();
        }

        [HttpPost, Route("AddClinic/{id:int:min(1)}")]
        public async Task<IActionResult> CreateClinic(int id,ClinicViewModel VMmodel)
        {
            if (ModelState.IsValid)
            {
                var mappedClinic = _mapper.Map<ClinicViewModel, Clinic>(VMmodel);
                await _unitOfWork.Clinics.Add(mappedClinic);
                var count = await _unitOfWork.Commit();
                if (count > 0)
                    TempData["CreateMessage"] = $" clinc {mappedClinic.Name} Was Added";

                return RedirectToAction(nameof(ClinicIndex));
            }

            ViewBag.doctorId = VMmodel.DoctorId;
            return View(VMmodel);
        }
        [Route("EditClinic/{id:int:min(1)}")]
        public async Task<IActionResult> EditClinic(int id)
        {
            var includes = new Dictionary<Expression<Func<Clinic, object>>, List<Expression<Func<object, object>>>>();

            List<Expression<Func<object, object>>> ThenIncludes = new List<Expression<Func<object, object>>>()
            {
                 d => ((Doctor)d).ApplicationUser
            };
            includes.Add(c => c.Doctor, ThenIncludes);

            var clinic = await _unitOfWork.Clinics.FindByWithExpression(c => c.Id == id, includes);
            var mappedClinic = _mapper.Map<Clinic, ClinicViewModel>(clinic);

            return View(mappedClinic);
        }
        
        [HttpPost, Route("EditClinic/{id:int:min(1)}")]
        public async Task<IActionResult> EditClinic(int id,ClinicViewModel VMmodel)
        {
            if (ModelState.IsValid)
            {
                var mappedClinic = _mapper.Map<ClinicViewModel, Clinic>(VMmodel);
                _unitOfWork.Clinics.Update(mappedClinic);
                var count = await _unitOfWork.Commit();
                if (count > 0)
                    TempData["EditedMessage"] = $"Changes Saved Successfully";
                return RedirectToAction(nameof(ClinicIndex));
            }
            return View();
        }
        [Route("ClinicDetails/{id:int:min(1)}")]
        public async Task<IActionResult> ClinicDetails(int id)
        {
            var includes = new Dictionary<Expression<Func<Clinic, object>>, List<Expression<Func<object, object>>>>();

            List<Expression<Func<object, object>>> ThenIncludes = new List<Expression<Func<object, object>>>()
            {
                 d => ((Doctor)d).ApplicationUser
            };
            includes.Add(c => c.Doctor, ThenIncludes);

            var clinic = await _unitOfWork.Clinics.FindByWithExpression(c => c.Id == id, includes);
            var mappedClinic = _mapper.Map<Clinic, ClinicViewModel>(clinic);

            return View(mappedClinic);
        }
        [Route("DeleteClinic/{id:int:min(1)}")]

        public async Task<IActionResult> ClinicDelete(int id)
        {
            var includes = new Dictionary<Expression<Func<Clinic, object>>, List<Expression<Func<object, object>>>>();

            List<Expression<Func<object, object>>> ThenIncludes = new List<Expression<Func<object, object>>>()
            {
                 d => ((Doctor)d).ApplicationUser
            };
            includes.Add(c => c.Doctor, ThenIncludes);

            var clinic = await _unitOfWork.Clinics.FindByWithExpression(c => c.Id == id, includes);
            var mappedClinic = _mapper.Map<Clinic, ClinicViewModel>(clinic);
            return View(mappedClinic);
        }
        [HttpPost, ActionName("ClinicDelete")]

        [Route("DeleteClinic/{id:int:min(1)}")]

        public async Task<IActionResult> ClinicDeleteConfirmed(int id)
        {
            var includes = new Dictionary<Expression<Func<Clinic, object>>, List<Expression<Func<object, object>>>>();

            List<Expression<Func<object, object>>> ThenIncludes = new List<Expression<Func<object, object>>>()
            {
                 d => ((Doctor)d).ApplicationUser
            };
            includes.Add(c => c.Doctor, ThenIncludes);

            var clinic = await _unitOfWork.Clinics.FindByWithExpression(c => c.Id == id, includes);

            clinic.IsDeleted = true;
            clinic.DeletionDate = DateOnly.FromDateTime(DateTime.Now);

            _unitOfWork.Clinics.Update(clinic);

            var count = await _unitOfWork.Commit();
            if (count > 0)
                TempData["DeleteMessage"] = $" Clinic {clinic.Name} Was Deleted";

            return RedirectToAction(nameof(ClinicIndex));
        }

        [Route("MyAppointment")]

        public async Task<IActionResult> AppointmentIndex()
        {
            string applicationUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            int doctorId = _unitOfWork.Doctors.GetDoctorIdByUserId(applicationUserId);
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
            int id = _unitOfWork.Doctors.GetDoctorIdByUserId(applicationUserId);
            ViewBag.doctorId = id;
            var appointment = await _unitOfWork.ClinicAppointments.FindByWithTwoThenIncludes(a => a.IsDeleted == false && a.Clinic.DoctorId == id, includes);
       
            var mappedappointmwnts = _mapper.Map<IEnumerable<ClinicAppointment>, IEnumerable<AppointmentViewModel>>(appointment);
            return View(mappedappointmwnts);

        }
        [Route("CancelDoctorAppointment/{id:int}")]

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
            var appointment = await _unitOfWork.ClinicAppointments.FindByWithExpression(a => a.Id == id, includes);
            var mappedappointmwnts = _mapper.Map<ClinicAppointment, AppointmentViewModel>(appointment);


            var homeincludes = new Dictionary<Expression<Func<HomeAppointment, object>>, List<Expression<Func<object, object>>>>()
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
            var homeappointment = await _unitOfWork.HomeAppointment.FindByWithExpression(a => a.Id == id, homeincludes);
            var mappedHomeappointmwnts = _mapper.Map<HomeAppointment, AppointmentViewModel>(homeappointment);


            if (ReservationPeriodNumber is null)
            {
                return View(mappedHomeappointmwnts);
            }
            else
            {
                return View(mappedappointmwnts);
            }

        }
        [HttpPost,Route("CancelDoctorAppointment/{id:int:min(1)}")]
        public async Task<IActionResult> CancelAppointment(int id,AppointmentViewModel model)
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
                await _unitOfWork.Commit();

                var PatientFullName = $"{homeappointment.Patient.ApplicationUser.FirstName} {homeappointment.Patient.ApplicationUser.LastName}";

                string toPhoneNumber = $"+2{homeappointment.Patient.ApplicationUser.PhoneNumber}";
                string smsBody = $"Dear Mr {PatientFullName} : Your Appointment was Canceled by the doctor for some reason you can book another time if you wish . Sorry For the Inconvenience";


                MessageResource result = await _smsService.SendSmsAsync(toPhoneNumber, smsBody);
                if (string.IsNullOrEmpty(result.ErrorMessage))
                    TempData["messagewassent"] = $"cancelation message was sent to patient {PatientFullName}";

                return RedirectToAction(nameof(homeVisitAppointmentsIndex));
            }
            else if (model.ReservationPeriodNumber is not null && appointment is not null)
            {
                appointment.IsDeleted = true;
                _unitOfWork.ClinicAppointments.Update(appointment);
                await _unitOfWork.Commit();

                var PatientFullName = $"{appointment.Patient.ApplicationUser.FirstName} {appointment.Patient.ApplicationUser.LastName}";

                string toPhoneNumber = $"+2{appointment.Patient.ApplicationUser.PhoneNumber}";
               
                string smsBody = $"Dear Mr {PatientFullName} : Your Appointment was Canceled by the doctor for some reason you can book another time if you wish . Sorry For the Inconvenience";
                MessageResource result = await _smsService.SendSmsAsync(toPhoneNumber, smsBody);
                
                if (string.IsNullOrEmpty(result.ErrorMessage))
                    TempData["messagewassent"] = $"cancelation message was sent to patient {PatientFullName}";

                return RedirectToAction(nameof(AppointmentIndex));
            }
            else
                return BadRequest();

        }
        [Route("ApointmentAttended/{id:int}")]
        public async Task<IActionResult> AttenededAppointment(int id, TimeOnly? ReservationPeriodNumber)
        {
            if (ReservationPeriodNumber is not null)
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
                var appointment = await _unitOfWork.ClinicAppointments.FindByWithExpression(a => a.Id == id, includes);
                appointment.IsAttended = true;
                appointment.IsAccepted = true;
                _unitOfWork.ClinicAppointments.Update(appointment);
                var count = await _unitOfWork.Commit();
                if (count > 0)
                    TempData["AttendedMessage"] = $"Appointment is marked as attended";
                return RedirectToAction(nameof(AppointmentIndex));

            }

            else
            {
                var Homeincludes = new Dictionary<Expression<Func<HomeAppointment, object>>, List<Expression<Func<object, object>>>>()
            {
                 {
                     a => a.Doctor,
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
                var Homeappointment = await _unitOfWork.HomeAppointment.FindByWithExpression(a => a.Id == id, Homeincludes);
                Homeappointment.IsAttended = true;
                _unitOfWork.HomeAppointment.Update(Homeappointment);
                var count = await _unitOfWork.Commit();
                if (count > 0)
                    TempData["AttendedMessage"] = $"Appointment is marked as attended";
                return RedirectToAction(nameof(homeVisitAppointmentsIndex));
            }

        }

        [Route("AcceptAppointment/{id:int}")]

        public async Task<IActionResult> AcceptAppointment(int id, TimeOnly? ReservationPeriodNumber)
        {
            if (ReservationPeriodNumber is not null)
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
                var appointment = await _unitOfWork.ClinicAppointments.FindByWithExpression(a => a.Id == id, includes);
                appointment.IsAccepted = true;
                _unitOfWork.ClinicAppointments.Update(appointment);
                var count = await _unitOfWork.Commit();
                if (count > 0)
                    TempData["AttendedMessage"] = $"You have accepted the appointment";
                return RedirectToAction(nameof(AppointmentIndex));

            }

            else
            {
                var Homeincludes = new Dictionary<Expression<Func<HomeAppointment, object>>, List<Expression<Func<object, object>>>>()
            {
                 {
                     a => a.Doctor,
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
                var Homeappointment = await _unitOfWork.HomeAppointment.FindByWithExpression(a => a.Id == id, Homeincludes);
                Homeappointment.IsAccepted = true;
                _unitOfWork.HomeAppointment.Update(Homeappointment);
                var count = await _unitOfWork.Commit();
                if (count > 0)
                    TempData["AttendedMessage"] = $"You have accepted the appointment";
                return RedirectToAction(nameof(homeVisitAppointmentsIndex));
            }

        }
        [Route("homeAppointment")]
        public async Task<IActionResult> homeVisitAppointmentsIndex()
        {
            var includes = new Dictionary<Expression<Func<HomeAppointment, object>>, List<Expression<Func<object, object>>>>()
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
            string applicationUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;


            int id = _unitOfWork.Doctors.GetDoctorIdByUserId(applicationUserId);
            var appointment = await _unitOfWork.HomeAppointment.FindByWithTwoThenIncludes(a => a.IsDeleted == false && a.DoctorId == id, includes);
            var mappedappointmwnts = _mapper.Map<IEnumerable<HomeAppointment>, IEnumerable<AppointmentViewModel>>(appointment);
            return View(mappedappointmwnts);

        }
        [Route("DeactivateAccount")]
        public async Task<IActionResult> DeactivateAccount()
        {
            string applicationUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            int doctorId = _unitOfWork.Doctors.GetDoctorIdByUserId(applicationUserId);
            return View(doctorId);
        }
        [Route("DeactivateAccount{id:int}")]

        public async Task<IActionResult> DeactivateAccountConfirmed(int id)
        {

            string[] includes = { "Clinics", "DoctorReviews", "ApplicationUser" };

            var doctor = await _unitOfWork.Doctors.FindBy(d => d.Id == id, includes);
            if (doctor == null)
            {
                return NotFound();
            }
            doctor.IsDeleted = true;
            doctor.DeletionDate = DateOnly.FromDateTime(DateTime.Now);
            foreach (var clinic in doctor.Clinics)
            {
                clinic.IsDeleted = true;
                clinic.DeletionDate = DateOnly.FromDateTime(DateTime.Now);
            }
            _unitOfWork.Doctors.Update(doctor);
            await _unitOfWork.Commit();
            return RedirectToAction("Logout", "Account");
        }
    }
}
