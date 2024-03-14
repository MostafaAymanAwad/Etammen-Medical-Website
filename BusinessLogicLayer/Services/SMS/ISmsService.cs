using Twilio.Rest.Api.V2010.Account;

namespace BusinessLogicLayer.Services.SMS;

public interface ISmsService
{
     Task<MessageResource> SendSmsAsync(string toMobileNumber, string Body);
}
