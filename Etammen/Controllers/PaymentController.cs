using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services.ServicesConfigurations;
using DataAccessLayerEF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;
namespace Etammen.Controllers;
public class PaymentController : Controller
{
    private readonly StripeSettings _stripeConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentController(IOptions<StripeSettings> stripeConfiguration, IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _stripeConfiguration = stripeConfiguration.Value;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }
    public IActionResult CheckoutSession(decimal fees, string clinicName, string appointmentId)
    {
        string currencyCode = "usd";
        string successUrl = GetAbsoluteUrl($"Success");
        string cancelUrl = GetAbsoluteUrl("Cancel");
        TempData["clinicName"] = clinicName;
        TempData["appointmentId"] = appointmentId;

        Stripe.StripeConfiguration.ApiKey = _stripeConfiguration.Secretkey;

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string>
            {
                "card"
            },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = currencyCode,
                        UnitAmount = Convert.ToInt32(fees) * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = clinicName
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl
        };
        var service = new SessionService();
        var session = service.Create(options);
        TempData["SessionId"] = session.Id;
        return Redirect(session.Url);
    }
    private string GetAbsoluteUrl(string actionName)
    {
        var uriBuilder = new UriBuilder
        {
            Scheme = _httpContextAccessor.HttpContext.Request.Scheme,
            Host = _httpContextAccessor.HttpContext.Request.Host.Host,
            Port = _httpContextAccessor.HttpContext.Request.Host.Port ?? (_httpContextAccessor.HttpContext.Request.IsHttps ? 443 : 80),
            Path = Url.Action(actionName, "Payment")
        };
        return uriBuilder.Uri.ToString();
    }

    public async Task<IActionResult> RefundPayment(int appointmentId)
    {
        Stripe.StripeConfiguration.ApiKey = _stripeConfiguration.Secretkey;

        string AppointmentPaymentIntent = string.Empty;

        ClinicAppointment clinicAppointment = await _unitOfWork.ClinicAppointments.FindBy(p => p.Id == appointmentId);
        HomeAppointment homeAppointment = await _unitOfWork.HomeAppointment.FindBy(p => p.Id == appointmentId);

        if (clinicAppointment is not null)
        {
            AppointmentPaymentIntent = clinicAppointment.PaymentIntentId;
        }
        else
        {
            AppointmentPaymentIntent = homeAppointment.PaymentIntentId;
        }
        var refundOptions = new RefundCreateOptions
        {
            PaymentIntent = AppointmentPaymentIntent
        };
        var refundService = new RefundService();
        var refund = await refundService.CreateAsync(refundOptions);
        return View("PaymentRefunded");
    }

    public async Task<IActionResult> Success()
    {
        string clinicName = TempData["clinicName"].ToString();
        int appointmentId = Convert.ToInt32(TempData["appointmentId"]);

        string sessionId = TempData["SessionId"].ToString();
        Session session = new SessionService().Get(sessionId);



        string paymentIntentId = session.PaymentIntentId;

        string currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (clinicName == "HomeVisit")
        {
            HomeAppointment homeAppointment = await _unitOfWork.HomeAppointment.FindBy(c => c.Id == appointmentId);
            homeAppointment.PaymentIntentId = paymentIntentId;
            homeAppointment.IsPaidOnline = true;
            _unitOfWork.HomeAppointment.Update(homeAppointment);
            await _unitOfWork.Commit();
        }
        else
        {
            ClinicAppointment appointment = await _unitOfWork.ClinicAppointments.FindBy(c => c.Id == appointmentId);
            appointment.PaymentIntentId = paymentIntentId;
            appointment.IsPaidOnline = true;
            _unitOfWork.ClinicAppointments.Update(appointment);
            await _unitOfWork.Commit();
        }

        return View("Success");
    }

    public IActionResult Cancel()
    {
        return View("Cancel");
    }
}

