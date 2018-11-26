using System;
using Xunit;
using Dombo.JobScheduler;
using System.Linq;
using Dombo.CommonModel;
using System.Threading;

namespace ServiceTest
{
    /// <summary>
    /// this testclass contains tests for JobScheduler functionality
    /// </summary>
    public class JobSchedulerTest
    {

        /// <summary>
        /// a simple start test by setting the constructor to true
        /// </summary>
        [Fact]
        public void SimpleStartTest()
        {
            JobService service = new JobService(true);//start with constructor
            Assert.True(service.Running);//check the service status is running
            service.Stop();
        }


        /// <summary>
        /// simple stat and stop test
        /// </summary>
        [Fact]
        public void StartStopTest()
        {
            JobService service = new JobService(false);
            service.Start(); 
            Assert.True(service.Running);//check the service status is running
            service.Stop();
            Assert.True(true);//make sure the job can be stop
        }

        /// <summary>
        /// test the job adding
        /// </summary>
        [Fact]
        public void BasicAddJobTest()
        {
            JobService service = new JobService(false);
            JobDetails job1 = JobDetails.CreateNew();
            JobDetails job2 = JobDetails.CreateNew();
            JobDetails job3 = JobDetails.CreateNew();
            service.Add(job1);
            service.Add(job2);
            service.Add(job3);
            var jobCollection = service.GetJobs();
            Assert.Equal(3, jobCollection.Count);
            Assert.NotNull(jobCollection.FirstOrDefault(x => x.JobId == job1.JobId));
            Assert.NotNull(jobCollection.FirstOrDefault(x => x.JobId == job1.JobId));
            Assert.NotNull(jobCollection.FirstOrDefault(x => x.JobId == job1.JobId));
            Assert.Null(jobCollection.FirstOrDefault(x => x.JobId == "invalid_job_id"));
        }

        /// <summary>
        /// test when  duplicated job instance is added
        /// expected result: only keep the jobs with unique jobid
        /// </summary>
        [Fact]
        public void AddDuplicateJobInstanceTest()
        {
            JobService service = new JobService(false);
            JobDetails job1 = JobDetails.CreateNew();
            JobDetails job2 = JobDetails.CreateNew();
            JobDetails job3 = JobDetails.CreateNew();
            service.Add(job1);
            service.Add(job1);
            service.Add(job1);
            var jobCollection = service.GetJobs();
            Assert.Equal(1, jobCollection.Count);
            Assert.NotNull(jobCollection.FirstOrDefault(x => x.JobId == job1.JobId));
        }


        /// <summary>
        /// test when jobs with same jobid is added
        /// expected result: only keep the jobs with unique jobid
        /// </summary>
        [Fact]
        public void AddDuplicateJobIdTest()
        {
            JobService service = new JobService(false);
            JobDetails job1 = JobDetails.CreateNew();
            JobDetails job2 = JobDetails.CreateNew();
            JobDetails job3 = JobDetails.CreateNew();
            job2.JobId = job1.JobId;
            job3.JobId = job1.JobId;
            service.Add(job1);
            service.Add(job2);
            service.Add(job3);
            var jobCollection = service.GetJobs();
            Assert.Equal(1, jobCollection.Count);
            Assert.NotNull(jobCollection.FirstOrDefault(x => x.JobId == job1.JobId));
        }


        /// <summary>
        /// test if when the service is started, job will be run
        /// expected result: job status should be started
        /// </summary>
        [Fact]
        public void BasicRunTest()
        {
            JobService service = new JobService(false);
            JobDetails job1 = JobDetails.CreateNew();
            SimpleSplitArgumentMockCommand cmd1 = new SimpleSplitArgumentMockCommand() { Argument = "" };
            job1.CommandCollection.Add(cmd1);

            service.Add(job1);

            Assert.False(cmd1.IsExecuted); //cmd not executed
            Assert.False(job1.IsJobStarted()); //job not started

            service.Start();


            while (cmd1.IsExecuted == false)
            {
                //do nothing
            }

            Assert.True(cmd1.IsExecuted); //cmd not executed
            Assert.True(job1.IsJobStarted()); //job not started
            service.Stop();
        }


        /// <summary>
        /// test if when the service completed, the cmd will be executed
        /// the job returns correct result
        /// </summary>
        [Fact]
        public void RunAndCheckJobStatusTest()
        {
            JobService service = new JobService(false);
            JobDetails job1 = JobDetails.CreateNew();
            SimpleSplitArgumentMockCommand cmd1 = new SimpleSplitArgumentMockCommand() { Argument = "arg1 arg2 arg3" };
            job1.CommandCollection.Add(cmd1);
            service.Add(job1);
            service.Start();


            while (cmd1.IsExecuted == false)
            {
                //do nothing
            }

            Assert.True(cmd1.IsExecuted); //cmd not executed
            Assert.True(job1.IsJobStarted()); //job not started
            service.Stop();

            var status = job1.JobStatus();
            Assert.Single(status);
            Assert.Equal("Successful", status[0].Value.StatusCode);
            var result = status[0].Value.Result as string[];
            Assert.NotNull(result);
            Assert.Equal("arg1", result[0]);
            Assert.Equal("arg2", result[1]);
            Assert.Equal("arg3", result[2]);
        }



        //simple mocking of the command, when executed, it will split string to array
        class SimpleSplitArgumentMockCommand : ICommand
        {
            public string Argument { get; set; }
            public volatile bool IsExecuted = false;
            public ICommandResult Run()
            {
                IsExecuted = true;
                return new SimpleMockCommandResult()
                {
                    StatusCode = "Successful",
                    Result = Argument.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                };
            }


        }

        //simple mocking of the command result
        class SimpleMockCommandResult : ICommandResult
        {
            public string StatusCode { get; set; }
            public object Result { get; set; }
        }


    }
}
