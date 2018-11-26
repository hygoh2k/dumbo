using Dombo.CommonModel;
using Dombo.ServiceProvider.ImageService;
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
           ImgurService service = new ImgurService( new Dombo.ServiceProvider.ImageService.HttpClientProxy(),
               @"https://api.imgur.com", @"199b7dbbdabc0cade4ef884875257c41daa8ac45");
           // var result = service.GetImage();

            var result = service.UploadImages(@"https://farm3.staticflickr.com/2879/11234651086_681b3c2c00_b_d.jpg"  );

            //var result = service.UploadImages(@"https://farm3.staticflickr1.com/2879/11234651086_681b3c2c00_b_d.jpg");
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceData>(result.Result);

            return;
        }
    }
}
