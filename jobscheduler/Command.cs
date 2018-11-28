
using Dombo.CommonModel;


namespace Dombo.JobScheduler
{

    public class ImageCommandResult : ICommandResult
    {
        public string StatusCode { get; set; }
        public object Result { get; set; }
    }

    public class ImageUploadCommand : ICommand
    {
        public string Argument { get; set; }

        IApiService _service;

        public ImageUploadCommand(IApiService service, string imageUrl)
        {
            _service = service;
            Argument = imageUrl;
        }

        public ICommandResult Run()
        {
            var result = _service.UploadImages(Argument);
            var resultData = Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceData>(result.Result);

            return new ImageCommandResult() { Result=result, StatusCode="Running" };
        }
    }
}
