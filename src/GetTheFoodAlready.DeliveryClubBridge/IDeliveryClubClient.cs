using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.DeliveryClubBridge.DataTypes;

namespace GetTheFoodAlready.DeliveryClubBridge
{
	public interface IDeliveryClubClient
	{
		Task<RootDeliveryClubVendorsResponse> GetDeliveryClubVendorsNearby(decimal longitude, decimal latitude, CancellationToken cancellationToken, int skip = 0, int take = 100);
	}
}