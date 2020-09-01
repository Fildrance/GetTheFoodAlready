using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.FoodAgregators;
using GetTheFoodAlready.Api.FoodAgregators.Requests;
using GetTheFoodAlready.Api.Orchestration.Reponses;
using GetTheFoodAlready.Api.Orchestration.Requests;
using GetTheFoodAlready.Handlers.RandomFoodRolling;

using MediatR;

namespace GetTheFoodAlready.Handlers.Orchestration
{
	/// <summary> Orchestrates getting food info and forming random propositions. </summary>
	public class GetRandomFoodPropositionsHandler : IRequestHandler<RandomFoodPropositionsRequest, RandomFoodPropositionsResponse>
	{
		#region [Fields]
		private readonly IDeliveryClubService _deliveryClubService;
		private readonly IRandomFoodStrategyProvider _strategyProvider;
		private readonly IVendorFilter _vendorFilter;
		#endregion

		#region [c-tor]
		public GetRandomFoodPropositionsHandler(
			IDeliveryClubService deliveryClubService,
			IRandomFoodStrategyProvider strategyProvider, 
			IVendorFilter vendorFilter
		)
		{
			_deliveryClubService = deliveryClubService ?? throw new ArgumentNullException(nameof(deliveryClubService));
			_strategyProvider = strategyProvider ?? throw new ArgumentNullException(nameof(strategyProvider));
			_vendorFilter = vendorFilter ?? throw new ArgumentNullException(nameof(vendorFilter));
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
			var filteredVendorPoints = _vendorFilter.FilterVendors(request, closestVendorPointsResponse.Vendors);

			var strategy = _strategyProvider.GetStrategy(request);
			var rollVendorResult = await strategy.RollVendor(request, filteredVendorPoints.ToList(), cancellationToken);
			var proposedFood = strategy.RollFood(rollVendorResult);

			return new RandomFoodPropositionsResponse(proposedFood);
		}
		#endregion
	}
}