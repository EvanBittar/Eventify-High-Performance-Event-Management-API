namespace Eventify_High_Performance_Event_Management_API.Repository
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}