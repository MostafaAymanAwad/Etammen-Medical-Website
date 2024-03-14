using Etammen.Services.ServicesConfigurations;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace BusinessLogicLayer.Services.SMS;

public class SmsService : ISmsService
{
    private readonly SmsConfiguration _smsConfiguration;

    public SmsService(IOptions<SmsConfiguration> smsConfiguration)
    {
        _smsConfiguration = smsConfiguration.Value;
    }

    public async Task<MessageResource> SendSmsAsync(string toMobileNumber, string Body)
    {
        TwilioClient.Init(_smsConfiguration.AccountSID, _smsConfiguration.Authtoken);
        MessageResource messageResource = await MessageResource.CreateAsync(
                to: toMobileNumber,
                body: Body,
                from: new Twilio.Types.PhoneNumber(_smsConfiguration.TwillioPhoneNumber)
            );
        return messageResource;
    }
}
