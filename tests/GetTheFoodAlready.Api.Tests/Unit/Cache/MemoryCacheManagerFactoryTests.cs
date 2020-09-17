using GetTheFoodAlready.Api.Cache;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace GetTheFoodAlready.Api.Tests.Unit.Cache
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Unit")]
	public class MemoryCacheManagerFactoryTests
	{
		MemoryCacheManagerFactory _cut;

		private const string Key = "key";

		[SetUp]
		public void Setup()
		{
			_cut = new MemoryCacheManagerFactory(MemoryCache.Default);
		}

		[Test]
		public async Task GetManager_Default_HaveSameCache()
		{
			//arrange
			var cache = new MemoryCache("someName");
			var expected = new object();
			cache.Set(Key, expected, new CacheItemPolicy { SlidingExpiration = ObjectCache.NoSlidingExpiration});
			_cut = new MemoryCacheManagerFactory(cache);
			//act
			var manager = _cut.GetManager<object>();
			var actual = await manager.Get(Key);
			//assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public async Task GetManager_UseFastExpirationPolicy_PolicyIsHonored()
		{
			//arrange
			var cache = new MemoryCache("someName");
			var expected = new object();
			_cut = new MemoryCacheManagerFactory(cache, TimeSpan.FromSeconds(2));
			//act
			var manager = _cut.GetManager<object>();
			await manager.Set(Key, expected);
			await Task.Delay(TimeSpan.FromSeconds(4));
			var actual = await manager.Get(Key);
			//assert
			Assert.Null(actual);
		}

		[Test]
		public async Task GetManager_UseSlowExpirationPolicy_PolicyIsHonored()
		{
			//arrange
			var cache = new MemoryCache("someName");
			var expected = new object();
			_cut = new MemoryCacheManagerFactory(cache, TimeSpan.FromSeconds(6));
			//act
			var manager = _cut.GetManager<object>();
			await manager.Set(Key, expected);
			await Task.Delay(3000);
			var actual = await manager.Get(Key);
			//assert
			Assert.AreEqual(expected, actual);
		}
	}
}
