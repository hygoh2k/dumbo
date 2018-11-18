

using Dombo.CommonModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dombo.ServiceProvider.ImageService
{


    public class ImgurService : IApiService
    {
        private readonly string _url;
        private readonly string _getImageLink;
        private readonly string _postImageLink;
        private readonly string _token;



        public ImgurService(string jsonConfig)
        {
            ServiceConfig config = JsonConvert.DeserializeObject<ServiceConfig>(jsonConfig);
            _url = config.BaseUrl;
            _getImageLink = config.GetImageUrl;
            _postImageLink = config.PostImageUrl;
            _token = config.Token;
        }


        public ImgurService(string url, string token, string getImageLink = @"3/account/me/images", string postImageLink = @"3/image")
        {
            
            _url = url;
            _getImageLink = getImageLink;
            _postImageLink = postImageLink;
            _token = token;

        } 

        //public ISerializableResult GetUploadJobStatusType()
        //{
        //    return new ImageUploadJobStatusResult();
        //}


        private HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);
            return client;
        }

        private async Task<ServiceResult> GetImageAsync()
        {

            string result = null;
            var response = await CreateClient().GetAsync(string.Format("{0}/{1}", _url, _getImageLink));
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

            FormUrlEncodedContent content = new FormUrlEncodedContent(keyValues);
            
            var response = await CreateClient().PostAsync(string.Format("{0}/{1}", _url, _postImageLink), content);
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
