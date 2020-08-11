using System;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.DeliveryClubBridge.DataTypes;

namespace GetTheFoodAlready.DeliveryClubBridge
{
	/// <summary>
	/// Client for interacting with delivery-club api.
	/// </summary>
	public interface IDeliveryClubClient : IDisposable
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
		Task<RootDeliveryClubVendorsResponse> GetDeliveryClubVendorsNearby(decimal longitude, decimal latitude, CancellationToken cancellationToken = default(CancellationToken), int skip = 0, int take = 200);
	}
}