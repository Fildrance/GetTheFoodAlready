using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.FoodAgregators;
using GetTheFoodAlready.Api.FoodAgregators.Requests;
using GetTheFoodAlready.Api.FoodAgregators.Responses;
using GetTheFoodAlready.Api.Orchestration.Requests;

using NLog;

namespace GetTheFoodAlready.Handlers.RandomFoodRolling
{
	public class RerollIfEmptyFinalFoodResultsRandomFoodStrategy : IRandomFoodStrategy
	{
		#region [Static fields]
		private static readonly ILogger Logger = LogManager.GetLogger(typeof(RerollIfEmptyFinalFoodResultsRandomFoodStrategy).FullName);
		#endregion

		#region [Fields]
		private readonly IDeliveryClubService _deliveryClubService;
		private readonly int _maxFoodItems;
		private readonly int _maxRerollAttempts;

		private readonly Random _randomGenerator = new Random();
		#endregion

		#region [c-tor]
		public RerollIfEmptyFinalFoodResultsRandomFoodStrategy(
			IDeliveryClubService deliveryClubService,
			int maxFoodItems = 4,
			int maxRerollAttempts = 5
		)
		{
			_deliveryClubService = deliveryClubService;
			_maxFoodItems = maxFoodItems;
			_maxRerollAttempts = maxRerollAttempts;
		}
		#endregion

		#region IRandomFoodStrategy implementation
		public async Task<IReadOnlyDictionary<VendorInfo, IReadOnlyCollection<FoodInfo>>> RollVendor(RandomFoodPropositionsRequest request, IEnumerable<VendorInfo> vendorPoints,
			CancellationToken cancellationToken)
		{
			var rerollAttempts = 1;
			FoodInfoGetResponse foodInfoResponse;
			VendorInfo selectedVendor = null;
			bool needReroll;
			var vendors = vendorPoints.ToList();

			if (vendors.Count == 0)
			{
				throw new InvalidOperationException("Failed to get random vendor point - empty list of vendor pointswas passed.");
			}

			do
			{
				if (selectedVendor != null)
				{
					vendors.Remove(selectedVendor);
					rerollAttempts++;
				}

				if (vendors.Count == 0)
				{
					throw new InvalidOperationException("Failed to get random vendor point - no vendor point had supported food items.");
				}

				selectedVendor = GetRandomElement(vendors);

				var foodRequest = new FoodInfoGetRequest(selectedVendor.Id, request.FoodCategoryExceptions);
				foodInfoResponse = await _deliveryClubService.GetFoodInfo(foodRequest, cancellationToken);

				var foodItemsCount = foodInfoResponse.FoodInfos.Count;
				// if no items found - need reroll
				needReroll = foodItemsCount == 0;
				if (needReroll)
				{
					Logger.Trace($"Got no food items for vendor {selectedVendor.DisplayName}, have {rerollAttempts} reroll attempts.");
					// if reroll required - check if reroll attempts are available
					needReroll = rerollAttempts < _maxRerollAttempts;
				}
			}
			while (needReroll);

			if (foodInfoResponse.FoodInfos.Count == 0)
			{
				throw new InvalidOperationException($"Failed to get proper food info for random vendor! Attempted to get from {rerollAttempts} vendors, all returned empty data set.");
			}

			var result = new Dictionary<VendorInfo, IReadOnlyCollection<FoodInfo>>
			{
				{selectedVendor, foodInfoResponse.FoodInfos}
			};
			return result;
		}

		public IReadOnlyDictionary<VendorInfo, (IEnumerable<FoodInfo> totalList, IEnumerable<FoodInfo> proposedList)> RollFood(
			IReadOnlyDictionary<VendorInfo, IReadOnlyCollection<FoodInfo>> vendorRollResult
		)
		{
			if (vendorRollResult.Count == 0)
			{
				throw new ArgumentNullException(nameof(vendorRollResult));
			}

			if (vendorRollResult.Count > 1)
			{
				var message =
					"This strategy was not supposed to process more then one vendor point. This error can be caused by using different strategies to roll vendor points and food - it was not supposed to be used this way.";
				throw new ArgumentException(message, nameof(vendorRollResult));
			}

			var foodVendor = vendorRollResult.FirstOrDefault();
			var randomFoodList = new List<FoodInfo>();
			do
			{
				var randomFood = GetRandomElement(foodVendor.Value);
				randomFoodList.Add(randomFood);
			}
			while (randomFoodList.Count < _maxFoodItems);

			var results = new Dictionary<VendorInfo, (IEnumerable<FoodInfo> totalList, IEnumerable<FoodInfo> proposedList)>
			{
				{foodVendor.Key, (foodVendor.Value, randomFoodList)}
			};

			return results;
		}

		public bool CanAccept(RandomFoodPropositionsRequest request)
		{
			//todo: get value from request, fill value in ui dropbox.
			return true;
		}
		#endregion

		#region [Private]
		#region [Private methods]
		private T GetRandomElement<T>(IReadOnlyCollection<T> vendors)
		{
			var indexOfRandomElement = _randomGenerator.Next(vendors.Count);
			var selectedVendor = vendors.ElementAt(indexOfRandomElement);
			return selectedVendor;
		}
		#endregion
		#endregion
	}
}