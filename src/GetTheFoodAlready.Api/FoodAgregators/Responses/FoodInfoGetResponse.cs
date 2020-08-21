using System.Collections.Generic;

namespace GetTheFoodAlready.Api.FoodAgregators.Responses
{
	public class FoodInfoGetResponse
	{
		#region [c-tor]
		public FoodInfoGetResponse(IReadOnlyCollection<FoodInfo> foodInfos)
		{
			FoodInfos = foodInfos;
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public IReadOnlyCollection<FoodInfo> FoodInfos { get; }
		#endregion
		#endregion
	}

	public class FoodInfo
	{
		public string CategoryName { get; set; }
	}
}