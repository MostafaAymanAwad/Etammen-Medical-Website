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
    public string SessionId { get; set; }
    private readonly StripeSettings _stripeConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    public string PaymentIntentId {  get; set; }

    public PaymentController(IOptions<StripeSettings> stripeConfiguration, IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
    {
        _stripeConfiguration = stripeConfiguration.Value;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
    }
    public IActionResult CheckoutSession(int fees, string clinicName, string appoitnmentId)
    {
        string currencyCode = "usd";
        string successUrl = GetAbsoluteUrl($"Success/{clinicName}/{appoitnmentId}");
        string cancelUrl = GetAbsoluteUrl("Cancel");
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
                        UnitAmount = fees * 100,
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
        SessionId = session.Id;
        PaymentIntentId = session.PaymentIntentId;

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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RefundPayment(string paymentIntentId)
    {
         
         var refundOptions = new RefundCreateOptions
         {
             PaymentIntent = paymentIntentId
         };

         var refundService = new RefundService();
         var refund = await refundService.CreateAsync(refundOptions);
         return View("PaymentRefunded");
    }

    public async Task<IActionResult> Success(string clinicName, string appointmnetId)
    {
        //string currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        //if(clinicName == "HomeVisit")
        //{
        //    HomeAppointment homeAppointment = _unitOfWork.HomeAppointments.FirstOrDefault(c => c.Id == appointmnetId);
        //    homeAppointment.PaymentIntentId = PaymentIntentId;
        //    homeAppointment.IsPaidOnline = true;
        //    _unitOfWork.HomeAppointments.Update(homeAppointment);
        //}
        //else
        //{
        //    Appointment appointment = _unitOfWork.Appointments.FirstOrDefault(c => c.Id == appointmnetId);
        //    appointment.PaymentIntentId = PaymentIntentId;
        //    appointment.IsPaidOnline = true;
        //    _unitOfWork.Appointments.Update(appointment);
        //}
        //return View("success");
        throw new NotImplementedException();
    }


    public IActionResult Cancel()
    {
        return View("Cancel");
    }
}

