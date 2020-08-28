using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.DeliveryClubBridge.DataTypes;

using NLog;

namespace GetTheFoodAlready.DeliveryClubBridge.Client
{
	public class AutoLoginningDeliveryClubClientDecorator : IDeliveryClubClient
	{
		#region [Static fields]
		private static readonly ILogger Logger = LogManager.GetLogger(typeof(AutoLoginningDeliveryClubClientDecorator).FullName);
		#endregion

		#region [Fields]
		private readonly IDeliveryClubClient _nested;
		#endregion

		#region [c-tor]
		public AutoLoginningDeliveryClubClientDecorator(IDeliveryClubClient nested)
		{
			_nested = nested?? throw new ArgumentNullException(nameof(nested));
		}
		#endregion

		#region IDeliveryClubClient implementation
		public async Task<RootDeliveryClubVendorsResponse> GetDeliveryClubVendorsNearby(string longitude, string latitude, CancellationToken cancellationToken = default(CancellationToken),
			int skip = 0, int take = 200)
		{
			await _nested.Login(cancellationToken);

			try
			{
				return await _nested.GetDeliveryClubVendorsNearby(longitude, latitude, cancellationToken, skip, take);
			}
			catch (Exception e)
			{
				if (e.Message.Contains("401"))
				{
					await _nested.Login(cancellationToken, true);
					return await _nested.GetDeliveryClubVendorsNearby(longitude, latitude, cancellationToken, skip, take);
				}

				throw;
			}
		}

		public async Task<DeliveryClubFoodInfo> GetFoodInfo(int vendorPointId, CancellationToken cancellationToken)
		{
			await _nested.Login(cancellationToken);
			try
			{
				return await _nested.GetFoodInfo(vendorPointId, cancellationToken);
			}
			catch (HttpRequestException e)
			{
				if (e.Message.Contains("401"))
				{
					await _nested.Login(cancellationToken, true);
					return await _nested.GetFoodInfo(vendorPointId, cancellationToken);
				}

				throw;
			}
		}

		public Task<string> Login(CancellationToken cancellationToken, bool forceReLogin = false)
		{
			return _nested.Login(cancellationToken, forceReLogin);
		}
		#endregion

		#region [Private]
		#region [Private methods]
		#endregion
		#endregion
	}
}