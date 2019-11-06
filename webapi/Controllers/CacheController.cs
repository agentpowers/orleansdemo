using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;
using grains;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
	public class CacheController : Controller
	{
		private readonly IClusterClient _client;

		public CacheController(IClusterClient client)
		{
			_client = client;
		}

		[HttpGet("{key}")]
		public async Task<string> Get(string key)
		{
			var cacheGrain = _client.GetGrain<ICacheGrain<string>>(key);
			return await cacheGrain.GetItem();
		}

		[HttpPut("{key}/{value}")]
		public async Task<string> Set(string key, string value)
		{
			var cacheGrain = _client.GetGrain<ICacheGrain<string>>(key);
			return await cacheGrain.SetItem(value);
		}
	}
}