using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dombo.CommonModel;
using Dombo.JobScheduler;
using Dombo.ServiceProvider.ImageService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace WebAPI
{

    

    public class Startup
    {
        private IConfiguration _configuration;
        private IHostingEnvironment _env;

        public Startup( IHostingEnvironment env)
            {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            _env = env;
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get => _configuration; private set => _configuration = value; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.AddSingleton<JobServiceBase, JobService>();
            services.AddSingleton<JobServiceBase>(new Dombo.JobScheduler.JobService());
            services.AddSingleton<IApiService>(new ImgurService(File.ReadAllText(Path.Combine(_env.ContentRootPath, "imgur_setting.json"))));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
