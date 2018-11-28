using System;
using System.Collections.Generic;
using System.Text;

namespace Dombo.JobScheduler
{
    /// <summary>
    /// JobIdResult model
    /// </summary>
    public class JobIdResult
    {
        public string jobId { get; private set; }

        public JobIdResult(string jobid)
        {
            this.jobId = jobid;
        }
    }
}
