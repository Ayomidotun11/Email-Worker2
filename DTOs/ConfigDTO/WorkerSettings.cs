using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Email_Worker2.DTOs.ConfigDTO
{
    public class WorkerSettings
    {
        public int IntervalMinutes { get; set; } = 20;
        public int BatchSize { get; set; } = 10; 
        public int DelayBetweenEmailsMs { get; set; } = 2000; 
    }
}
