using System;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.DeliveryClubBridge.DataTypes;

using NLog;

namespace GetTheFoodAlready.DeliveryClubBridge.Client
{
	public class AutoRetryingDeliveryClubClientDecorator : IDeliveryClubClient
	{
		#region [Static fields]
		private static readonly ILogger Logger = LogManager.GetLogger(typeof(AutoRetryingDeliveryClubClientDecorator).FullName);
		#endregion

		#region [Fields]
		private readonly int _maxRetryCount;
		private readonly IDeliveryClubClient _nested;
		#endregion

		#region [c-tor]
		public AutoRetryingDeliveryClubClientDecorator(IDeliveryClubClient nested, int maxRetryCount = 5)
		{
			_nested = nested ?? throw new ArgumentNullException(nameof(nested));
			_maxRetryCount = maxRetryCount;
		}
		#endregion

		#region IDeliveryClubClient implementation
		public Task<RootDeliveryClubVendorsResponse> GetDeliveryClubVendorsNearby(string longitude, string latitude, CancellationToken cancellationToken = default(CancellationToken), int skip = 0,
			int take = 200)
		{
			return DoWithRetry
			(
				() => _nested.GetDeliveryClubVendorsNearby(longitude, latitude, cancellationToken, skip, take),
				response => true
			);
		}

		public Task<DeliveryClubFoodInfo> GetFoodInfo(int vendorPointId, CancellationToken cancellationToken)
		{
			return DoWithRetry(
				() => _nested.GetFoodInfo(vendorPointId, cancellationToken), 
				info => info?.Products?.Count > 0
			);
		}

		public Task<string> Login(CancellationToken cancellationToken, bool forceReLogin = false)
		{
			return DoWithRetry(
				() => _nested.Login(cancellationToken),
				info => true
			);
		}
		#endregion

		#region [Private]
		#region [Private methods]
		private async Task<T> DoWithRetry<T>(Func<Task<T>> factory, Func<T, bool> successCheck)
		{
			bool found;
			T result;
			var retryCount = 0;
			do
			{
				result = await factory();

				found = successCheck(result);
				if (!found)
				{
					Logger.Trace("Call was not successfull.");
				}

				retryCount++;
			}
			while (!found
			       && retryCount < _maxRetryCount);

			return result;
		}
		#endregion
		#endregion
	}
}