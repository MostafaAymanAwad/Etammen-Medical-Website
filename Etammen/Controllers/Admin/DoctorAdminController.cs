using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Models;
using Etammen.Mapping.DoctorForAdmin;
using Etammen.ViewModels.Admin.Doctor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Etammen.Controllers.Admin
{
    public class DoctorAdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DoctorsAdminMapper _doctorsMapper;


        public DoctorAdminController(IUnitOfWork unitOfWork, DoctorsAdminMapper getAllDoctorsMapper)
        {
            _unitOfWork = unitOfWork;
            _doctorsMapper = getAllDoctorsMapper;

        }
        public async Task<IActionResult> Index()
        {
            var doctors = await _unitOfWork.Doctors.FindAllBy(e => e.IsDeleted == false, new[] { "ApplicationUser" });
            var doctorsViewModel = _doctorsMapper.MapDoctorsToViewModel(doctors);
            return View(doctorsViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _unitOfWork.Doctors.FindBy(e => e.Id == id, new[] { "ApplicationUser" });
            var doctorViewModel = _doctorsMapper.MapOneDoctorToViewModel(doctor);
            doctorViewModel.Clinics = await _unitOfWork.Clinics.FindAllBy(e => e.DoctorId == id);
            return View(doctorViewModel);

        }


        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _unitOfWork.Doctors.FindBy(e => e.Id == id, new[] { "ApplicationUser" });
            var doctorViewModel = _doctorsMapper.MapOneDoctorToViewModel(doctor);
            doctorViewModel.Clinics = await _unitOfWork.Clinics.FindAllBy(e => e.DoctorId == id);
            return View(doctorViewModel);
        }

        // POST: DoctorAdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(GetDoctorByIdViewModel doctorViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var doctor = await _unitOfWork.Doctors.FindBy(e => e.Id == doctorViewModel.Id);
                    doctor.IsDeleted = true;
                    doctor.DeletionDate = DateOnly.FromDateTime(DateTime.Now);
                    await _unitOfWork.Doctors.Delete(doctor.Id);
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
