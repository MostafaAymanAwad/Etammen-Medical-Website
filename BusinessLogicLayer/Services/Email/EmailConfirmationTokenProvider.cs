using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Etammen.Services.Email;
public class EmailConfirmationTokenProvider<Tuser> : DataProtectorTokenProvider<Tuser> where Tuser : class
{
    public EmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider, IOptions<EmailConfirmationTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<Tuser>> logger) : base(dataProtectionProvider, options, logger)
    {
    }
}
public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions
{

}
