using Microsoft.AspNetCore.Mvc;
using Orleans;
using System.Threading.Tasks;
using grains;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
	public class UserController : Controller
	{
		private readonly IClusterClient _client;

		public UserController(IClusterClient client)
		{
			_client = client;
		}

		[HttpGet("[action]/{id}")]
		public async Task<UserInfo> Info(long id)
		{
			var userGrain = _client.GetGrain<IUserGrain>(id);
			return await userGrain.GetInfo();
		}

		[HttpPut("[action]/{id}")]
		public async Task<UserInfo> Info(long id, string name, int age)
		{
			var userGrain = _client.GetGrain<IUserGrain>(id);
			return await userGrain.UpdateInfo(new UserInfo { Name = name, Age = age });
		}
	}
}