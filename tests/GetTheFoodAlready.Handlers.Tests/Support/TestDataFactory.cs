using System;

using GetTheFoodAlready.Api.Orchestration.Requests;
using GetTheFoodAlready.Api.Support;

namespace GetTheFoodAlready.Handlers.Tests.Support
{
	public static class TestDataFactory
	{
		public static RandomFoodPropositionsRequest CreateRequest(
			string latitude = "12.45345",
			string longitude = "54.45134",
			RatingInfo rating = null,
			bool freeDelivery = false,
			TimeSpan? deliveryTime = null,
			string[] cuisines = null,
			string[] paymentTypes = null
		)
		{
			return new RandomFoodPropositionsRequest(
				new AddressInfo("Name", latitude, longitude), 
				deliveryTime, 
				cuisines ?? new string[0], 
				paymentTypes ?? new string[0], 
				freeDelivery, 
				null, 
				rating, 
				new string[0]
			);
		}
	}
}
