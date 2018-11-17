using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace TestJson
{
    
    public class Task
    {
        [JsonProperty(PropertyName = @"TaskName")]
        public string Name { get; set; }
    }

    public class JobDetails
    {
        [JsonProperty(PropertyName = @"Id")]
        public string JobId { get; set; }

        [JsonProperty(PropertyName = @"Actions")]
        public List<Task> TaskCollection { get; private set; }

        public static JobDetails CreateNewJob() => new JobDetails() {
            JobId = Guid.NewGuid().ToString(),
            TaskCollection = new List<Task>()
        };
    }

    class Program
    {
        static void Main(string[] args)
        {
            var job = JobDetails.CreateNewJob();
            job.TaskCollection.Add(new Task() { Name = "Task 1" });
            job.TaskCollection.Add(new Task() { Name = "Task 2" });

            string json = JsonConvert.SerializeObject(job);
            Console.WriteLine("Hello World!");
        }
    }
}
