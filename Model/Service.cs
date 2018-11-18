using System;
using System.Net;

namespace Dombo.CommonModel
{
    public struct ServiceResult
    {
        public HttpStatusCode ResultStatus { get; set; }
        public string Result { get; set; }
    }

    public class ServiceConfig
    {
        public string BaseUrl { get; set; }
        public string Token { get; set; }
        public string GetImageUrl { get; set; }
        public string PostImageUrl { get; set; }
    }

    public class DataContent
    {
        public string error { get; set; }
        public string method { get; set; }
        public string request { get; set; }
    }

    public class ServiceData
    {
        public bool success { get; set; }
        public int status { get; set; }
        public DataContent data { get; set; }
    }

    

    public interface IApiService
    {
        string GetImage();
        ServiceResult UploadImages(string path);
        //ISerializableResult GetUploadJobStatusType();
    }
}
