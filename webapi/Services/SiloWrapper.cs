using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Services
{
	public class SiloWrapper : IHostedService
	{
		const string connString = @"User ID=agentpowers;Password=5544338;Host=localhost;Port=5432;Database=orleans;
Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0;";
		private readonly ISiloHost _silo;
		public readonly IClusterClient Client;

		public SiloWrapper()
		{
			_silo = GetSiloHost();
			Client = _silo.Services.GetRequiredService<IClusterClient>();
		}

		private static ISiloHost GetSiloHost() 
		{
			return new SiloHostBuilder()
				.UseLocalhostClustering()
				.ConfigureApplicationParts(parts =>
					parts.AddApplicationPart(typeof(grains.IUserGrain).Assembly).WithReferences())
				.AddAdoNetGrainStorageAsDefault(options => 
				{
					options.Invariant = "Npgsql";
					options.ConnectionString = "host=localhost;database=orleans;password=5544338;username=agentpowers";
					options.UseJsonFormat = true;
				})
				// .AddMongoDBGrainStorageAsDefault(options =>
				// {
				// 	options.ConnectionString = "mongodb://mongo01/OrleansTestApp";
				// })
				.ConfigureLogging(x =>
				{
					x.AddConsole();
					x.SetMinimumLevel(LogLevel.Warning);
				})
				.UseDashboard(x =>
				{
					x.HostSelf = false;
				})
				.UseTransactions()
				.Build();
		}
		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await _silo.StartAsync(cancellationToken);
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			await _silo.StopAsync(cancellationToken);
		}
	}
}