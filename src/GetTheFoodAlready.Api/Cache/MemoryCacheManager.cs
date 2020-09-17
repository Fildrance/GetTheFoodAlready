using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace GetTheFoodAlready.Api.Cache
{
	/// <summary>
	/// Cache manager that utilizes memory cache.
	/// </summary>
	/// <typeparam name="T">Type of objects to be cached.</typeparam>
	public class MemoryCacheManager<T> : ICacheManager<T> where T : class
	{
		private readonly MemoryCache _cache;
		private readonly CacheItemPolicy _policy;

		public MemoryCacheManager(MemoryCache cache, CacheItemPolicy policy)
		{
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));
			_policy = policy ?? throw new ArgumentNullException(nameof(policy));
		}

		public Task Evict(string key)
		{
			_cache.Remove(key);
			return Task.CompletedTask;
		}

		public Task<T> Get(string key)
		{
			var found = (T) _cache.Get(key);
			return Task.FromResult(found);
		}

		public Task Set(string key, T value)
		{
			_cache.Set(key, value, _policy);
			return Task.CompletedTask;
		}

		public Task<bool> TryGet(string key, out T value)
		{
			if (_cache.Contains(key))
			{
				value = (T) _cache.Get(key);
				return Task.FromResult(true);
			}
			value = null;
			return Task.FromResult(false);
		}
	}
}
