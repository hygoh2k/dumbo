﻿using Dombo.ServiceProvider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Dombo.JobScheduler
{
    public interface ICommandResult
    {
        string StatusCode { get; set; }
        object Result { get; set; }
    }


    public interface ICommand
    {
        string ArgumentCollection { get; set; }
        ICommandResult Run();

    }

    public class ImageCommandResult : ICommandResult
    {
        public string StatusCode { get; set; }
        public object Result { get; set; }
    }



    public class ImageUploadCommand : ICommand
    {
        public string ArgumentCollection { get; set; }

        IApiService _service;

        public ImageUploadCommand(IApiService service, string imageUrl)
        {
            _service = service;
            ArgumentCollection = imageUrl;

        }



        public ICommandResult Run()
        {
            var result = _service.UploadImages(ArgumentCollection);
            var resultData = Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceData>(result.Result);

            return new ImageCommandResult() { Result=result };
        }
    }
}
