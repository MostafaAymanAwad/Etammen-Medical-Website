using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Security;
using Etammen.Services.ServicesConfigurations;
using Microsoft.AspNetCore.Http;

namespace Etammen.Services.Email;
public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailConfiguration;

    public EmailService(IOptions<EmailConfiguration> emailConfiguration)
    {
        _emailConfiguration = emailConfiguration.Value;
    }

    public async Task SendEmailAsync(Message message)
    {
        var Email = new MimeMessage()
        {
            Sender = MailboxAddress.Parse(_emailConfiguration.From),
            Subject = message.Subject
        };
        Email.To.Add(MailboxAddress.Parse(message.MailTo)); 

        var builder = new BodyBuilder();
        if(message.Attachments is not null)
        {
            byte[] fileBytes;
            foreach(IFormFile attachment in message.Attachments) 
            {
                if(attachment.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        attachment.CopyTo(memoryStream);
                        fileBytes = memoryStream.ToArray();
                        builder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                    }
                }
            }
        }
        builder.HtmlBody = message.Body;
        Email.Body = builder.ToMessageBody();
        Email.From.Add(new MailboxAddress(_emailConfiguration.Username, _emailConfiguration.From));

        using (var smtp = new SmtpClient()) 
        {
            smtp.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.Port, SecureSocketOptions.StartTls); 
            smtp.Authenticate(_emailConfiguration.From, _emailConfiguration.Password);
            await smtp.SendAsync(Email);
            smtp.Disconnect(true);
        }
    }
}
