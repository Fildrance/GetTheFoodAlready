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
		public string ProductName { get; set; }
		public string CategoryName { get; set; }
		public string Description { get; set; }
		public string Price { get; set; }
		public string Volume { get; set; }
	}
}