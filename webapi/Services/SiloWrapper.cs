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
		private readonly ISiloHost _silo;
		public readonly IClusterClient Client;

		public SiloWrapper()
		{
			_silo = new SiloHostBuilder().UseLocalhostClustering()
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
					// x.BasePath = "/dashboard";
				})
				.UseTransactions()
				.Build();

			Client = _silo.Services.GetRequiredService<IClusterClient>();
		}

		public async Task Init()
		{
			await _silo.StartAsync();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			await _silo.StopAsync(cancellationToken);
		}
	}
}