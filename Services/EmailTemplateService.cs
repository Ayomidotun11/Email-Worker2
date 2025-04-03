using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email_Worker2.Services
{

    public interface IEmailTemplateService
    {
        Task<string> GetWelcomeEmailTemplateAsync(string userName);
    }
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly ILogger<EmailTemplateService> _logger;
        private const string TemplatesPath = "Templates";

        public EmailTemplateService(ILogger<EmailTemplateService> logger)
        {
            _logger = logger;
        }
        public async Task<string> GetWelcomeEmailTemplateAsync(string userName)
        {
            try
            {
                string templatePath = Path.Combine(TemplatesPath, "WelcomeEmail.html");
                string template = await File.ReadAllTextAsync(templatePath);
                return template.Replace("{UserName}", userName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading welcome email template");
                throw;
            }
        }
    }
}
