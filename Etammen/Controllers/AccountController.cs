using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Etammen.ViewModels;
using Etammen.Helpers;
using Microsoft.AspNetCore.Identity;
using DataAccessLayerEF.Models;
using Etammen.Mapping;
using Etammen.Services.Email;
using DataAccessLayerEF.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessLogicLayer.Services.SMS;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Jwt.AccessToken;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DataAccessLayerEF.SettingsModel;
using Twilio.Types;



namespace Etammen.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AccountMapper _mapper;
    private readonly DoctorRegisterationHelper _doctorRegisterationHelper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<AccountController> _logger;


    private const string UploadedPicturesFolder = "DoctorImages";
    private const string EmailConfirmationHtml = "EmailConfirmation/new-email.html";


    public AccountController(IUnitOfWork unitOfWork, AccountMapper mapper, DoctorRegisterationHelper doctorRegisterationData,
        UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        IWebHostEnvironment webHostEnvironment, IEmailService emailService, ISmsService smsService,
        ILogger<AccountController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _doctorRegisterationHelper = doctorRegisterationData;
        _userManager = userManager;
        _signInManager = signInManager;
        _webHostEnvironment = webHostEnvironment;
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }
    public IActionResult RegisterDoctor()
    {
            DoctorRegisterViewModel doctorRegisterViewModel = new DoctorRegisterViewModel();
            populateDoctorViewModelLists(doctorRegisterViewModel);
            return View(doctorRegisterViewModel);
    }
    private void populateDoctorViewModelLists(DoctorRegisterViewModel doctorRegisterViewModel)
    {
        doctorRegisterViewModel.SpecialityList = _doctorRegisterationHelper.SpecialitySelectList;
        doctorRegisterViewModel.DegreeList = _doctorRegisterationHelper.DegreeSelectList;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterDoctor(DoctorRegisterViewModel doctorRegisterViewModel, IFormFile certificate, IFormFile profilePicture)
    {
        if (certificate is null || profilePicture is null)
        {
            ModelState.TryAddModelError("", "Please upload valid pictures");
            populateDoctorViewModelLists(doctorRegisterViewModel);
            return View(doctorRegisterViewModel);
        }
        ModelState.Remove("SpecialityList");
        ModelState.Remove("DegreeList");
        if (ModelState.IsValid)
        {
            ApplicationUser newUser = _mapper.GetUserFromVM(doctorRegisterViewModel);
            IdentityResult result = await _userManager.CreateAsync(newUser, doctorRegisterViewModel.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "Doctor");
                await SendEmailConfirmation(newUser);

                List<string> imagePAths = _doctorRegisterationHelper.SaveUploadedImages(new List<IFormFile> { certificate, profilePicture }, UploadedPicturesFolder);

                Doctor newDoctor = _mapper.GetDoctorFromVM(doctorRegisterViewModel, newUser.Id, imagePAths);
                await _unitOfWork.Doctors.Add(newDoctor);
                await _unitOfWork.Commit();
                return View("RegisterationSuccess");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError("", error.Description);
                }
            }
        }
        populateDoctorViewModelLists(doctorRegisterViewModel);
        return View(doctorRegisterViewModel);
    }

    public IActionResult Register()
    {
        PatientRegisterViewModel patientRegisterViewModel = new PatientRegisterViewModel();
        patientRegisterViewModel.GovCitiesDict = _doctorRegisterationHelper.CityAreasDictionary;

        return View(patientRegisterViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(PatientRegisterViewModel patientRegisterViewModel)
    {
        ModelState.Remove("GovCitiesDict");
        if (ModelState.IsValid)
        {
            ApplicationUser newUser = _mapper.GetUserFromVM(patientRegisterViewModel);

            IdentityResult result = await _userManager.CreateAsync(newUser, patientRegisterViewModel.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "Patient");
                await SendEmailConfirmation(newUser);

                Patient newPatient = _mapper.GetPatientFromVM(patientRegisterViewModel, newUser.Id);
                await _unitOfWork.Patients.Add(newPatient);
                await _unitOfWork.Commit();

                return View("RegisterationSuccess");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError("", error.Description);
                }
            }
        }
        patientRegisterViewModel.GovCitiesDict = _doctorRegisterationHelper.CityAreasDictionary;
        return View(patientRegisterViewModel);
    }

    private async Task SendEmailConfirmation(ApplicationUser newUser)
    {
        string mailBody;
        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
        string callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = newUser.Id, token = emailConfirmationToken }, HttpContext.Request.Scheme);
        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, EmailConfirmationHtml);
        using (var streamreader = new StreamReader(filePath))
        {
            mailBody = streamreader.ReadToEnd();
            streamreader.Close();
        }
        mailBody = mailBody.Replace("callbackplaceholder", callbackUrl);

        await _emailService.SendEmailAsync(new Message(newUser.Email, "confirmation email for Etammen.com", mailBody));
    }
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId is null || token is null)
            return View("ConfirmationFailed");
        ApplicationUser userToConfirm = await _userManager.FindByIdAsync(userId);
        var result = await _userManager.ConfirmEmailAsync(userToConfirm, token);
        if (result.Succeeded)
        {
            if (await _userManager.IsInRoleAsync(userToConfirm, "Patient"))
                return View("ConfirmationSuccessedPatient");
            return View("ConfirmationSuccessedDoctor");
        }
        return View("ConfirmationFailed");
    }
    #region Login and Logout
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (ModelState.IsValid)
        {
            ApplicationUser userToLogin = await _userManager.FindByEmailAsync(loginViewModel.Email);
            if (userToLogin is not null)
            {
                return await CheckUserRole(userToLogin, loginViewModel);
            }
        }
        ModelState.TryAddModelError("", "Invalid email or password.");
        return View(loginViewModel);
    }

    private async Task<IActionResult> CheckUserRole(ApplicationUser userToLogin, LoginViewModel loginViewModel)
    {
        if (await _userManager.IsInRoleAsync(userToLogin, "Admin"))
            return await LogUserIn(userToLogin, loginViewModel, "Admin");

        if (!await _userManager.IsEmailConfirmedAsync(userToLogin))
        {
            ModelState.TryAddModelError("", "Email is not confirmed yet, please check your email.");
            return View(loginViewModel);
        }

        if (await _userManager.IsInRoleAsync(userToLogin, "Doctor"))
        {
            Doctor doctorUser = await _unitOfWork.Doctors.FindBy(d => d.ApplicationUserId == userToLogin.Id);
            if (!doctorUser.IsRegistered)
            {
                ModelState.AddModelError("", "Documents are still in verification phase, you will be notified by email once verified.");
                return View(loginViewModel);
            }
            if (doctorUser.IsDeleted)
            {
                return RedirectToAction("ReactivateAccount",userToLogin);
            }
            else
                return await LogUserIn(userToLogin, loginViewModel, "Doctor");
        }

        return await LogUserIn(userToLogin, loginViewModel, "Patient");
    }
    public async Task<IActionResult> ReactivateAccount(ApplicationUser userToLogin)
    {
        Doctor doctor = await _unitOfWork.Doctors.FindBy(d => d.ApplicationUserId == userToLogin.Id);

        ReactivateAccountViewModel reactivateAccountViewModel = new ReactivateAccountViewModel();
        _mapper.GetReactivateAccountViewModel(userToLogin,doctor, reactivateAccountViewModel);


        return View(reactivateAccountViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReactivateAccount(ReactivateAccountViewModel reactivateAccountViewModel)
    {
        Doctor doctor = await _unitOfWork.Doctors.FindBy(d => d.ApplicationUserId == reactivateAccountViewModel.ApplicationUserId, ["Clinics","ApplicationUser"]);
        doctor.IsDeleted = false;

        foreach (var doctorClinic in doctor.Clinics)
        {
            doctorClinic.IsDeleted = false;
        }
        await _unitOfWork.Commit();
         await _signInManager.SignInAsync(doctor.ApplicationUser, isPersistent: false);
         return RedirectToAction("Profile","Doctors");
    }

    private async Task<IActionResult> LogUserIn(ApplicationUser userToLogIn, LoginViewModel loginViewModel, string Role)
    {
        var singinREsult = await _signInManager.PasswordSignInAsync(userToLogIn, loginViewModel.Password, isPersistent: loginViewModel.RememberMe, lockoutOnFailure: true);
        if (singinREsult.Succeeded)
        {
            await _userManager.ResetAccessFailedCountAsync(userToLogIn);

            if (Role == "Admin")
                return RedirectToAction("Home", "Admin");

            if (Role == "Doctor")
                return RedirectToAction("AppointmentIndex", "Doctors");

            return RedirectToAction("Search", "Patient");

        }
        if (singinREsult.IsLockedOut)
        {
            string forgetPassLink = Url.Action("ForgotPassword", "Account", new { }, HttpContext.Request.Scheme);
            string emailBody = $"""Your Etammen account is locked out due to multiple failed login attempts, if it was you and you forgot your password, or if it wasn't you, you might want to reset your password, please<a href="{forgetPassLink}">click here</a> to reset your password.""";

            await _emailService.SendEmailAsync(new Message(userToLogIn.Email, "Ettamen Account is Locked Out", emailBody));

            string lockoutErrorMsg = $"Account is locked out due to multiple failed login attempts, you can try again after {userToLogIn.LockoutEnd.Value.ToLocalTime().ToString("HH:mm")} or you can check your email to reset password.";
            ModelState.AddModelError("", lockoutErrorMsg);
            return View(loginViewModel);
        }
        ModelState.AddModelError("", "Invalid Email or Password.");
        return View(loginViewModel);
    }


    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    #endregion


    #region ResetPassword
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewmodel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewmodel forgotPasswordViewmodel)
    {
        if (forgotPasswordViewmodel.ResetOption == "email")
            return await ResetPasswordViaEmail(forgotPasswordViewmodel);
        return await ResetPasswordViaSMS(forgotPasswordViewmodel);
    }
    private async Task<IActionResult> ResetPasswordViaEmail(ForgotPasswordViewmodel forgotPasswordViewmodel)
    {
        ModelState.Remove("PhoneNumber");
        if (ModelState.IsValid)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(forgotPasswordViewmodel.Email);
            if (user is not null)
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, token = resetToken }, HttpContext.Request.Scheme);
                await _emailService.SendEmailAsync(new Message(user.Email, "Etammen Account Password Reset", $"""To reset your password <a href="{callbackUrl}">Click Here</a>."""));
                return View("ResetPasswordSent");
            }
        }
        ModelState.TryAddModelError("", "Invalid email.");
        return View(forgotPasswordViewmodel);
    }
    public async Task<IActionResult> ResetPassword(string userId, string token)
    {
        if (userId is null || token is null)
            return RedirectToAction("ForgotPassword");
        return View(new ResetPasswordViewModel { Token = token, UserId = userId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
    {
        if (ModelState.IsValid)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(resetPasswordViewModel.UserId);
            if (user is null)
                return RedirectToAction("ForgotPassword");
            var passresetResult = await _userManager.ResetPasswordAsync(user, resetPasswordViewModel.Token, resetPasswordViewModel.Password);
            if (passresetResult.Succeeded)
            {
                user.LockoutEnd = null;
                await _userManager.UpdateAsync(user);
                return View("ResetPasswordSuccess");
            }
            foreach (var error in passresetResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

        }
        return View(new ResetPasswordViewModel { Token = resetPasswordViewModel.Token, UserId = resetPasswordViewModel.UserId });
    }

    private async Task<IActionResult> ResetPasswordViaSMS(ForgotPasswordViewmodel forgotPasswordViewmodel)
    {
        ModelState.Remove("email");
        if (ModelState.IsValid)
        {
            ApplicationUser user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == forgotPasswordViewmodel.PhoneNumber);
            if (user is not null)
            {
                var passwordResetToken = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, "ResetPasswordPurpose");
                string toPhoneNumber = $"+2{user.PhoneNumber}";
                string smsBody = $"Your OTP for resetting your Etammen account password is {passwordResetToken}.";
                MessageResource result = await _smsService.SendSmsAsync(toPhoneNumber, smsBody);

                if (string.IsNullOrEmpty(result.ErrorMessage))
                    return RedirectToAction("ResetPasswordOtp", new { userId = user.Id, token = passwordResetToken });
                else
                {
                    ModelState.TryAddModelError("", "Couldn't send sms to the provided phone number.");
                    return View(forgotPasswordViewmodel);
                }
            }
        }
        ModelState.TryAddModelError("", "Invalid phone number.");
        return View(forgotPasswordViewmodel);
    }

    public async Task<IActionResult> ResetPasswordOtp(string userId, string token)
    {
        return View(new ResetPasswordViewModel { Token = token, UserId = userId });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPasswordOtp(ResetPasswordViewModel resetPasswordViewModel, string Otp)
    {
        if (ModelState.IsValid)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(resetPasswordViewModel.UserId);
            if (user is not null)
            {
                var tokenVerified = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, "ResetPasswordPurpose", Otp);
                if (!tokenVerified)
                {
                    ModelState.AddModelError("", "Invalid OTP.");
                    return View(resetPasswordViewModel);
                }
                var resetpasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, resetpasswordToken, resetPasswordViewModel.Password);
                if (resetResult.Succeeded)
                {
                    user.LockoutEnd = null;
                    await _userManager.UpdateAsync(user);
                    return View("ResetPasswordSuccess");
                }
                foreach (var error in resetResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return RedirectToAction("ForgotPassword");
        }
        return View(resetPasswordViewModel);
    }
    #endregion


    #region External Login Providers

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalLogin(string provider)
    {
        string redirectUrl = Url.Action("ExternalLoginCallBack", "Account");
        AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    public async Task<IActionResult> ExternalLoginCallBack()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info is null)
        {
            ModelState.TryAddModelError("", $"Couldn't login with {info.LoginProvider}.");
            return RedirectToAction("Login");
        }

        var userEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
        ApplicationUser user = await _userManager.FindByEmailAsync(userEmail);

        var signinResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        if (!signinResult.Succeeded)
        {
            if(user is null)
            {
                ExternalLoginViewModel externalLoginViewModel = new ExternalLoginViewModel();
                PopulateExternalLoginVmData(externalLoginViewModel, info);
                return View("ExternalLogin", externalLoginViewModel);
            }
        }
        return await AssociateExternalLoginProviderWithUser(user, info);
    }

    private void PopulateExternalLoginVmData(ExternalLoginViewModel externalLoginViewModel, ExternalLoginInfo info)
    {
        externalLoginViewModel.Provider = info.LoginProvider;
        externalLoginViewModel.GovCitiesDict = _doctorRegisterationHelper.CityAreasDictionary;
    }

    private async Task<IActionResult> AssociateExternalLoginProviderWithUser(ApplicationUser user, ExternalLoginInfo info)
    {
        var addloginResult = await _userManager.AddLoginAsync(user, info);
        await _userManager.AddToRoleAsync(user, "Patient");
        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Search", "Patient");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel externalloginViewModel)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();

        ModelState.Remove("Provider");
        ModelState.Remove("Principal");

        ModelState.Remove("GovCitiesDict");
        if (ModelState.IsValid)
        {
            externalloginViewModel.Principal = info.Principal;

            ApplicationUser newUser = _mapper.GetUserFromExternalLoginViewModel(externalloginViewModel);

            IdentityResult result = await _userManager.CreateAsync(newUser);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "Patient");

                Patient newPatient = _mapper.GetPatientFromExternalLoginViewModel(externalloginViewModel, newUser.Id);
                await _unitOfWork.Patients.Add(newPatient);
                await _unitOfWork.Commit();

                return await AssociateExternalLoginProviderWithUser(newUser, info);
               
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        }
        PopulateExternalLoginVmData(externalloginViewModel, info);
        return View("ExternalLogin", externalloginViewModel);
    }

    #endregion


    public IActionResult AccessDenied()
    {
        return RedirectToAction("StatusCodeError","Error", new { statusCode = 403 });
    }

}


