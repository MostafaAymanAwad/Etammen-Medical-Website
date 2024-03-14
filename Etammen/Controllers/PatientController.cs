using AutoMapper;
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
		public async Task<IActionResult> Index(string specialty, string city, string area,string doctorName,string clinicName)
		{
			 var searchedDoctors= await _unitOfWork.Doctors.Search(specialty, city, area, doctorName, clinicName);
			 var mappedDoctors=_mapper.Map<IEnumerable<Doctor>, IEnumerable<DoctorWithNameViewModel>>(searchedDoctors);
			 return View(mappedDoctors);
		}
        public async Task<IActionResult> pagination(int pageNumber = 1, int pageSize = 10)
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
                // Log exception
                throw; // Rethrow the exception for further investigation
            }
        }



        // GET: PatientController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PatientController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PatientController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PatientController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PatientController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PatientController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PatientController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }

}
