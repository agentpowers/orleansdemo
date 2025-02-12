using System.Threading.Tasks;
using Orleans;
using Orleans.Providers;

namespace grains
{

	public class UserGrain:Grain<UserInfo>,IUserGrain
    {
		public override async Task OnActivateAsync()
		{
			await ReadStateAsync();
			await base.OnActivateAsync();
		}

		public ValueTask<UserInfo> GetInfo()
        {		
			return new ValueTask<UserInfo>(State);
        }

		public async Task<UserInfo> UpdateInfo(UserInfo info)
		{
			State = info;
			await WriteStateAsync();
			return State;
		}

		public async Task<uint> GetBalance()
		{
			var account = this.GrainFactory.GetGrain<IAccountGrain>(this.GetPrimaryKeyLong());
			return await account.GetBalance();
		}
	}
}