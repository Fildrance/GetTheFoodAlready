using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.FoodAgregators.Requests;
using GetTheFoodAlready.Api.FoodAgregators.Responses;

namespace GetTheFoodAlready.Api.FoodAgregators
{
	/// <summary>
	/// Service for manipulating with Delivery club.
	/// This is wrapper around mediator.
	/// </summary>
	public interface IDeliveryClubService
	{
		/// <summary>
		/// Gets list of closest vendor points.
		/// </summary>
		/// <param name="request">Request for closest restoraunts. Contains point to which term 'closest' should be applied.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>List of closest restoraunts.</returns>
		Task<ClosestVendorPointsGetResponse> GetClosestVendorPoints(ClosestVendorPointsGetRequest request, CancellationToken cancellationToken);
		/// <summary>
		/// Gets information on menu of requested vendor point.
		/// </summary>
		/// <param name="foodInfoRequest">Request with vendor point info and accepted menu categories.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>List of filtered menu items.</returns>
		Task<FoodInfoGetResponse> GetFoodInfo(FoodInfoGetRequest foodInfoRequest, CancellationToken cancellationToken);
	}
}
