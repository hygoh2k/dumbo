

using Dombo.CommonModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Dombo.ServiceProvider.ImageService
{
    public class HttpClientProxy : IHttpHandler
    {
        public HttpClient HttpClient { get; private set; }


        //public KeyValuePair<string, IEnumerable<string>>[] RequestHeaderParams { get; set; }

        public IHttpHandler CreateHandler(KeyValuePair<string, IEnumerable<string>>[] RequestHeaderParams)
        {
            var client = new HttpClient();
            foreach (var headerParam in RequestHeaderParams)
            {
                client.DefaultRequestHeaders.Add(headerParam.Key, headerParam.Value);
            }
            
            return new HttpClientProxy() { HttpClient = client };
        }

        public HttpResponseMessage Get(string url)
        {
            //todo
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return HttpClient.GetAsync(url);
        }

        public HttpResponseMessage Post(string url, HttpContent content)
        {
            //todo
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return HttpClient.PostAsync(url, content);
        }
    }

    public class ImgurService : IApiService
    {
        private readonly string _url;
        private readonly string _getImageLink;
        private readonly string _postImageLink;
        private readonly string _token;
        private readonly IHttpHandler _httpHandler;


        public ImgurService(IHttpHandler httpHandler, string jsonConfig)
        {
            ServiceConfig config = JsonConvert.DeserializeObject<ServiceConfig>(jsonConfig);
            _url = config.BaseUrl;
            _getImageLink = config.GetImageUrl;
            _postImageLink = config.PostImageUrl;
            _token = config.Token;
            _httpHandler = httpHandler;
        }


        public ImgurService(IHttpHandler httpHandler, string url, string token, string getImageLink = @"3/account/me/images", string postImageLink = @"3/image")
        {
            
            _url = url;
            _getImageLink = getImageLink;
            _postImageLink = postImageLink;
            _token = token;
            _httpHandler = httpHandler;

        } 

        //public ISerializableResult GetUploadJobStatusType()
        //{
        //    return new ImageUploadJobStatusResult();
        //}


        //private HttpClient CreateClient()
        //{
        //    var client = new HttpClient();
        //    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);
        //    return client;
        //}


        //private IHttpHandler CreateClient(KeyValuePair<string, IEnumerable<string>>[] requestHeaderParams)
        //{

        //    var client = new HttpClient();
        //    foreach (var headerParam in requestHeaderParams)
        //    {
        //        client.DefaultRequestHeaders.Add(headerParam.Key, headerParam.Value);
        //    }
        //    //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);
        //    //return client;
        //    return new HttpClientHandler(client);
        //}

        private async Task<ServiceResult> GetImageAsync()
        {
            string result = null;
            var header = new KeyValuePair<string, IEnumerable<string>>("Authorization", new string[] { "Bearer " + _token });
            var response = await _httpHandler.CreateHandler(new KeyValuePair<string, IEnumerable<string>>[] { header })
                .GetAsync(string.Format("{0}/{1}", _url, _getImageLink));

            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsStringAsync();
            }

            return new ServiceResult() { Result = result, ResultStatus = response.StatusCode }; 
        }


        private async Task<ServiceResult> UploadImageAsync(string imageUrl)
        {
            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(KeyValuePair.Create<string, string>("image", imageUrl));
            var header = new KeyValuePair<string, IEnumerable<string>>("Authorization", new string[] { "Bearer " + _token });
            FormUrlEncodedContent content = new FormUrlEncodedContent(keyValues);
            var response = await _httpHandler.CreateHandler(new KeyValuePair<string, IEnumerable<string>>[] { header })
                .PostAsync(string.Format("{0}/{1}", _url, _postImageLink), content);

            string result = await response.Content.ReadAsStringAsync();
            return new ServiceResult() { Result = result, ResultStatus = response.StatusCode };
        }

        public ServiceResult UploadImages(string imageUrl)
        {
            return UploadImageAsync(imageUrl).Result;
        }

        public string GetImage()
        {
            UploadedImage result = JsonConvert.DeserializeObject<UploadedImage>(GetImageAsync().Result.Result);
            return JsonConvert.SerializeObject(new UploadedImageResult(result.data));
        }
    }
}
