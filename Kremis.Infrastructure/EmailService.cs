using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kremis.Infrastructure.Contracts;
using Kremis.Utility.Options;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Kremis.Infrastructure
{
    public class EmailService : IEmailSender
    {
        private readonly ILoggerService _logger;
        private readonly EmailOptions _emailSettings;

        public EmailService(ILoggerService logger, IOptions<EmailOptions> emailSettings)
        {
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = CreateEmailMessage(email, subject, htmlMessage);
            return ExecuteAsync(message);
        }
        
        private MimeMessage CreateEmailMessage(string email, string subject, string htmlMessage)
        {
            var to = new List<MailboxAddress>
            {
                new MailboxAddress(email)
            };
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailSettings.Sender));
            emailMessage.To.AddRange(to);
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            return emailMessage;
        }

        private async Task ExecuteAsync(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port.GetValueOrDefault(), true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);

                await client.SendAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
    }
}
