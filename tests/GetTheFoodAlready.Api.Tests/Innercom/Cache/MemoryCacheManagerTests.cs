using GetTheFoodAlready.Api.Cache;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace GetTheFoodAlready.Api.Tests.Innercom.Cache
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Innercom")]
	public class MemoryCacheManagerTests
	{
		MemoryCacheManager<TestClass> _cut;

		private static readonly CacheItemPolicy policy = new CacheItemPolicy { SlidingExpiration = ObjectCache.NoSlidingExpiration };
		private const string Key = "lefu";

		[SetUp]
		public void Setup()
		{
			var memCache = new MemoryCache("test");
			_cut = new MemoryCacheManager<TestClass>(memCache, policy);
		}
		[Test]
		public async Task Get_RequestNonExistingKey_ReturnsNull()
		{
			// arrange
			// act
			var actual = await _cut.Get(Key);
			// assert
			Assert.IsNull(actual);
		}

		[Test]
		public async Task Get_DifferentCache_DoesntSeeThrough()
		{
			// arrange
			var otherCache = new MemoryCache("test");
			otherCache.Set(Key, new TestClass(), policy);
			// act
			var actual = await _cut.Get(Key);
			// assert
			Assert.IsNull(actual);
		}

		[Test]
		public async Task Evict_AddThenRequestAfterEvict_ReturnsNull()
		{
			// arrange
			var cache = new MemoryCache("test");
			cache.Set(Key, new TestClass(), policy);
			_cut = new MemoryCacheManager<TestClass>(cache, policy);
			// act
			await _cut.Evict(Key);
			// assert
			var found = cache.Get(Key);
			Assert.IsNull(found);
		}

		[Test]
		public async Task TryGet_ExistingKey_OutsItem()
		{
			// arrange
			var cache = new MemoryCache("test");
			var expected = new TestClass();
			cache.Set(Key, expected, policy);
			_cut = new MemoryCacheManager<TestClass>(cache, policy);
			// act
			await _cut.TryGet(Key, out var actual);
			// assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public async Task Set_ExistingKey_IsReplaced()
		{
			// arrange
			var cache = new MemoryCache("test");
			var old = new TestClass();
			var expected = new TestClass();
			cache.Set(Key, old, policy);
			_cut = new MemoryCacheManager<TestClass>(cache, policy);
			// act
			await _cut.Set(Key, expected);
			// assert
			Assert.AreEqual(expected, cache.Get(Key));
		}

		[Test]
		public async Task Set_NonExistingKey_IsSet()
		{
			// arrange
			var cache = new MemoryCache("test");
			var expected = new TestClass();
			_cut = new MemoryCacheManager<TestClass>(cache, policy);
			// act
			await _cut.Set(Key, expected);
			// assert
			Assert.AreEqual(expected, cache.Get(Key));
		}

		[Test]
		public void Set_ValueIsNull_Throws()
		{
			// arrange
			var cache = new MemoryCache("test");
			var expected = new TestClass();
			_cut = new MemoryCacheManager<TestClass>(cache, policy);
			// act
			// assert
			Assert.ThrowsAsync<ArgumentNullException>(() => _cut.Set(Key, null));
		}

		[Test]
		public async Task TryGet_ExistingKey_ReturnsTrue()
		{
			// arrange
			var otherCache = new MemoryCache("test");
			var expected = new TestClass();
			otherCache.Set(Key, expected, policy);
			_cut = new MemoryCacheManager<TestClass>(otherCache, policy);
			// act
			var actual = await _cut.TryGet(Key, out var item);
			// assert
			Assert.True(actual);
		}

		[Test]
		public async Task TryGet_NonExistingKey_ReturnsFalse()
		{
			// arrange
			// act
			var actual = await _cut.TryGet(Key, out var item);
			// assert
			Assert.False(actual);
		}

		[Test]
		public async Task TryGet_NonExistingKey_OutsNull()
		{
			// arrange
			// act
			await _cut.TryGet(Key, out var actual);
			// assert
			Assert.Null(actual);
		}

		private class TestClass
		{

		}

	}
}
