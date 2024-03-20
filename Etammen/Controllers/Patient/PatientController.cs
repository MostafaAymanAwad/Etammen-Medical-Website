using AutoMapper;
using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Enums;
using DataAccessLayerEF.Models;
using Etammen.Mapping;
using Etammen.Mapping.DoctorForAdmin;
using Etammen.ViewModels;
using Etammen.ViewModels.Admin.Doctor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
using System.Linq.Expressions;

namespace Etammen.Controllers.Patient
{
    public class PatientController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPatientRepository _patientRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IApplicationUser _applicationUser;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDoctorReviewsRepository _doctorReviewsRepository;
        private readonly DoctorsAdminMapper _doctorsMapper;
        private readonly DoctorReviewMapping _doctorReviewMapper;
        private readonly DoctorDetailsMapping _doctorDetailsMapping;
        private readonly ClinicDetailsForDoctorPageMapper _clinicMapper;

        public PatientController(IUnitOfWork unitOfWork, 
            DoctorsAdminMapper getAllDoctorsMapper,
            IPatientRepository patientRepository, 
            DoctorReviewMapping doctorReviewMapping, 
            DoctorDetailsMapping doctorDetailsMapping,
            IClinicRepository clinicRepository,
            IApplicationUser applicationUser,
            IAppointmentRepository appointmentRepository,
            IDoctorReviewsRepository doctorReviewsRepository,
            ClinicDetailsForDoctorPageMapper clinicMapper)
        {
            _unitOfWork = unitOfWork;
            _doctorsMapper = getAllDoctorsMapper;
            _patientRepository = patientRepository;
            _doctorReviewMapper = doctorReviewMapping;
            _doctorDetailsMapping = doctorDetailsMapping;
            _clinicRepository = clinicRepository;
            _applicationUser = applicationUser;
            _appointmentRepository = appointmentRepository;
            _doctorReviewsRepository = doctorReviewsRepository;
            _clinicMapper = clinicMapper;
        }


        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5)
        {
            try
            {
                var numberOfRows = _patientRepository.NumberOfRows;
                var totalPages = (int)Math.Ceiling((double)numberOfRows / pageSize);

                if (pageNumber < 1 || pageNumber > totalPages)
                {

                    return RedirectToAction("Index", new { pageNumber = totalPages, pageSize });
                }

                var patients = await _patientRepository.PatientsPaginationNextAsync(pageNumber, pageSize);

                var viewModel = _doctorsMapper.MapDoctorEntitiesToDoctorViewModel(patients);

                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = totalPages;

                return View(viewModel);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /*
         * 
         * This view should be incorporated into the Doctor's profile, 
         * but I'm awaiting for it from Moaz and Mostafa. 
         * I must disable the rating feature for patients associated with the same doctor, 
         * as duplicating doctor and patient IDs is prohibited due to the composite primary key constraint.
         * Upon a patient's entry into the doctor's profile, I will verify if the doctor's review table contains,
         * both the patient ID and the doctor ID. If true, 
         * I will either remove the form from the DOM or render it as read-only.
         * 
         */
        public IActionResult DoctorReviews(int id)
        {
            //var doctor = await _unitOfWork.Doctors.FindBy(e => e.Id == id);
            var doctorViewModel = new DoctorReviewViewModel()
            {
                DoctorId = id,
                // Just to get things going, but i'm Waiting for the Patient id from Fawzi
                PatientId = 4
            };
            return View(doctorViewModel);
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
            doctorDetailsViewModel.IsReview = _doctorReviewsRepository.IsReviewdBy(id,5);
            return View(doctorDetailsViewModel);
        }

        public IActionResult GetDoctors(string[] degrees, decimal[] feess, int gender,int pageNumber)
        {
            var doctors = _patientRepository.GetAllDoctorsFilter(degrees, feess, gender, pageNumber);
            var doctorsVM = _doctorsMapper.MapDoctorEntitiesToDoctorViewModel(doctors);
            return PartialView("_Patient", doctorsVM);
        }
    }
}
