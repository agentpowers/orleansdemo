using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;
using Orleans;
using Orleans.Configuration;
using System;
using System.Net;

namespace WebApi
{
    public class Program
    {
        private const string Invariant = "Npgsql";
        private const string ConnectionString = "host=localhost;database=orleans;username=orleans;password=orleans";

        public static void Main(string[] args)
        {
            int apiPort, siloPort, gatewayPort;
            try
            {
                apiPort = int.Parse(args[0]);
                siloPort = int.Parse(args[1]);
                gatewayPort = int.Parse(args[2]);
            }
            catch (Exception)
            {
                apiPort = 50000;
                siloPort = 11111;
                gatewayPort = 30000;
            }
            var host = new HostBuilder()
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>();
                    builder.UseUrls($"http://*:{apiPort}");
                })
                //https://stackoverflow.com/questions/54841844/orleans-direct-client-in-asp-net-core-project/54842916#54842916
                .UseOrleans(builder =>
                {
                    // EnableDirectClient is no longer needed as it is enabled by default
                    builder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "my-first-cluster";
                        options.ServiceId = "netcore3";
                    })
                    .ConfigureEndpoints(siloPort: siloPort, gatewayPort: gatewayPort)
                    .UseAdoNetClustering(options => 
                    {
                        options.Invariant = Invariant;
                        options.ConnectionString = ConnectionString;
                    })
                    .AddAdoNetGrainStorageAsDefault(options => 
                    {
                        options.Invariant = Invariant;
                        options.ConnectionString = ConnectionString;
                        options.UseJsonFormat = true;
                    })
                    .ConfigureApplicationParts(parts =>
                        parts.AddApplicationPart(typeof(grains.IUserGrain).Assembly).WithReferences())
                    .ConfigureLogging(x =>
                    {
                        x.AddConsole();
                        x.SetMinimumLevel(LogLevel.Warning);
                    })
                    .UseDashboard(x =>
                    {
                        x.HostSelf = false;
                    })
                    .UseTransactions();
                })
                .Build();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}