using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.FoodAgregators;
using GetTheFoodAlready.Api.FoodAgregators.Requests;
using GetTheFoodAlready.Api.FoodAgregators.Responses;
using GetTheFoodAlready.Api.Orchestration.Reponses;
using GetTheFoodAlready.Api.Orchestration.Requests;
using GetTheFoodAlready.DeliveryClubBridge;

using MediatR;

namespace GetTheFoodAlready.Handlers.Orchestration
{
	/// <summary> Orchestrates getting food info and forming random propositions. </summary>
	public class GetRandomFoodPropositionsHandler : IRequestHandler<RandomFoodPropositionsRequest, RandomFoodPropositionsResponse>
	{
		#region  [Constants]
		//todo: replace with injection
		private const int MaxRerollAttempts = 5;
		#endregion

		#region [Fields]
		private readonly IDeliveryClubService _deliveryClubService;
		private readonly DeliveryClubExpectedTimeParser _parser;
		private readonly Random _randomGenerator = new Random();
		#endregion

		#region [c-tor]
		public GetRandomFoodPropositionsHandler(
			IDeliveryClubService deliveryClubService, 
			DeliveryClubExpectedTimeParser parser
		)
		{
			_deliveryClubService = deliveryClubService ?? throw new ArgumentNullException(nameof(deliveryClubService));
			_parser = parser ?? throw new ArgumentNullException(nameof(deliveryClubService));
		}
		#endregion

		#region IRequestHandler<RandomFoodPropositionsRequest,RandomFoodPropositionsResponse> implementation
		public async Task<RandomFoodPropositionsResponse> Handle(RandomFoodPropositionsRequest request, CancellationToken cancellationToken)
		{
			var addressInfo = request.AddressInfo;
			var closestVendorPointsRequest = new ClosestVendorPointsGetRequest
			{
				Latitude = addressInfo.Latitude,
				Longitude = addressInfo.Longitude 
			};
			var closestVendorPointsResponse = await _deliveryClubService.GetClosestVendorPoints(closestVendorPointsRequest, cancellationToken);

			var filtered = FilterVendors(request, closestVendorPointsResponse.Vendors)
				.ToList();

			int rerollAttempts = 0;
			FoodInfoGetResponse foodInfoResponse;
			VendorInfo selectedVendor = null;
			bool needReroll;
			do
			{
				if (selectedVendor != null)
				{
					filtered.Remove(selectedVendor);
					rerollAttempts++;
				}
				var indexOfRandomElement = _randomGenerator.Next(filtered.Count);
				selectedVendor = filtered[indexOfRandomElement];

				var foodRequest = new FoodInfoGetRequest(selectedVendor.Id, request.FoodCategoryExceptions);
				foodInfoResponse = await _deliveryClubService.GetFoodInfo(foodRequest, cancellationToken);

				var foodItemsCount = foodInfoResponse.FoodInfos.Count;
				// if no items found - need reroll
				needReroll = foodItemsCount == 0;
				if (needReroll)
				{
					// if reroll required - check if reroll attempts are available
					needReroll = rerollAttempts < MaxRerollAttempts;
				}
			}
			while (needReroll);

			if (foodInfoResponse.FoodInfos.Count == 0)
			{
				throw new InvalidOperationException($"Failed to get proper food info for random vendor! Attempted to get from {rerollAttempts} vendors, all returned empty data set.");
			}

			var proposedFood = SelectRandomFood(foodInfoResponse.FoodInfos);

			return new RandomFoodPropositionsResponse(
				foodInfoResponse.FoodInfos,
				proposedFood,
				selectedVendor
			);
		}
		#endregion

		#region [Private]
		#region [Private methods]
		// todo: move filtering... cannot be placed in orchestration, its different level.
		private IEnumerable<VendorInfo> FilterVendors(RandomFoodPropositionsRequest request, IEnumerable<VendorInfo> fullVendorsList)
		{
			var filtered = fullVendorsList.Where(x => !request.AcceptableCuisineTypes.Except(x.Cuisines).Any())
				.Where(x => !request.AcceptablePaymentTypes.Except(x.AvailablePaymentTypes).Any());
			if (request.AcceptableDeliveryTimeTil.HasValue)
			{
				filtered = filtered.Where(x => {
					if (string.IsNullOrEmpty(x.DeliveryTime))
					{
						return true;
					}

					// accepts last number in time string of vendor as proposed delivery time (always consider worst outcome).
					var possibleTime = _parser.GetPessimisticDeliveryTime(x.DeliveryTime);
					return request.AcceptableDeliveryTimeTil >= possibleTime;
				});
			}

			if (request.IsSearchingOnlyFreeDeliveryMarked)
			{
				filtered = filtered.Where(x => x.IsDeliveringForFree);
			}

			var ratingInfo = request.RequiredRatingInfo;
			if (null != ratingInfo)
			{
				filtered = filtered.Where(x =>
					x.ScoreCount >= ratingInfo.RateCount
					&& x.Score >= ratingInfo.MinimalRating
					&& x.RatingScore != DeliveryClubConstants.NotEnoughRatingType
				);
			}

			return filtered;
		}

		private IReadOnlyCollection<FoodInfo> SelectRandomFood(IReadOnlyCollection<FoodInfo> foodInfos)
		{
			var totalCount = foodInfos.Count;
			IList<FoodInfo> randomFoodList = new List<FoodInfo>();
			var maxFood = 4;
			do
			{
				var randomIndex = _randomGenerator.Next(totalCount);
				var randomFood = foodInfos.ElementAt(randomIndex);
				randomFoodList.Add(randomFood);
			}
			while (randomFoodList.Count < maxFood);

			return randomFoodList.ToArray();
		}
		#endregion
		#endregion
	}
}