
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dombo.CommonModel;
using Newtonsoft;


namespace Dombo.JobScheduler
{
    public class JobResult : ICommandResult
    {
        public string StatusCode { get; set; }
        public object Result { get; set; }

        public JobResult()
        {
            StatusCode = "Not Executed";
            Result = null;
        }
    }

    public class JobDetails
    {
        /// <summary>
        /// dont use this, json serializer will call this for new object
        /// </summary>
        //public JobDetails()
        //{
        //    JobId = Guid.NewGuid().ToString();
        //    CreatedDateTime = DateTime.UtcNow;
        //}
        

        public static JobDetails CreateNew()
        {
            return new JobDetails() { JobId = Guid.NewGuid().ToString(), CreatedDateTime = DateTime.UtcNow };
        }

        public string JobId { get;  set; }

        public DateTime CreatedDateTime { get;  set; }
        public DateTime FinishedDateTime { get;  set; }

        List<ICommand> _commandCollection = new List<ICommand>();

        public IList<ICommand> CommandCollection => _commandCollection;

        private ConcurrentDictionary<ICommand, ICommandResult> _jobStatus = new ConcurrentDictionary<ICommand, ICommandResult>();

        private volatile bool _jobStarted = false;


        public void Execute()
        {
            _jobStarted = true;

            _commandCollection.ForEach(x => _jobStatus.TryAdd(x, new JobResult() ));

            foreach (var cmd in CommandCollection)
            {
                _jobStatus[cmd] = cmd.Run(); //update the status
            }

            FinishedDateTime = DateTime.UtcNow;
        }

        public bool IsJobStarted()
        {
            return _jobStarted;
        }

            

        public KeyValuePair<ICommand, ICommandResult>[] JobStatus()
        {
            return  _jobStatus.ToArray(); //create a snapshot and return

        }

        public string GetJobId()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new JobIdResult(JobId));
        }

    }

    


    public abstract class JobServiceBase
    {
        public abstract void Start();
        public abstract void Stop();
        public abstract void Add(JobDetails job);
        public abstract ICollection<JobDetails> GetJobs();

    }



    public class JobService : JobServiceBase
    {
        ConcurrentBag<JobDetails> _jobCollection;
        ConcurrentStack<string> _pendingJobCollection;



        public JobService(bool startService)
        {
            _jobCollection = new ConcurrentBag<JobDetails>();
            _pendingJobCollection = new ConcurrentStack<string>();

            if (startService)
                Start();//start the service !
        }

        public override void Add(JobDetails job)
        {
            if (! _jobCollection.Any(x => x.JobId.Equals(job.JobId))) //add when job id is unique
            {
                _jobCollection.Add(job);
                _pendingJobCollection.Push(job.JobId);
            }
        }


        async void RunAsync()
        {
            await Task.Run(() => NotSyncFync());
        }


        public volatile bool Running;

        private void NotSyncFync()
        {
            while (Running)
            {
                //execute job
                string jobId = null;
                if (_pendingJobCollection.TryPop(out jobId))
                {
                    _jobCollection.Single(x => x.JobId == jobId).Execute();

                }
            }
        }

        public override void Start()
        {
            Running = true;
            RunAsync();

        }

        public override void Stop()
        {
            Running = false;
        }

        public override ICollection<JobDetails> GetJobs() => _jobCollection.ToList();
    }

    //public class TaskRepository
    //{
    //    private readonly ICollection<JobDetails> _jobCollection;

    //    public ICollection<JobDetails> JobCollection => _jobCollection;

    //    public void Add(JobDetails job) => _jobCollection.Add(job);



    //    public bool Execute(string jobId)
    //    {
    //        JobDetails job = _jobCollection.First(x => x.JobId.Equals(jobId));
    //        if(job!=null)
    //        {
    //            //execute the euqry
    //        }

    //        return false;
    //    }


    //}

}
