using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dombo.JobScheduler;
using Dombo.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dombo.CommonModel;

namespace WebAPI.Controllers
{
    public class ImageItem
    {
        public string link { get; set; }
    }

    public class UploadedImageNew
    {
        public ImageItem data { get; set; }
        public bool success { get; set; }
        public int status { get; set; }
    }

    public class JobStatusModel
    {
        public string id { get; set; }
        public string created { get; set; }
        public string finished { get; set; }
        public string status { get; set; }
        public JobStatusContent uploaded { get; set; }

    }

    public enum JobStatusType
    {
        PENDING, COMPLETE, FAILED
    }

    public class JobStatusContent
    {
        public string[] pending { get { return _jobStatus[JobStatusType.PENDING].ToArray(); } }
        public string[] complete { get { return _jobStatus[JobStatusType.COMPLETE].ToArray(); } }
        public string[] failed { get { return _jobStatus[JobStatusType.FAILED].ToArray(); } }

        Dictionary<JobStatusType, List<string>> _jobStatus = new Dictionary<JobStatusType, List<string>>();

        public JobStatusContent()
        {
            _jobStatus.Add(JobStatusType.PENDING, new List<string>());
            _jobStatus.Add(JobStatusType.COMPLETE, new List<string>());
            _jobStatus.Add(JobStatusType.FAILED, new List<string>());
        }

        public void AddJob(JobStatusType jobType, string url)
        {
            _jobStatus[jobType].Add(url);
        }
    }


    [Route("v1/image")]
    [ApiController]
    public class ImageController : ControllerBase
    {

        class ImageControllerUpload
        {
            public string[] urls { get; set; }
        }


        JobServiceBase _jobService;
        IApiService _imageServie;
        public ImageController(JobServiceBase jobService, IApiService imageService)
        {
            _jobService = jobService;
            _imageServie = imageService;
        }
        
        //GET v1/image
        [HttpGet]
        public ActionResult<string> Get()
        {
            var jobList = _jobService.GetJobs();
            return _imageServie.GetImage();
        }

        //GET v1/image/job/<job_id>
        [HttpGet]
        [Route("job/{id}")]
        public ActionResult<string> GetJob(string id)
        {
            var job = _jobService.GetJobs().SingleOrDefault(x => x.JobId == id);
            JobStatusModel jobStatus = new JobStatusModel()
            {

                uploaded = new JobStatusContent()

            };

            if (job != null)
            {
                jobStatus.created = job.CreatedDateTime.ToString("O");
                jobStatus.id = job.JobId;
                var jobStatusList = job.JobStatus();
                foreach (var item in jobStatusList)
                {
                    
                    if (item.Value != null && item.Value.Result != null)
                    {
                        UploadedImageNew imageData = Newtonsoft.Json.JsonConvert
                                                        .DeserializeObject<UploadedImageNew>(((ServiceResult)item.Value.Result).Result.ToString());

                        if (imageData.data.link != null)
                            jobStatus.uploaded.AddJob(JobStatusType.COMPLETE, imageData.data.link);
                        else
                            jobStatus.uploaded.AddJob(JobStatusType.FAILED, item.Key.ArgumentCollection);
                    }
                    else
                    {
                        jobStatus.uploaded.AddJob(JobStatusType.PENDING, item.Key.ArgumentCollection);
                    }

                }

                //if(jobStatus.uploaded.complete.Length == job.JobStatus().Length)
                jobStatus.status = (job.IsJobStarted() == false) ?
                    "pending" : ((jobStatus.uploaded.pending.Length == 0) ? "completed" : "in-progress");

                jobStatus.finished = jobStatus.status.Equals("completed") ? job.FinishedDateTime.ToString("O") : null;
            }



            return Newtonsoft.Json.JsonConvert.SerializeObject(jobStatus);
        }

        [HttpGet]
        [Route("job")]
        public ActionResult<string> GetJobUrl([FromQuery(Name = "id")] string value)
        {
            return GetJob(value);
        }

        [HttpGet]
        [Route("upload/{id}")]
        public ActionResult<string> GetJobUrl2(string id)
        {
            return GetJob(id);
        }




        // POST v1/images/upload
        [HttpPost]
        [Route("upload")]
        public ActionResult<string> PostUpload( [FromBody] object obj)
        {

            var uploadSetting = Newtonsoft.Json.JsonConvert.DeserializeObject<ImageControllerUpload>(obj.ToString());

            var uploadJob = new JobDetails();

            uploadJob.CommandCollection.AddRangeEx(
                        uploadSetting.urls.Distinct()
                        .Select(x => new ImageUploadCommand(_imageServie, x))
                        .ToArray());
            _jobService.Add(uploadJob);

            return Ok(uploadJob.GetJobId());
        }



        // POST v1/images/upload
        [HttpPost]
        [Route("stop")]
        public ActionResult<string> PostStop([FromBody] object obj)
        {
            _jobService.Stop();

            return Ok();
        }
    }
}