using Dombo.CommonModel;
using Dombo.JobScheduler;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

using Moq;
using System;
using System.Collections.Generic;
using WebAPI.Controllers;
using Xunit;
using static WebAPI.Controllers.ImageController;
using System.Linq;
using Newtonsoft.Json;

namespace ImageControllerTest
{
    /// <summary>
    /// API tests for ImageController
    /// </summary>
    public class ImageControllerTest
    {
        /// <summary>
        /// test for job posting
        /// expected result: receiving a correct JobId after uploading
        /// </summary>
        [Fact]
        public void PostUploadValidateReturnJobId()
        {
            var jobService = new SimpleJobServiceMock();
            var response = new ImageController(jobService, new Mock<IApiService>().Object)
                .PostUpload(
                new UploadParameter()
                {
                    urls = new string[] { }
                });

            
            OkObjectResult result = response.Result as OkObjectResult;
            Assert.Equal(200, result.StatusCode);
            var job = jobService.GetJobs().FirstOrDefault(x => x.JobId.Equals(JsonConvert.DeserializeObject<JobDetails>(result.Value.ToString()).JobId));
            Assert.NotNull(job);
        }

        /// <summary>
        /// test for job posting
        /// expected result: correct content
        /// </summary>
        [Fact]
        public void PostUploadValidateJobContent()
        {
            var jobService = new SimpleJobServiceMock();
            var response = new ImageController(jobService, new Mock<IApiService>().Object)
                .PostUpload(
                new UploadParameter() {
                    urls = new string[] { "http://dummyurl.com" }
                });

            OkObjectResult result = response.Result as OkObjectResult;
            Assert.Equal(200, result.StatusCode);
            var job = jobService.GetJobs().FirstOrDefault(
                x => x.JobId.Equals(JsonConvert.DeserializeObject<JobDetails>(result.Value.ToString()).JobId));
            Assert.Equal(1, job.CommandCollection.Count);
            Assert.Equal(@"http://dummyurl.com", job.CommandCollection[0].Argument);
        }

        /// <summary>
        /// test for get image API
        /// expected result: validate the return result
        /// </summary>
        [Fact]
        public void GetImageTest()
        {
            var jobService = new SimpleJobServiceMock();
            var imageServiceMock = new Mock<IApiService>();
            imageServiceMock.Setup(x => x.GetImage()).Returns("dummy_uploaded_image");

            var response = new ImageController(
                new Mock<JobServiceBase>().Object, imageServiceMock.Object)
                .Get();
            Assert.Equal("dummy_uploaded_image", response.Value);

        }


        /// <summary>
        /// get jobs test API
        /// expected result: receive the jobs status
        /// </summary>
        [Fact]
        public void GetJobTest()
        {
            var jobServiceMock = new SimpleJobServiceMock();

            var job = JobDetails.CreateNew();
            jobServiceMock.Add(job);


            var imageServiceMock = new Mock<IApiService>();
            //imageServiceMock.Setup(x => x.GetImage()).Returns("dummy_uploaded_image");

            Assert.Null(
            JsonConvert.DeserializeObject<JobStatusModel>(
                new ImageController(jobServiceMock, imageServiceMock.Object).GetJob("wrong-job-id").Value).id);

            Assert.Equal(job.JobId,
            JsonConvert.DeserializeObject<JobStatusModel>(
                new ImageController( jobServiceMock, imageServiceMock.Object).GetJob(job.JobId).Value.ToString()).id);



        }


        /// <summary>
        /// a simple mocked job service that keep the a list of jobs
        /// </summary>
        class SimpleJobServiceMock : JobServiceBase
        {
            List<JobDetails> _jobDetails = new List<JobDetails>();
            public override void Add(JobDetails job)
            {
                _jobDetails.Add(job);
            }

            public override ICollection<JobDetails> GetJobs()
            {
                return _jobDetails;
            }

            public override void Start()
            {
                //do nothing
            }

            public override void Stop()
            {
                //do nothing
            }
        }

        /// <summary>
        /// a simple mocked image service
        /// </summary>
        class SimpleImageService : IApiService
        {
            string _imagePath = string.Empty;

            public SimpleImageService()
            {

            }

            public string GetImage()
            {
                return "http://imgur/uploaded/" + _imagePath;
            }

            public ServiceResult UploadImages(string path)
            {
                _imagePath = path;
                return new ServiceResult();
            }
        }

    }
}
