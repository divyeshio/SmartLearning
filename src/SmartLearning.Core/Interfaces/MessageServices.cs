using System.Net.Mail;

namespace SmartLearning.Core.Interfaces
{
  public class AuthMessageSender : IEmailSender, ISmsSender
  {
    public Task SendEmailAsync(string email, string subject, string message)
    {
      MailMessage mail = new();
      SmtpClient SmtpServer = new("smtp.gmail.com");
      mail.From = new MailAddress("smartlearningsurat@gmail.com");
      mail.To.Add(email);
      mail.Subject = subject;
      mail.Body = message;


      SmtpServer.Port = 587;
      SmtpServer.Credentials = new System.Net.NetworkCredential("smartlearningsurat@gmail.com", "notyourtype");
      SmtpServer.EnableSsl = true;

      SmtpServer.Send(mail);


      return Task.FromResult(0);
    }

    public Task SendSmsAsync(string number, string message)
    {
      return Task.FromResult(0);
    }

    Task IEmailSender.SendEmailAsync(string to, string from, string subject, string body)
    {
      throw new NotImplementedException();
    }
  }
}
