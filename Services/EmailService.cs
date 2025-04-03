using Email_Worker2.DTOs;
using Email_Worker2.DTOs.ConfigDTO;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email_Worker2.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage emailMessage);
        
    }
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(EmailMessage emailMessage)
        {
            try
            {
                var message = CreateMimeMessage(emailMessage);
                await SendEmailSetupAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email to {EmailAddress}", emailMessage.To);
                throw;
            }
        }

        private MimeMessage CreateMimeMessage(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(MailboxAddress.Parse(emailMessage.To));
            message.Subject = emailMessage.Subject;

            var builder = new BodyBuilder();
            if (emailMessage.IsHtml)
            {
                builder.HtmlBody = emailMessage.Body;
            }
            else
            {
                builder.TextBody = emailMessage.Body;
            }

            // Add attachments if any
            foreach (var attachment in emailMessage.Attachments)
            {
                builder.Attachments.Add(attachment);
            }

            message.Body = builder.ToMessageBody();
            return message;
        }
        private async Task SendEmailSetupAsync(MimeMessage message)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort,
                    _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                if (!string.IsNullOrEmpty(_emailSettings.SmtpUsername))
                {
                    await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                }

                await client.SendAsync(message);
                _logger.LogInformation("Email sent successfully to {EmailAddress}", message.To);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {EmailAddress}", message.To);
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
