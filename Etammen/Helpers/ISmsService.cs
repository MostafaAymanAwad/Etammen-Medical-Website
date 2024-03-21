using DataAccessLayerEF.SettingsModel;
using Twilio.Rest.Api.V2010.Account;

namespace Etammen.Helpers
{
    public interface ISmsService
    {
        MessageResource Send(SMSMessage sms);
    }
}
