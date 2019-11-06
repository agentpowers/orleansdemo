using System.Threading.Tasks;
using Orleans;
using Orleans.Providers;

namespace grains
{

	public interface ICacheGrain<T>: IGrainWithStringKey
	{
		Task<T> GetItem();
        Task<T> SetItem(T obj);
	}

	public class Cache<T>
	{
		public Cache() : this(default(T))
        {
        }

        public Cache(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
	}
	public class CacheGrain<T>:Grain<Cache<T>>, ICacheGrain<T>
    {
		public Task<T> GetItem()
		{
			return Task.FromResult(this.State.Value);
		}

		public async Task<T>SetItem(T obj)
		{
			State.Value = obj;
            await WriteStateAsync();

            return State.Value;
		}
	}
}