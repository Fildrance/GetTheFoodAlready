using GetTheFoodAlready.Api.Cache;
using GetTheFoodAlready.DeliveryClubBridge.DataTypes;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GetTheFoodAlready.DeliveryClubBridge.Client
{
	/// <summary> Decorator that caches call results to delivery club client. </summary>
	public class CachingDeliveryClubClientDecorator : IDeliveryClubClient
	{
		private readonly ICacheManager<RootDeliveryClubVendorsResponse> _vendorCache;
		private readonly ICacheManager<DeliveryClubFoodInfo> _foodCache;
		private readonly IDeliveryClubClient _nested;

		public CachingDeliveryClubClientDecorator(
			IDeliveryClubClient nested,
			ICacheManagerFactory factory			
		)
		{
			_nested = nested ?? throw new ArgumentNullException(nameof(nested));
			var fact = factory ?? throw new ArgumentNullException(nameof(factory));
			_vendorCache = fact.GetManager<RootDeliveryClubVendorsResponse>();
			_foodCache = fact.GetManager<DeliveryClubFoodInfo>();

		}

		public Task<RootDeliveryClubVendorsResponse> GetDeliveryClubVendorsNearby(string longitude, string latitude, CancellationToken cancellationToken = default, int skip = 0, int take = 200)
		{
			var key = $"{longitude}{latitude}{skip}{take}";
			return CacheOutput(() => _nested.GetDeliveryClubVendorsNearby(longitude, latitude, cancellationToken, skip, take), _vendorCache, key);
		}

		public Task<DeliveryClubFoodInfo> GetFoodInfo(int vendorPointId, CancellationToken cancellationToken)
		{
			var key = $"{vendorPointId}";
			return CacheOutput(() => _nested.GetFoodInfo(vendorPointId, cancellationToken), _foodCache, key);
		}

		public Task<string> Login(CancellationToken cancellationToken, bool forceReLogin = false)
		{
			// login should not be cached as it will create undesired and hard to understand behaviour.
			return _nested.Login(cancellationToken, forceReLogin);
		}

		private static async Task<T> CacheOutput<T>(Func<Task<T>> factory, ICacheManager<T> manager, string key) where T : class
		{
			if (await manager.TryGet(key, out var cached))
			{
				return cached;
			}
			var result = await factory();
			await manager.Set(key, result);
			return result;
		}
	}
}
