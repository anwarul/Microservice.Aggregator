using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedlimeSolutions.Microservice.Framework;
using RedlimeSolutions.Microservice.Framework.ApiDoc;
using RedlimeSolutions.Microservice.Framework.ApiPipline;
using RedlimeSolutions.Microservice.Framework.Core;
using RedlimeSolutions.Microservice.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RS.MF.ServiceName.HostService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var apiBuilder = new ApiPiplineBuilderOptions
            {
                RequiresImpersonation = false,
                EnableFileLogging = true,
                EnableConsoleLogging = true,
                EnableSeriLogging = true,
                UseJwtBearerAuthentication = true,
                CommandLineArguments = args,
                AddApplicationServices = AddApplicationServices,
                AddApplicationConfigure = Configure,
                RDMSMigrationAssemblyName = typeof(Program).Assembly.FullName
            };
            var host = WebHostApiPipelineBuilder.BuildWebApiPipeline(apiBuilder);
            host.Build().Run();
        }

        private static void AddApplicationServices(IServiceCollection services, IAppSettings appSettings, IConfiguration Configuration)
        {
            services.AddSingleton(appSettings);
            services.AddApiDocument(new SwaggerOptions()
            {
                Description = "Use the api to sign electronic documents",
                ApplicableEnvironments = new string[] { "dev", "dev-az", "prod-az", "stg", "stg-az" }
            }, appSettings);
            services.AddMediatR(typeof(Program).Assembly);
            services.AddControllers()
               .AddFluentValidation(s =>
               {
                   //s.RegisterValidatorsFromAssemblyContaining<UserQuery>();
                   s.DisableDataAnnotationsValidation = false;
               });

            services.AddCore(
                typeof(Producer)
                /*,
                typeof(UserQuery)*/
                );
        }

        public static void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseCore();
        }
    }
}