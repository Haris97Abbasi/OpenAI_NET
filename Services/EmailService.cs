using OpenAiProject.Models;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace OpenAiProject.Services
{

    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(EmailModel emailModel)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("OpenAI Quiz", emailModel.From));
            message.To.Add(new MailboxAddress("", emailModel.To));
            message.Subject = emailModel.Subject;

            message.Body = new TextPart("plain")
            {
                Text = emailModel.Body
            };

            using (var client = new SmtpClient())
            {
                // Updated client connect configuration for Gmail with TLS
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("YOUR_EMAIL", "PASSWORD"); //Enter your email and password
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}

