using AutoMapper;
using BusinessLogicLayer.Interfaces;
using DataAccessLayerEF.Models;
using Etammen.Mapping.DoctorForAdmin;
using Etammen.Services.Email;
using Etammen.ViewModels.Admin.Doctor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace Etammen.Controllers
{
    [Route("doctorAdmin")]
    public class DoctorAdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DoctorsAdminMapper _doctorsMapper;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        public DoctorAdminController(IUnitOfWork unitOfWork, DoctorsAdminMapper getAllDoctorsMapper, IMapper mapper,
            UserManager<ApplicationUser> applicationUser, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _doctorsMapper = getAllDoctorsMapper;
            _mapper = mapper;
            _userManager = applicationUser;
            _emailService = emailService;
        }
        [Route("GetAllDoctors")]
        public async Task<IActionResult> Index()
        {
            var doctors = await _unitOfWork.Doctors.FindAllBy(e => e.IsDeleted == false, new[] { "ApplicationUser" });
            var doctorsViewModel = _doctorsMapper.MapDoctorsToViewModel(doctors);
            return View(doctorsViewModel);
        }
        [Route("GetDoctorDetails/{id:int:min(1)}")]
        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _unitOfWork.Doctors.FindBy(e => e.Id == id, new[] { "ApplicationUser" });
            var doctorViewModel = _doctorsMapper.MapOneDoctorToViewModel(doctor);
            doctorViewModel.Clinics = await _unitOfWork.Clinics.FindAllBy(e => e.DoctorId == id);
            return View(doctorViewModel);

        }
        [Route("DeleteDoctor/{id:int:min(1)}")]

        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _unitOfWork.Doctors.FindBy(e => e.Id == id, new[] { "ApplicationUser" });
            var doctorViewModel = _doctorsMapper.MapOneDoctorToViewModel(doctor);
            doctorViewModel.Clinics = await _unitOfWork.Clinics.FindAllBy(e => e.DoctorId == id);
            return View(doctorViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("DeleteDoctor/{id:int:min(1)}")]
        public async Task<IActionResult> Delete(int id,GetDoctorByIdViewModel doctorViewModel)
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
        [Route("ApproveDoctor")]
        public async Task<IActionResult> Approve()
        {
            string[] includes = ["ApplicationUser"];
            IEnumerable<Doctor> Doctors = await _unitOfWork.Doctors.FindAllBy((D => D.IsRegistered == false && D.IsDeleted == false), includes);
            IEnumerable<ApproveDoctorViewModel> mappedDoctors =
                _mapper.Map<IEnumerable<Doctor>, IEnumerable<ApproveDoctorViewModel>>(Doctors);
            return View(mappedDoctors);
        }
        [Route("ApproveDoctor/{id:int}/{isApproved:bool}")]
        public async Task<IActionResult> ApprovePost(int id, bool isApproved)
        {
            Doctor doctor = await _unitOfWork.Doctors.FindBy((D) => D.Id == id && D.IsDeleted == false, ["ApplicationUser"]);
            if (isApproved)
            {
                doctor.IsRegistered = true;
                _unitOfWork.Doctors.Update(doctor);

                string rejectionMailBody = "we are glad to inform you that the documents you provided for you Etammen's account verification was accepted," +
                     " you can now log in to your account, best wishes doctor!";
                string rejectionMailSubject = "Etammen account Documents Verification Update";

                await SendStatusUpdateEmail(doctor.ApplicationUser.Email, rejectionMailSubject, rejectionMailBody);
                
            }
            else
            {
                _unitOfWork.Doctors.Delete(doctor.Id, true);
                await _userManager.DeleteAsync(doctor.ApplicationUser);

                string rejectionMailBody = "we are sorry to inform you that the documents you provided for you Etammen's account verification was not accepted," +
                    " please try to provide valid documents and register again";
                string rejectionMailSubject = "Etammen account Documents Verification Update";

                await _emailService.SendEmailAsync(new Message(doctor.ApplicationUser.Email,rejectionMailSubject, rejectionMailBody));

                await SendStatusUpdateEmail(doctor.ApplicationUser.Email, rejectionMailSubject, rejectionMailBody);

            }
            int rows = await _unitOfWork.Commit();
            return RedirectToAction(nameof(Approve));
        }

        private async Task SendStatusUpdateEmail(string email,string rejectionMailBody, string rejectionMailSubject)
        {
            await _emailService.SendEmailAsync(new Message(email, rejectionMailSubject, rejectionMailBody));
        }
    }
}
