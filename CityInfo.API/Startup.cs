using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using CityInfo.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using CityInfo.API.Entities;

namespace CityInfo.API
{
    public class Startup
    {
        //public static IConfigurationRoot Configuration;
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        //public Startup(IHostingEnvironment env) {
        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(env.ContentRootPath)
        //        .AddJsonFile("appSettings.json", optional:false, reloadOnChange:true);

        //    Configuration = builder.Build();
        //}
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc()
                .AddMvcOptions(o => o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()));
                

            //.AddJsonOptions(o => {
            //    if (o.SerializerSettings.ContractResolver != null) {
            //        var castedResolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
            //        castedResolver.NamingStrategy = null;
            //    }
            //});

#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif

            var connectionString = Startup.Configuration["connectionStrings:cityInfoDBConnectionString"];
            services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString));
            services.AddScoped<ICityInfoRepository, CityInfoRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, CityInfoContext cityInfoContext)
        {
            env.ConfigureNLog("nlog.config");

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler();
            }

            cityInfoContext.EnsureSeedDataForContext();

            AutoMapper.Mapper.Initialize(cfg => {
                cfg.CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDTO>();
                cfg.CreateMap<Entities.City, Models.CityDTO>();
                cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterestDTO>();
                cfg.CreateMap<Models.PointOfInterestForCreationDTO, Entities.PointOfInterest>();
                cfg.CreateMap<Models.PointOfInterestForUpdateDTO, Entities.PointOfInterest>();
                cfg.CreateMap<Entities.PointOfInterest, Models.PointOfInterestForUpdateDTO>();
            });
            
            app.UseStatusCodePages();
            //MVC will handle HTTP Requests
            app.UseMvc();


            
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
