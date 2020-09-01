using System.Collections.Generic;

using GetTheFoodAlready.Api.FoodAgregators.Responses;
using GetTheFoodAlready.Api.Orchestration.Requests;

namespace GetTheFoodAlready.Handlers.RandomFoodRolling
{
	/// <summary> Filter for list of vendor points. </summary>
	public interface IVendorFilter
	{
		/// <summary> Gets list of vendors and filters it according to request data. </summary>
		/// <param name="request">Request, containing filter data.</param>
		/// <param name="fullVendorsList">Full enumeration of vendor points.</param>
		/// <returns>Enumeration of vendor points that were accepted by filter.</returns>
		IEnumerable<VendorInfo> FilterVendors(RandomFoodPropositionsRequest request, IEnumerable<VendorInfo> fullVendorsList);
	}
}