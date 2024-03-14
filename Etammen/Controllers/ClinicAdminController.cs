using BusinessLogicLayer.Interfaces;
using Etammen.Mapping.ClinicForAdmin;
using Etammen.ViewModels.Admin.Clinic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Etammen.Controllers
{
    public class ClinicAdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ClinicAdminMapper _mapper;
        public ClinicAdminController(IUnitOfWork unitOfWork, ClinicAdminMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var clinics = await _unitOfWork.Clinics.FindAllBy(e => e.IsDeleted == false, new[] { "Doctor" });
            var clinicViewModels = _mapper.MapClinicsToGetAllViewModel(clinics);
            return View(clinicViewModels);
        }

        public async Task<IActionResult> Details(int id)
        {
            var clinic = await _unitOfWork.Clinics.FindBy(e => e.Id == id, new[] { "Doctor" });
            var doctorId = clinic.DoctorId;
            var doctor = await _unitOfWork.Doctors.FindBy(e => e.Id == doctorId, new[] { "ApplicationUser" });
            var clinicViewModel = _mapper.MapOneClinicToViewModel(clinic);
            clinicViewModel.DoctorFirstName = doctor.ApplicationUser.FirstName;
            clinicViewModel.DoctorLastName = doctor.ApplicationUser.LastName;
            return View(clinicViewModel);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var clinic = await _unitOfWork.Clinics.FindBy(e => e.Id == id, new[] { "Doctor" });
            var doctorId = clinic.DoctorId;
            var doctor = await _unitOfWork.Doctors.FindBy(e => e.Id == doctorId, new[] { "ApplicationUser" });
            var clinicViewModel = _mapper.MapOneClinicToViewModel(clinic);
            clinicViewModel.DoctorFirstName = doctor.ApplicationUser.FirstName;
            clinicViewModel.DoctorLastName = doctor.ApplicationUser.LastName;
            return View(clinicViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(GetOneClinicViewModel clinicViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var clinic = await _unitOfWork.Clinics.FindBy(e => e.Id == clinicViewModel.Id);
                    clinic.IsDeleted = true;
                    clinic.DeletionDate = DateOnly.FromDateTime(DateTime.Now);
                    await _unitOfWork.Clinics.Delete(clinic.Id);
                    await _unitOfWork.Commit();
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(clinicViewModel);
            }
        }
    }
}
