using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using WebApi.Services;

namespace WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<SiloWrapper>();
            services.AddSingleton<IHostedService>(x=>x.GetRequiredService<SiloWrapper>());
            services.AddControllers();
            services.AddSingleton(x => x.GetRequiredService<SiloWrapper>().Client);
            // add client factory
            services.AddSingleton<IGrainFactory>(x => x.GetRequiredService<SiloWrapper>().Client);
			services.AddServicesForSelfHostedDashboard();
		}

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            
            // init siloWrapper before dashboard middleware start(erros out otherwise)
            var siloWrapper = app.ApplicationServices.GetRequiredService<SiloWrapper>();
            siloWrapper.Init().GetAwaiter().GetResult();

			app.UseOrleansDashboard(new OrleansDashboard.DashboardOptions { BasePath = "/dashboard"});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}