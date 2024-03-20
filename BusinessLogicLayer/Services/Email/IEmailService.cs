namespace Etammen.Services.Email;

public interface IEmailService
{
    Task SendEmailAsync(Message message);
}
