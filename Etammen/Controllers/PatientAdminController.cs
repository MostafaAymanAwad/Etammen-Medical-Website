using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Models;
using Etammen.Mapping.PatientForAdmin;
using Etammen.ViewModels.Admin.Doctor;
using Etammen.ViewModels.Admin.Patient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Etammen.Controllers
{
    public class PatientAdminController : Controller
    {
        IUnitOfWork _unitOfWork;
        PatientForAdminMapper _mapper;
        public PatientAdminController(IUnitOfWork unitOfWork, PatientForAdminMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var patients = await _unitOfWork.Patients.FindAllBy(e => e.IsDeleted == false, new[] { "ApplicationUser" });
            var patientViewModels = _mapper.MapDoctorsToViewModel(patients);
            return View(patientViewModels);
        }

        // GET: PatientController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var patient = await _unitOfWork.Patients.FindBy(e => e.Id == id, new[] { "ApplicationUser" });
            var patientViewModel = _mapper.MapPatientToViewModel(patient);
            return View(patientViewModel);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var patient = await _unitOfWork.Patients.FindBy(e => e.Id == id, new[] { "ApplicationUser" });
            var patientViewModel = _mapper.MapPatientToViewModel(patient);
            return View(patientViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(GetOnePatientViewModel doctorViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var patient = await _unitOfWork.Patients.FindBy(e => e.Id == doctorViewModel.Id, new[] { "ApplicationUser" });
                    patient.IsDeleted = true;
                    patient.DeletionDate = DateOnly.FromDateTime(DateTime.Now);
                    await _unitOfWork.Patients.Delete(patient.Id);
                    await _unitOfWork.Commit();
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(doctorViewModel);
            }
        }
    }
}
