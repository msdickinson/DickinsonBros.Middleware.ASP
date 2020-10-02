using DickinsonBros.DateTime.Extensions;
using DickinsonBros.Encryption.Certificate.Extensions;
using DickinsonBros.Guid.Extensions;
using DickinsonBros.Logger.Extensions;
using DickinsonBros.Redactor.Extensions;
using DickinsonBros.Stopwatch.Extensions;
using DickinsonBros.Telemetry.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DickinsonBros.Middleware.ASP.Runner
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddControllers();

            //Stack
            services.AddGuidService();
            services.AddDateTimeService();
            services.AddStopwatchService();
            services.AddLoggingService();
            services.AddRedactorService();
            services.AddConfigurationEncryptionService();
            services.AddTelemetryService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Adds Middleware
            app.UseMiddleware<MiddlewareService>();

            //Comes with ASP.net Template
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
