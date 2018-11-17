
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
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
        public JobDetails() => JobId = Guid.NewGuid().ToString();

        public string JobId { get; private set; }

        List<ICommand> _commandCollection = new List<ICommand>();

        public IList<ICommand> CommandCollection
        {
            get
            {
                return _commandCollection;
            }
        }

        private ConcurrentDictionary<ICommand, ICommandResult> _jobStatus = new ConcurrentDictionary<ICommand, ICommandResult>();


        public void Execute()
        {
            ////////_jobStatus = new ConcurrentDictionary<ICommand, ICommandResult>();
            _commandCollection.ForEach(x => _jobStatus.TryAdd(x, new JobResult() ));

            foreach (var cmd in CommandCollection)
            {
                _jobStatus[cmd] = cmd.Run(); //update the status
            }
        }

        public string JobStatus()
        {
            var jobSnapshot = _jobStatus.ToList(); //create a snapshot

            List<KeyValuePair<ICommand, ICommandResult>> status = new List<KeyValuePair<ICommand, ICommandResult>>();

            foreach(var item in _commandCollection)
            {
                if( jobSnapshot.Any(x => x.Key == item))
                {
                    status.Add(jobSnapshot.FirstOrDefault(x => x.Key == item));
                }
                else
                {
                    status.Add(new KeyValuePair<ICommand, ICommandResult>(item, new JobResult()));
                }
            }


            return Newtonsoft.Json.JsonConvert.SerializeObject(status);

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



        public JobService()
        {
            _jobCollection = new ConcurrentBag<JobDetails>();
            _pendingJobCollection = new ConcurrentStack<string>();
            Start();//start the service !
        }

        public override void Add(JobDetails job)
        {
            _jobCollection.Add(job);
            _pendingJobCollection.Push(job.JobId);
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
