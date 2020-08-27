using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.DeliveryClubBridge.DataTypes;

using NLog;

namespace GetTheFoodAlready.DeliveryClubBridge
{
	public class AutoRetryingDeliveryClubClient : AutoLoginningDeliveryClubClient
	{
		#region [Static fields]
		private static readonly ILogger Logger = LogManager.GetLogger(typeof(AutoRetryingDeliveryClubClient).FullName);
		// todo: inject tetry count
		private const int MaxRetryCount = 15;
		#endregion

		#region [c-tor]
		public AutoRetryingDeliveryClubClient(HttpClientHandlerProvider provider) : base(provider)
		{
			
		}
		#endregion

		#region [Public]
		#region [Public methods]
		public override async Task<DeliveryClubFoodInfo> GetFoodInfo(int vendorPointId, CancellationToken cancellationToken)
		{
			bool found;
			DeliveryClubFoodInfo result;
			var retryCount = 0;
			do
			{ 
				result = await base.GetFoodInfo(vendorPointId, cancellationToken);

				found = result.Products.Count > 0;
				if (!found)
				{
					Logger.Trace("Found 0 products, retrying to get food info.");
				}

				retryCount++;
			}
			while (!found && retryCount < MaxRetryCount);
			
			
			return result;
		}
		#endregion
		#endregion
	}
}
