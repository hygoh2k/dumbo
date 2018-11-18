using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dombo.JobScheduler;
using Dombo.ServiceProvider;
using Dombo.Extension;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    
    [Route("v1/image")]
    [ApiController]
    public class ImageController : ControllerBase
    {

        class ImageControllerUpload
        {
            public string id { get; set; }
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
            if(job != null)
            {
                return job.JobStatus();
            }
            return "Empty";
        }

        [HttpGet]
        [Route("job")]
        public ActionResult<string> GetJobUrl([FromQuery(Name = "id")] string value)
        {
            return GetJob(value);
        }



        // POST v1/images/upload
        [HttpPost]
        [Route("upload")]
        public ActionResult<string> PostUpload( [FromBody] object obj)
        {
            // ActionResult<string> rest = new ActionResult<string>("{no result}");

            var uploadSetting = Newtonsoft.Json.JsonConvert.DeserializeObject<ImageControllerUpload>(obj.ToString());

            var uploadJob = new JobDetails();

            uploadJob.CommandCollection.AddRangeEx(
                        uploadSetting.urls.Select(x => new ImageUploadCommand(_imageServie, x))
                        .ToArray());
            _jobService.Add(uploadJob);

            return Ok(uploadJob.JobId);
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