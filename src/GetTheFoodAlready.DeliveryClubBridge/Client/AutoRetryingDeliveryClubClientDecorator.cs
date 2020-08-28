using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.DeliveryClubBridge.DataTypes;

using NLog;

namespace GetTheFoodAlready.DeliveryClubBridge.Client
{
	public class AutoRetryingDeliveryClubClientDecorator : IDeliveryClubClient
	{
		#region  [Constants]
		// todo: inject tetry count
		private const int MaxRetryCount = 15;
		#endregion

		#region [Static fields]
		private static readonly ILogger Logger = LogManager.GetLogger(typeof(AutoRetryingDeliveryClubClientDecorator).FullName);
		#endregion

		#region [Fields]
		private readonly IDeliveryClubClient _nested;
		#endregion

		#region [c-tor]
		public AutoRetryingDeliveryClubClientDecorator(IDeliveryClubClient nested)
		{
			_nested = nested;
		}
		#endregion

		#region IDeliveryClubClient implementation
		public Task<RootDeliveryClubVendorsResponse> GetDeliveryClubVendorsNearby(string longitude, string latitude, CancellationToken cancellationToken = default(CancellationToken), int skip = 0,
			int take = 200)
		{
			return _nested.GetDeliveryClubVendorsNearby(longitude, latitude, cancellationToken, skip, take);
		}

		public async Task<DeliveryClubFoodInfo> GetFoodInfo(int vendorPointId, CancellationToken cancellationToken)
		{
			bool found;
			DeliveryClubFoodInfo result;
			var retryCount = 0;
			do
			{
				result = await _nested.GetFoodInfo(vendorPointId, cancellationToken);

				found = result.Products.Count > 0;
				if (!found)
				{
					Logger.Trace("Found 0 products, retrying to get food info.");
				}

				retryCount++;
			}
			while (!found
			       && retryCount < MaxRetryCount);

			return result;
		}

		public Task<string> Login(CancellationToken cancellationToken, bool forceReLogin = false)
		{
			return _nested.Login(cancellationToken, forceReLogin);
		}
		#endregion
	}
}