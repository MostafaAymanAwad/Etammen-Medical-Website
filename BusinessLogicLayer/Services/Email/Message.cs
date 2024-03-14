using Microsoft.AspNetCore.Http;

namespace Etammen.Services.Email;

public class Message
{
    public Message(string mailTo, string subject, string body, IList<IFormFile> attachments=null)
    {
        MailTo = mailTo;
        Subject = subject;
        Body = body;
        Attachments = attachments;
    }

    public string MailTo { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public IList<IFormFile> Attachments { get; set; }
}
