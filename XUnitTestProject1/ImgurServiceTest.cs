using System;
using Xunit;
using Dombo.JobScheduler;
using System.Linq;
using Dombo.CommonModel;
using System.Threading;
using Dombo.ServiceProvider.ImageService;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.ComponentModel;

namespace ServiceTest
{
    /// <summary>
    /// this testclass contains tests ImgurService
    /// </summary>
    public class ImgurServiceTest : IDisposable
    {
        
        public ImgurServiceTest()
        {
            //test initialization
            ImgurWebsiteMock.ResetInstance();//make sure service is reset
        }

        public void Dispose()
        {
            //test Cleanup
            ImgurWebsiteMock.ResetInstance();//make sure service is reset
        }


        /// <summary>
        /// a simple start test 
        /// </summary>
        [Fact]  
        public void GetImageTest()
        {
            ImgurService imgurService = new ImgurService(new SimpleHttpMock(), @"https://imgur.com", @"token_number");
            var result = JsonConvert.DeserializeObject<UploadResultFormat>(imgurService.GetImage());
            Assert.Equal(3, result.uploaded.Length);
            Assert.Contains(@"https://i.imgur.com/aaa.jpg", result.uploaded);
            Assert.Contains(@"https://i.imgur.com/bbb.jpg", result.uploaded);
            Assert.Contains(@"https://i.imgur.com/ccc.jpg", result.uploaded);
        }


        /// <summary>
        /// a simple start test 
        /// </summary>
        [Fact]
        public void UploadImageTest()
        {
            ImgurService imgurService = new ImgurService(new SimpleHttpMock(), @"https://imgur.com", @"token_number");
            imgurService.UploadImages("https://instagram.com/123.jpg");
            var uploadedImage = JsonConvert.DeserializeObject<UploadResultFormat>(imgurService.GetImage());
            Assert.Equal(4, uploadedImage.uploaded.Length);
            Assert.Contains(@"https://i.imgur.com/123.jpg", uploadedImage.uploaded);
        }

        class ImgurWebsiteMock
        {

            private ImgurWebsiteMock()
            {
                //init default images
                _imageList.AddRange(
                    new string[] {
                        @"https://i.imgur.com/aaa.jpg",
                        @"https://i.imgur.com/bbb.jpg",
                        @"https://i.imgur.com/ccc.jpg"
                });
            }

            private static ImgurWebsiteMock _instance = null;

            public static ImgurWebsiteMock GetInstance()
            {
                _instance = _instance == null ? new ImgurWebsiteMock() : _instance;
                return _instance;
            }

            public static void ResetInstance()
            {
                _instance = null;
            }



            List<string> _imageList = new List<string>();

            public string[] UploadedImageList
            {
                get { return _imageList.ToArray(); }
            }

            

            public StringContent GetUploadedImages()
            {
                return new StringContent(
                    JsonConvert.SerializeObject( 
                        new UploadedImage()
                            {
                                data = _imageList.ConvertAll<ImageItem>(x => new ImageItem() { link = x }).ToArray()
                            }), 
                    Encoding.UTF8, "application/json");
            }

            public StringContent UploadImages(string url)
            {
                string imageFileName = url.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Last();
                string newUrl = string.Format("https://i.imgur.com/{0}", imageFileName);

                _imageList.Add(newUrl);
                return new StringContent(
                    JsonConvert.SerializeObject(
                        new UploadedImage()
                        {
                            data = null// _imageList.ConvertAll<ImageItem>(x => new ImageItem() { link = x }).ToArray()
                        }),
                    Encoding.UTF8, "application/json");
            }
        }


        class UploadResultFormat
        {
            public string[] uploaded { get; set; }
        }



        class SimpleHttpMock : IHttpHandler
        {
            //ImgurWebsiteMock _imgur = new ImgurWebsiteMock(); 
            ImgurWebsiteMock _imgurMock = null;


            public SimpleHttpMock()
            {

                _imgurMock = ImgurWebsiteMock.GetInstance();
            }

            public IHttpHandler CreateHandler(KeyValuePair<string, IEnumerable<string>>[] requestHeaderParams)
            {
                return new SimpleHttpMock();
            }

            public HttpResponseMessage Get(string url)
            {
                throw new NotImplementedException();
            }

            public Task<HttpResponseMessage> GetAsync(string url)
            {
                HttpResponseMessage msg = new HttpResponseMessage();
                msg.Content = this._imgurMock.GetUploadedImages();
                return Task.FromResult<HttpResponseMessage>(msg);
            }

            public HttpResponseMessage Post(string url, HttpContent content)
            {
                throw new NotImplementedException();
            }

            public Task<HttpResponseMessage> PostAsync(string url, HttpContent content)
            {
                 
                FormUrlEncodedContent encodedContent = content as FormUrlEncodedContent;
                var formatedUrl = encodedContent.ReadAsStringAsync().Result
                    .Replace("%3A", ":")
                    .Replace("%2F", "/")
                    .Substring("image=".Length);

                var uploadedResult = this._imgurMock.UploadImages(formatedUrl);
                return Task.FromResult<HttpResponseMessage>(
                    new HttpResponseMessage() { Content = uploadedResult });
            }
        }





    }


    
}
