using ArgosSharp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgosSharp.Application.Contracts.JobExecutions
{
    public sealed record CreateJobExecutionRequest
    {
        public int JobId { get; set; }
        public JobParameters Parameters { get; set; }
    }
}
