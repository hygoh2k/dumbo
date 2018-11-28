

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
    /// <summary>
    /// A proxy of HttpClient
    /// 
    /// </summary>
    public class HttpClientProxy : IHttpHandler
    {
        public HttpClient HttpClient { get; private set; }

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
            //todo: not implemented yet
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return HttpClient.GetAsync(url);
        }

        public HttpResponseMessage Post(string url, HttpContent content)
        {
            //todo: not implemented yet
            throw new NotImplementedException();
        }

        public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
        {
            return HttpClient.PostAsync(url, content);
        }
    }

    public class ImgurService : IApiService
    {
        private string _url;
        private string _getImageLink;
        private string _postImageLink;
        private string _token;
        private IHttpHandler _httpHandler;


        /// <summary>
        /// constructor that initialized from a json config file
        /// </summary>
        /// <param name="httpHandler"></param>
        /// <param name="jsonConfig"></param>
        public ImgurService(IHttpHandler httpHandler, string jsonConfig)
        {
            ServiceConfig config = JsonConvert.DeserializeObject<ServiceConfig>(jsonConfig);

            ConfigureImgurService(
                httpHandler,
                config.BaseUrl,
                config.Token,
                config.GetImageUrl,
                config.PostImageUrl
                );
        }


        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="httpHandler"></param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="getImageLink"></param>
        /// <param name="postImageLink"></param>
        public ImgurService(IHttpHandler httpHandler, 
            string url, 
            string token, 
            string getImageLink = @"3/account/me/images", 
            string postImageLink = @"3/image")
        {
            ConfigureImgurService(
                httpHandler,
                url,
                token,
                getImageLink,
                postImageLink
            );
        }

        private void ConfigureImgurService(IHttpHandler httpHandler, string baseUrl, string token, string getImageUrl, string postImageUrl)
        {
            _url = baseUrl;
            _getImageLink = getImageUrl;
            _postImageLink = postImageUrl;
            _token = token;
            _httpHandler = httpHandler;
        }


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

        /// <summary>
        /// get the image in imgur service
        /// </summary>
        /// <returns>returns the result in json</returns>
        public string GetImage()
        {
            var imgurResult = GetImageAsync();
            //if (imgurResult.Result.ResultStatus == HttpStatusCode.OK)
            //{
            //    UploadedImage result = JsonConvert.DeserializeObject<UploadedImage>(imgurResult.Result.Result);
            //    return JsonConvert.SerializeObject(new UploadedImageResult(result.data));
            //}
            //else
            //{
            //    //something wrong
            //    return string.Empty;
            //}

            return imgurResult.Result.ResultStatus == HttpStatusCode.OK ?
                 JsonConvert.SerializeObject(
                     new UploadedImageResult(JsonConvert.DeserializeObject<UploadedImage>(imgurResult.Result.Result).data)) :
                 string.Empty;
        }
    }
}
