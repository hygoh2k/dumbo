
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dombo.CommonModel;


namespace Dombo.JobScheduler
{
    /// <summary>
    /// Data Model of JobCommandResult
    /// </summary>
    public class JobCommandResult : ICommandResult
    {
        public string StatusCode { get; set; }
        public object Result { get; set; }

        public JobCommandResult()
        {
            StatusCode = "Not Executed";
            Result = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class JobDetails
    {
        List<ICommand> _commandCollection = new List<ICommand>();
        private ConcurrentDictionary<ICommand, ICommandResult> _jobStatus = new ConcurrentDictionary<ICommand, ICommandResult>();
        private volatile bool _jobStarted = false;

        /// <summary>
        /// create a new instance of JobDetails
        /// </summary>
        /// <returns>new instance of JobDetails</returns>
        public static JobDetails CreateNew()
        {
            return new JobDetails() { JobId = Guid.NewGuid().ToString(), CreatedDateTime = DateTime.UtcNow };
        }

        /// <summary>
        /// get set the JobId
        /// </summary>
        public string JobId { get;  set; }

        /// <summary>
        /// get set the job created date time
        /// </summary>
        public DateTime CreatedDateTime { get;  set; }


        /// <summary>
        /// get set the job finished date time
        /// </summary>
        public DateTime FinishedDateTime { get;  set; }

        /// <summary>
        /// get set the collection of the command
        /// </summary>
        public IList<ICommand> CommandCollection => _commandCollection;

        /// <summary>
        /// get if the job is started
        /// </summary>
        /// <returns></returns>
        public bool IsJobStarted() => _jobStarted;

        /// <summary>
        /// get the list of job status
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<ICommand, ICommandResult>[] JobStatus() => _jobStatus.ToArray(); //create a snapshot and return

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetJobId() => Newtonsoft.Json.JsonConvert.SerializeObject(new JobIdResult(JobId));


        public void Execute()
        {
            _jobStarted = true;

            _commandCollection.ForEach(x => _jobStatus.TryAdd(x, new JobCommandResult() ));

            foreach (var cmd in CommandCollection)
            {
                _jobStatus[cmd] = cmd.Run(); //update the status
            }

            FinishedDateTime = DateTime.UtcNow;
        }
    }



    /// <summary>
    ///  the abstract model of job scheduler service
    /// </summary>
    public abstract class JobServiceBase
    {
        /// <summary>
        /// starts this service
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// stops the service
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// add a job to the service
        /// </summary>
        /// <param name="job"></param>
        public abstract void Add(JobDetails job);

        /// <summary>
        /// get a collection of jobs from the service
        /// </summary>
        /// <returns>a collection of JobDetails</returns>
        public abstract ICollection<JobDetails> GetJobs();

    }


    /// <summary>
    /// the simple implementation of job scheduler service
    /// </summary>
    public class JobService : JobServiceBase
    {
        ConcurrentBag<JobDetails> _jobCollection; //collection of jobs
        ConcurrentStack<string> _pendingJobCollection; //collection of pending jobs


        /// <summary>
        /// job service constructor
        /// </summary>
        /// <param name="startService"> true: service will start when this constructor is called </param>
        public JobService(bool startService)
        {
            _jobCollection = new ConcurrentBag<JobDetails>();
            _pendingJobCollection = new ConcurrentStack<string>();

            if (startService)
                Start();//start the service !
        }

        /// <summary>
        /// add a job to the service queue
        /// </summary>
        /// <param name="job"></param>
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

        /// <summary>
        /// start the job scheduler service
        /// </summary>
        public override void Start()
        {
            Running = true;
            RunAsync();

        }


        /// <summary>
        /// stop the job scheduler service
        /// </summary>
        public override void Stop() => Running = false;


        /// <summary>
        /// get all jobs in the queue
        /// </summary>
        /// <returns></returns>
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
