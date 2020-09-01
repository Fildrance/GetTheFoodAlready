using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.FoodAgregators.Responses;
using GetTheFoodAlready.Api.Orchestration.Requests;

namespace GetTheFoodAlready.Handlers.RandomFoodRolling
{
	/// <summary> Strategy for getting random food items. </summary>
	public interface IRandomFoodStrategy
	{
		/// <summary>
		/// Creates list of random food from full list. Food can be chosen from many passed vendors, or on one.
		/// </summary>
		/// <param name="vendorRollResult">Information on vendor points and their products.</param>
		/// <returns>Map of vendor point to enumerations of proposed random food items and total lists of mentioned vendor point.</returns>
		IReadOnlyDictionary<VendorInfo, (IEnumerable<FoodInfo> totalList, IEnumerable<FoodInfo> proposedList)> RollFood(IReadOnlyDictionary<VendorInfo, IReadOnlyCollection<FoodInfo>> vendorRollResult);
		/// <summary>
		/// Rolls random vendor point, based on <see cref="request"/> data.
		/// </summary>
		/// <param name="request">Request that can contain data for filtering or random-rolling vendor points.</param>
		/// <param name="vendorPoints">Enumeration of vendor points to be searched for one random vendor.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Map of vendor points to enumerations of theirs proposed foods.</returns>
		Task<IReadOnlyDictionary<VendorInfo, IReadOnlyCollection<FoodInfo>>> RollVendor(RandomFoodPropositionsRequest request, IEnumerable<VendorInfo> vendorPoints, CancellationToken cancellationToken);
		/// <summary> Checks, if strategy is applicable. </summary>
		/// <param name="request">Request data that may contain info for deciding, if strategy is applicable.</param>
		/// <returns>True if strategy is applicable for passed request, false otherwise.</returns>
		bool CanAccept(RandomFoodPropositionsRequest request);
	}
}