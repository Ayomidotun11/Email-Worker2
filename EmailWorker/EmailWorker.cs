using Email_Worker2.DTOs;
using Email_Worker2.DTOs.ConfigDTO;
using Email_Worker2.Services;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email_Worker2.EmailWorker
{
    public class EmailWorker
    {
        private readonly ILogger<EmailWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly WorkerSettings _workerSettings;



        public EmailWorker(
            IServiceProvider serviceProvider,
                IOptions<WorkerSettings> workerSettings,
                ILogger<EmailWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _workerSettings = workerSettings.Value;
            _logger = logger;
        }

        public async Task RunEmailWorkerAsync()
        {
            _logger.LogInformation("Email Service is starting....");
            while (true)
            {
                try
                {
                    await ProcessEmailsAsync();
                    _logger.LogInformation("Email Service has completed processing.");

                    
                    await Task.Delay(TimeSpan.FromMinutes(_workerSettings.IntervalMinutes));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing emails.");
                }
            }
        }

        private async Task ProcessEmailsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var usersService = scope.ServiceProvider.GetRequiredService<IUsersService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var templateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

            try
            {
                // Get users who haven't received emails yet
                var usersToProcess = (await usersService.GetUsersForEmailProcessingAsync())
                    .Take(_workerSettings.BatchSize)
                    .ToList();

                if (!usersToProcess.Any())
                {
                    _logger.LogInformation("No new users to process");
                    return;
                }

                _logger.LogInformation("Found {Count} new users to process", usersToProcess.Count);

                foreach (var user in usersToProcess)
                {
                    {
                        try
                        {
                            string emailBody = await templateService.GetWelcomeEmailTemplateAsync(user.Name);
                            var emailMessage = new EmailMessage(
                                user.Email,
                                "Welcome to Our Dotun's Email Service",
                                emailBody,
                                true);

                            await emailService.SendEmailAsync(emailMessage);
                            await usersService.UpdateUserEmailStatusAsync(user.Id, DateTime.UtcNow);

                            _logger.LogInformation("Successfully sent welcome email to user {UserId} ({Email})",
                                user.Id, user.Email);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to process email for user {UserId} ({Email})",
                                user.Id, user.Email);
                            throw;
                        }
                    };

                    // Add delay between emails to avoid overwhelming the SMTP server

                    await Task.Delay(_workerSettings.DelayBetweenEmailsMs);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during email processing batch");
                throw;
            }

        }




    } 
}
