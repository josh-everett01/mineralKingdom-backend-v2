using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace MineralKingdomApi.Services;

public class EmailService : IEmailService
{
  private readonly IConfiguration _config;

  public EmailService(IConfiguration config)
  {
    _config = config;
  }

  public async Task SendVerificationEmailAsync(string toEmail, string token)
  {
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress("Mineral Kingdom", _config["Smtp:FromEmail"]));
    message.To.Add(MailboxAddress.Parse(toEmail));
    message.Subject = "Verify your email";

    var verificationLink = $"http://localhost:5177/auth/verify?token={token}";

    message.Body = new TextPart("plain")
    {
      Text = $"Thanks for signing up to Mineral Kingdom!\n\nPlease verify your email by clicking the link below:\n\n{verificationLink}"
    };

    using var smtp = new SmtpClient();
    int port = int.Parse(_config["Smtp:Port"] ?? "587");
    await smtp.ConnectAsync(_config["Smtp:Host"], port, SecureSocketOptions.StartTls);
    await smtp.AuthenticateAsync(_config["Smtp:Username"], _config["Smtp:Password"]);
    await smtp.SendAsync(message);
    await smtp.DisconnectAsync(true);
  }
}
