using AutoMapper;
using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Models;
using Etammen.Mapping.DoctorForAdmin;
using Etammen.ViewModels.Admin.Doctor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Etammen.Controllers
{
    public class DoctorAdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DoctorsAdminMapper _doctorsMapper;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        public DoctorAdminController(IUnitOfWork unitOfWork, DoctorsAdminMapper getAllDoctorsMapper, IMapper mapper, UserManager<ApplicationUser> applicationUser)
        {
            _unitOfWork = unitOfWork;
            _doctorsMapper = getAllDoctorsMapper;
            _mapper = mapper;
            _userManager = applicationUser;
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
        public async Task<IActionResult> Approve()
        {
            string[] includes = ["ApplicationUser"];
            IEnumerable<Doctor> Doctors = await _unitOfWork.Doctors.FindAllBy((D => D.IsRegistered == false && D.IsDeleted == false), includes);
            IEnumerable<ApproveDoctorViewModel> mappedDoctors =
                _mapper.Map<IEnumerable<Doctor>, IEnumerable<ApproveDoctorViewModel>>(Doctors);
            return View(mappedDoctors);
        }

        public async Task<IActionResult> ApprovePost(int id, bool isApproved)
        {
            Doctor doctor = await _unitOfWork.Doctors.FindBy((D) => D.Id == id && D.IsDeleted == false, ["ApplicationUser"]);
            if (isApproved)
            {
                doctor.IsRegistered = true;
                _unitOfWork.Doctors.Update(doctor);
            }
            else
            {
                //Send Email To User tilling him that he was declined
                _unitOfWork.Doctors.Delete(doctor.Id, true);
                await _userManager.DeleteAsync(doctor.ApplicationUser);
            }
            int rows = await _unitOfWork.Commit();
            return RedirectToAction(nameof(Approve));
        }
    }
}
