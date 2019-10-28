using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;
using Orleans;

namespace WebApi
{
	public class Program
    {
        public static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>();
                })
                //https://stackoverflow.com/questions/54841844/orleans-direct-client-in-asp-net-core-project/54842916#54842916
                .UseOrleans(builder =>
                {
                    // EnableDirectClient is no longer needed as it is enabled by default
                    builder.UseLocalhostClustering()
                    .ConfigureApplicationParts(parts =>
                        parts.AddApplicationPart(typeof(grains.IUserGrain).Assembly).WithReferences())
                    .AddAdoNetGrainStorageAsDefault(options => 
                    {
                        options.Invariant = "Npgsql";
                        options.ConnectionString = "host=localhost;database=orleans;username=agentpowers";
                        options.UseJsonFormat = true;
                    })
                    .ConfigureLogging(x =>
                    {
                        x.AddConsole();
                        x.SetMinimumLevel(LogLevel.Warning);
                    })
                    .UseDashboard(x =>
                    {
                        x.HostSelf = false;
                    })
                    .UseTransactions();
                })
                .Build();
            host.Run();
        }
    }
}