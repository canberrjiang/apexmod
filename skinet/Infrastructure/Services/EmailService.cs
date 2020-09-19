using Core.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Infrastructure.Services
{
  public class EmailService : IEmailService
  {
    private readonly IConfiguration _config;
    public EmailService(IConfiguration config)
    {
      _config = config;
    }

    public void SendEmail()
    {
      var email = new MimeMessage();
      email.Sender = MailboxAddress.Parse(_config["Email:SenderAddress"]);
      email.To.Add(MailboxAddress.Parse(_config["Email:RecipientAddress"]));
      email.Subject = "You have an new order";
      email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
      {
        Text = "<h1> You have an new order </h1> <p> Please check out the new order </p> "
      };

      // send email
      using var smtp = new SmtpClient();
      smtp.Connect(_config["Email:Host"], 587, MailKit.Security.SecureSocketOptions.StartTls);
      smtp.Authenticate(_config["Email:Username"], _config["Email:Password"]);
      smtp.Send(email);
      smtp.Disconnect(true);
    }
  }
}