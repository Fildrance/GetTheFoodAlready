using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.DeliveryClubBridge.DataTypes;

namespace GetTheFoodAlready.DeliveryClubBridge
{
	/// <summary>
	/// Client for interacting with delivery-club api.
	/// </summary>
	public interface IDeliveryClubClient
	{
		/// <summary>
		/// Gets list of vendors, closest to pinpointed location.
		/// </summary>
		/// <param name="longitude">Longitude of pinpointed location.</param>
		/// <param name="latitude">Latitude of pinpointed location.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <param name="skip">Count of elements to skip. Default is 0.</param>
		/// <param name="take">Count of elements to take. Default is 200.</param>
		/// <returns>Container with paged list of nearby vendor points.</returns>
		Task<RootDeliveryClubVendorsResponse> GetDeliveryClubVendorsNearby(string longitude, string latitude, CancellationToken cancellationToken = default(CancellationToken), int skip = 0, int take = 200);

		/// <summary>
		/// Gets list of food items by vendor point id.
		/// </summary>
		/// <param name="vendorPointId">Id of vendor point to be asked for menu.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Container with list of menu items and menu categories.</returns>
		Task<DeliveryClubFoodInfo> GetFoodInfo(int vendorPointId, CancellationToken cancellationToken);
	}
}