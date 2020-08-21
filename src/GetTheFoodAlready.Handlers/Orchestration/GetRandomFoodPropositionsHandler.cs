using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
		#region [Static fields]
		private static readonly char[] Separators = {' ', '-'};
		#endregion

		#region [Static methods]
		// todo: move filtering... cannot be placed in orchestration, its different level.
		private static IEnumerable<VendorInfo> FilterVendors(RandomFoodPropositionsRequest request, IEnumerable<VendorInfo> fullVendorsList)
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
					var possbleTimeString = x.DeliveryTime
						.Split(Separators, StringSplitOptions.RemoveEmptyEntries)
						.LastOrDefault(s => Regex.IsMatch(s, @"\d"));
					if (possbleTimeString == null)
					{
						return true;
					}
					var possibleTime = int.Parse(possbleTimeString);
					return request.AcceptableDeliveryTimeTil < possibleTime;
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
		#endregion

		#region [Fields]
		private readonly IDeliveryClubService _deliveryClubService;
		#endregion

		#region [c-tor]
		public GetRandomFoodPropositionsHandler(IDeliveryClubService deliveryClubService)
		{
			_deliveryClubService = deliveryClubService ?? throw new ArgumentNullException(nameof(deliveryClubService));
		}
		#endregion

		#region IRequestHandler<RandomFoodPropositionsRequest,RandomFoodPropositionsResponse> implementation
		public async Task<RandomFoodPropositionsResponse> Handle(RandomFoodPropositionsRequest request, CancellationToken cancellationToken)
		{
			var addressInfo = request.AddressInfo;
			var getClosestVendorPointsRequest = new ClosestVendorPointsGetRequest
			{
				Latitude = addressInfo.Latitude,
				Longitude = addressInfo.Longitude 
			};
			var closestVendorPointsResponse = await _deliveryClubService.GetClosestVendorPoints(getClosestVendorPointsRequest, cancellationToken);

			var filtered = FilterVendors(request, closestVendorPointsResponse.Vendors)
				.ToArray();

			var foodInfoResponses = filtered.Select(x => _deliveryClubService.GetFoodInfo(new FoodInfoGetRequest(x.Id, request.FoodCategoryExceptions), cancellationToken));
			var foodInfos = await Task.WhenAll(foodInfoResponses);
			Console.WriteLine(foodInfos.Length);

			return new RandomFoodPropositionsResponse();
		}
		#endregion
	}
}