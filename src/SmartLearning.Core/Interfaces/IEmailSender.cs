namespace SmartLearning.Core.Interfaces
{
  public interface IEmailSender
  {
    Task SendEmailAsync(string email, string subject, string message);
    Task SendEmailAsync(string to, string from, string subject, string body);
  }
}
