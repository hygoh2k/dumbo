using Dombo.CommonModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dombo.ServiceProvider.ImageService
{
    //public class JobStatusModel
    //{
    //    public string id { get; set; }
    //    public string created { get; set; }
    //    public string finished { get; set; }
    //    public string status { get; set; }
    //    public JobStatusContent uploaded { get; set; }

    //}

    //public enum JobStatusType
    //{
    //    PENDING, COMPLETE, FAILED
    //}

    //public class JobStatusContent
    //{
    //    public string[] pending { get { return _jobStatus[JobStatusType.PENDING].ToArray(); } }
    //    public string[] complete { get { return _jobStatus[JobStatusType.COMPLETE].ToArray(); } }
    //    public string[] failed { get { return _jobStatus[JobStatusType.FAILED].ToArray(); } }

    //    Dictionary<JobStatusType, List<string>> _jobStatus = new Dictionary<JobStatusType, List<string>>();

    //    public JobStatusContent()
    //    {
    //        _jobStatus.Add(JobStatusType.PENDING, new List<string>());
    //        _jobStatus.Add(JobStatusType.COMPLETE, new List<string>());
    //        _jobStatus.Add(JobStatusType.FAILED, new List<string>());
    //    }

    //    public void AddJob(JobStatusType jobType, string url)
    //    {
    //        _jobStatus[jobType].Add(url);
    //    }
    //}

    //public class ImageUploadJobStatusResult : ISerializableResult
    //{
    //    public string SerializeObject(object jsonObject)
    //    {

    //        var listObj = jsonObject as List<KeyValuePair<ICommand, ICommandResult>>;
    //        JobStatusContent content = new JobStatusContent();
    //        foreach (var item in listObj)
    //        {
    //            var sourceUrl = item.Key.ArgumentCollection;
    //            var uploadResult = item.Value.Result;

    //            if (uploadResult != null && uploadResult is ServiceResult)
    //            {
    //                //executed
    //                var itemResult = Newtonsoft.Json.JsonConvert.DeserializeObject<UploadedImageNew>(((ServiceResult)uploadResult).Result.ToString());
    //                content.AddJob(JobStatusType.COMPLETE, itemResult.data.link);
    //            }
    //            else
    //            {
    //                //not yet executed
    //                content.AddJob(JobStatusType.PENDING, sourceUrl);
    //            }
    //        }


    //        return Newtonsoft.Json.JsonConvert.SerializeObject(content);
    //    }
    //}

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


    public class UploadedImage
    {
        public ImageItem[] data { get; set; }
        public bool success { get; set; }
        public int status { get; set; }
    }

    public class UploadedImageResult
    {
        public string[] uploaded { get; private set; }

        public UploadedImageResult(ImageItem[] uploadedImageItemList)
        {
            uploaded = new List<ImageItem>(uploadedImageItemList).ConvertAll(x => x.link).ToArray();
        }
    }


}
