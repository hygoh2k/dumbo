using ImageService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{



    class Program
    {
        
        static void Main(string[] args)
        {
           ImageService.ImgurService service = new ImageService.ImgurService(@"https://api.imgur.com", @"199b7dbbdabc0cade4ef884875257c41daa8ac45");
           // var result = service.GetImage();

            var result = service.UploadImages(@"https://farm3.staticflickr.com/2879/11234651086_681b3c2c00_b_d.jpg"  );

            //var result = service.UploadImages(@"https://farm3.staticflickr1.com/2879/11234651086_681b3c2c00_b_d.jpg");
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceData>(result.Result);

            return;

            //using (var client = new HttpClient())
            //{
            //    var url = "https://api.imgur.com/3/account/huanyong/images";
            //    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + "199b7dbbdabc0cade4ef884875257c41daa8ac45");
            //    var response = client.GetStringAsync(url).Result;
                
            //}

            //var client_id = "9ca4b5273713ebf";
            //var client_secret = "48b3381652da37234d42455a2738d93f75cd74ab";
            //string credentials = String.Format("{0}:{1}", client_id, client_secret);

            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));



            //RunAsync().GetAwaiter().GetResult();




            Console.WriteLine("Hello World!");
        }
    }
}
