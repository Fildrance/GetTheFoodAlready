using System;
using System.Collections.Generic;

using GetTheFoodAlready.Api.FoodAgregators.Responses;

namespace GetTheFoodAlready.Api.Orchestration.Reponses
{
	public class RandomFoodPropositionsResponse 
	{
		#region [c-tor]
		public RandomFoodPropositionsResponse(
			IReadOnlyCollection<FoodInfo> foodInfos, 
			IReadOnlyCollection<FoodInfo> proposedFoods,
			VendorInfo vendorInfo
		)
		{
			FullListOfFoodInfos = foodInfos ?? throw new ArgumentNullException(nameof(foodInfos));
			ProposedFoods = proposedFoods ?? throw new ArgumentNullException(nameof(proposedFoods));
			VendorInfo = vendorInfo ?? throw new ArgumentNullException(nameof(vendorInfo));
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public IReadOnlyCollection<FoodInfo> FullListOfFoodInfos { get; }
		public IReadOnlyCollection<FoodInfo> ProposedFoods { get; }
		public VendorInfo VendorInfo { get; }
		#endregion
		#endregion
	}
}