using System.Collections.Generic;

using GetTheFoodAlready.Api.FoodAgregators.Responses;

namespace GetTheFoodAlready.Api.Orchestration.Responses
{
	public class RandomFoodPropositionsResponse 
	{
		// tmp solution with tuple, need to see secnarios for usage of that response to deduce proper data structure.
		#region [c-tor]
		public RandomFoodPropositionsResponse(IReadOnlyDictionary<VendorInfo, (IEnumerable<FoodInfo> totalList, IEnumerable<FoodInfo> proposedList)> vendorsToRollResults)
		{
			VendorsToRollResults = vendorsToRollResults;
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public IReadOnlyDictionary<VendorInfo, (IEnumerable<FoodInfo> totalList, IEnumerable<FoodInfo> proposedList)> VendorsToRollResults { get; }
		#endregion
		#endregion
	}
}