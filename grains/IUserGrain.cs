using System.Threading.Tasks;
using Orleans;

namespace grains
{
    public interface IUserGrain:IGrainWithIntegerKey
    {
        ValueTask<UserInfo> GetInfo();
		Task<UserInfo> UpdateInfo(UserInfo info);
		Task<uint> GetBalance();
	}
}