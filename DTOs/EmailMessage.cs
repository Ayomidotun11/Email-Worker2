using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email_Worker2.DTOs
{
    public class EmailMessage
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public List<string> Attachments { get; set; } = new List<string>();

        public EmailMessage(string to, string subject, string body, bool isHtml = true)
        {
            To = to;
            Subject = subject;
            Body = body;
            IsHtml = isHtml;
        }
    }

}
