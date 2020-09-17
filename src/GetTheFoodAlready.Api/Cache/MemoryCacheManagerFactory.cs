using System;
using System.Runtime.Caching;

namespace GetTheFoodAlready.Api.Cache
{
	/// <summary> Cache factory that creates memory cache managers. </summary>
	public class MemoryCacheManagerFactory : ICacheManagerFactory
	{
		private readonly MemoryCache _cache;
		private readonly CacheItemPolicy _policy;

		public MemoryCacheManagerFactory(MemoryCache cache, TimeSpan? cacheSlidingPolicy = null)
		{
			_cache = cache ?? throw new ArgumentNullException(nameof(cache));
			_policy = new CacheItemPolicy
			{
				SlidingExpiration = cacheSlidingPolicy ?? TimeSpan.FromHours(2)
			};
		}

		public ICacheManager<T> GetManager<T>() where T : class
		{
			return new MemoryCacheManager<T>(_cache, _policy);
		}
	}
}
