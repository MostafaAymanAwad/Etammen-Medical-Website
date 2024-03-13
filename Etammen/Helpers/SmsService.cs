using DataAccessLayerEF.SettingsModel;
using Etammen.Settings;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Etammen.Helpers
{
    public class SmsService:ISmsService
    {
        private readonly TwilioSettings _options;
        public SmsService(IOptions<TwilioSettings> options)
        {
            _options = options.Value;
        }

        public MessageResource Send(SMSMessage sms)
        {
            TwilioClient.Init(_options.AccountSID, _options.AuthToken);
            var result = MessageResource.Create(
                body: sms.body,
                from: new Twilio.Types.PhoneNumber(_options.TwilioPhoneNumber),
                to:sms.PhoneNumber
                );
            return result;

        }
    }
}
