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
using Twilio.Rest.IpMessaging.V2.Service.Channel;
using BusinessLogicLayer.Services.SMS;
using Twilio.TwiML.Messaging;

namespace Etammen.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISmsService _smsService;

        public DoctorsController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, ISmsService smsService)
        {

            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _smsService = smsService;
        }

        public async Task<IActionResult> Profile(int id = 8)
        {
            string[] includes = { "Clinics", "DoctorReviews", "ApplicationUser" };

            var doctor = await _unitOfWork.Doctors.FindBy(d => d.Id == id, includes);
            if (doctor == null)
            {
                return NotFound();
            }
            var mappedDoctor = _mapper.Map<Doctor, DoctorViewModel>(doctor);

            return View(mappedDoctor);
        }

        public async Task<IActionResult> ProfileEdit(int id)
        {
            string[] includes = { "Clinics", "DoctorReviews", "ApplicationUser" };

            var doctor = await _unitOfWork.Doctors.FindBy(d => d.Id == id, includes);
            if (doctor == null)
            {
                return NotFound();
            }
            var mappedDoctor = _mapper.Map<Doctor, DoctorViewModel>(doctor);

            return View(mappedDoctor);
        }

        [HttpPost]
        public async Task<IActionResult> ProfileEdit(DoctorViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (model.ProfilePictureFormFile is not null)
                {
                    model.ProfilePicture = DocumentSettings.UploadFile(model.ProfilePictureFormFile, "Images");
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



        public async Task<IActionResult> ClinicIndex(int id = 8)
        {
            ViewBag.id = id;
            var includes = new Dictionary<Expression<Func<Clinic, object>>, Expression<Func<object, object>>>();
            includes.Add(c => c.Doctor, d => ((Doctor)d).ApplicationUser);
            var clinicList = await _unitOfWork.Clinics.GetAllWithExpression(includes, c => c.IsDeleted == false && c.DoctorId == id);
            var mappedClinics = _mapper.Map<IEnumerable<Clinic>, IEnumerable<ClinicViewModel>>(clinicList);
            return View(mappedClinics);
        }
        public async Task<IActionResult> CreateClinic(int id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateClinic(ClinicViewModel VMmodel)
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
            return View();
        }
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
        [HttpPost]
        public async Task<IActionResult> EditClinic(ClinicViewModel VMmodel)
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

        public async Task<IActionResult> AppointmentIndex(int id = 8)
        {
            var includes = new Dictionary<Expression<Func<Appointment, object>>, List<Expression<Func<object, object>>>>()
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
            var appointment = await _unitOfWork.Appointments.FindByWithTwoThenIncludes(a => a.IsDeleted == false && a.Clinic.DoctorId == id, includes);
            var mappedappointmwnts = _mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentViewModel>>(appointment);
            return View(mappedappointmwnts);

        }

        public async Task<IActionResult> CancelAppointment(int id)
        {
            var includes = new Dictionary<Expression<Func<Appointment, object>>, List<Expression<Func<object, object>>>>()
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

            var appointment = await _unitOfWork.Appointments.FindByWithExpression(a => a.Id == id, includes);
            var mappedappointmwnts = _mapper.Map<Appointment, AppointmentViewModel>(appointment);


            return View(mappedappointmwnts);

        }
        [HttpPost, ActionName("CancelAppointment")]
        public async Task<IActionResult> CancelAppointmentConfirmed(int id)
        {
            var includes = new Dictionary<Expression<Func<Appointment, object>>, List<Expression<Func<object, object>>>>()
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

            var appointment = await _unitOfWork.Appointments.FindByWithExpression(a => a.Id == id, includes);

            appointment.IsDeleted = true;

            var PatientFullName = $"{appointment.Patient.ApplicationUser.FirstName} {appointment.Patient.ApplicationUser.LastName}";
            var PhoneNumber = appointment.Patient.ApplicationUser.PhoneNumber;
            var messageBody = $"Dear Mr {PatientFullName} : Your Appointment was Canceled by the doctor for some reason you can book another time if you wish . Sorry For the Inconvenience";

            _unitOfWork.Appointments.Update(appointment);
            await _unitOfWork.Commit();
            var messageSent = _smsService.SendSmsAsync(PhoneNumber, messageBody);
            if (messageSent is not null)
                TempData["MessageWasSent"] = $"cancelation Message Was Sent to patient {PatientFullName}";
            return RedirectToAction(nameof(AppointmentIndex));

        }

        public async Task<IActionResult> homeVisitAppointmentsIndex(int id)
        {
            var includes = new Dictionary<Expression<Func<Appointment, object>>, List<Expression<Func<object, object>>>>()
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
            var appointment = await _unitOfWork.Appointments.FindByWithTwoThenIncludes(a => a.IsDeleted == false && a.Clinic.DoctorId == id, includes);
            var mappedappointmwnts = _mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentViewModel>>(appointment);
            return View(mappedappointmwnts);

        }

        public async Task<IActionResult> DeactivateAccount(int id = 4)
        {
            ViewBag.id = id;
            return View();
        }

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
            return RedirectToAction("Index", "Home");
        }
    }
}
