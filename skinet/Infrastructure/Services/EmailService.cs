using Core.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace Infrastructure.Services
{
  public class EmailService : IEmailService
  {
    public void SendEmail()
    {
      var email = new MimeMessage();
      email.Sender = MailboxAddress.Parse("jiangliyangfan@gmail.com");
      email.To.Add(MailboxAddress.Parse("canberrajiang@hotmail.com"));
      email.Subject = "You have an new order";
      email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
      {
        Text = "<h1> You have an new order </h1> <p> Please check out the new order </p> "
      };

      // send email
      using var smtp = new SmtpClient();
      smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
      smtp.Authenticate("[username]", "[password]");
      smtp.Send(email);
      smtp.Disconnect(true);
    }
  }
}