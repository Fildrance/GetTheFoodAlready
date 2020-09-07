using System;
using System.Collections.Generic;

using GetTheFoodAlready.Api.Orchestration.Responses;
using GetTheFoodAlready.Api.Support;

using MediatR;

namespace GetTheFoodAlready.Api.Orchestration.Requests
{
	/// <summary> Request for random food propositions. </summary>
	public class RandomFoodPropositionsRequest : IRequest<RandomFoodPropositionsResponse>
	{
		public RandomFoodPropositionsRequest(
			AddressInfo addressInfo,
			TimeSpan? acceptableDeliveryTimeTil,
			IReadOnlyCollection<string> acceptableCuisineTypes,
			IReadOnlyCollection<string> acceptablePaymentTypes,
			bool isSearchingOnlyFreeDeliveryMarked,
			int? minimumOrderAmount,
			RatingInfo requiredRatingInfo, 
			IReadOnlyCollection<string> foodCategoryExceptions
		)
		{
			AddressInfo = addressInfo;
			AcceptableDeliveryTimeTil = acceptableDeliveryTimeTil;
			AcceptableCuisineTypes = acceptableCuisineTypes;
			AcceptablePaymentTypes = acceptablePaymentTypes;
			IsSearchingOnlyFreeDeliveryMarked = isSearchingOnlyFreeDeliveryMarked;
			MinimumOrderAmount = minimumOrderAmount;
			RequiredRatingInfo = requiredRatingInfo;
			FoodCategoryExceptions = foodCategoryExceptions;
		}

		public AddressInfo AddressInfo { get; }
		public TimeSpan? AcceptableDeliveryTimeTil { get; }
		public IReadOnlyCollection<string> AcceptableCuisineTypes { get; }
		public IReadOnlyCollection<string> AcceptablePaymentTypes { get; }
		public IReadOnlyCollection<string> FoodCategoryExceptions { get; }
		public bool IsSearchingOnlyFreeDeliveryMarked { get; }
		public int? MinimumOrderAmount { get; }
		public RatingInfo RequiredRatingInfo { get; }
	}

	public class RatingInfo
	{
		public RatingInfo(decimal minimalRating, int rateCount)
		{
			MinimalRating = minimalRating;
			RateCount = rateCount;
		}
		public decimal MinimalRating { get; }
		public int RateCount { get; }
	}
}