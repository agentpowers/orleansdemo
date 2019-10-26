using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace dashboard
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var clientBuilder = 
                new ClientBuilder()
                .UseLocalhostClustering()
                // Clustering provider
                .ConfigureApplicationParts(parts =>
					parts.AddApplicationPart(typeof(grains.IUserGrain).Assembly).WithReferences())
                .UseDashboard();
				// .AddAdoNetGrainStorageAsDefault(options => 
				// {
				// 	options.Invariant = "Npgsql";
				// 	options.ConnectionString = "host=localhost;database=orleans;password=5544338;username=agentpowers";
				// 	options.UseJsonFormat = true;
				// });

            var client = clientBuilder.Build();
            await client.Connect();

            Console.ReadLine();
        }
    }
}
