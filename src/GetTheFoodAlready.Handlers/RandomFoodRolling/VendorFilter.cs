using System;
using System.Collections.Generic;
using System.Linq;

using GetTheFoodAlready.Api.FoodAgregators.Responses;
using GetTheFoodAlready.Api.Orchestration.Requests;
using GetTheFoodAlready.Api.Support;
using GetTheFoodAlready.DeliveryClubBridge;

namespace GetTheFoodAlready.Handlers.RandomFoodRolling
{
	/// <summary> Default implementation. </summary>
	public class VendorFilter : IVendorFilter
	{
		#region [Static methods]
		private static IEnumerable<VendorInfo> FilterByCuisineAndPaymentTypes(RandomFoodPropositionsRequest request, IEnumerable<VendorInfo> fullVendorsList)
		{
			var filtered = fullVendorsList;
			if (request.AcceptableCuisineTypes.Any())
			{
				filtered = filtered.Where(x => x.Cuisines.Any(c => request.AcceptableCuisineTypes.Contains(c)));
			}

			if (request.AcceptablePaymentTypes.Any())
			{
				filtered = filtered.Where(x => x.AvailablePaymentTypes.Any(c => request.AcceptablePaymentTypes.Contains(c)));
			}
				
			return filtered;
		}

		private static IEnumerable<VendorInfo> FilterByFreeDelivery(RandomFoodPropositionsRequest request, IEnumerable<VendorInfo> filtered)
		{
			if (request.IsSearchingOnlyFreeDeliveryMarked)
			{
				filtered = filtered.Where(x => x.IsDeliveringForFree);
			}

			return filtered;
		}

		private static IEnumerable<VendorInfo> FilterByRating(RandomFoodPropositionsRequest request, IEnumerable<VendorInfo> filtered)
		{
			var ratingInfo = request.RequiredRatingInfo;
			if (null != ratingInfo)
			{
				filtered = filtered.Where(x =>
					x.RatingScore != DeliveryClubConstants.NotEnoughRatingType
					&& x.ScoreCount >= ratingInfo.RateCount
					&& x.Score >= ratingInfo.MinimalRating
				);
			}

			return filtered;
		}
		#endregion

		#region [Fields]
		private readonly ITimeSpanParser _parser;
		#endregion

		#region [c-tor]
		public VendorFilter(ITimeSpanParser parser)
		{
			_parser = parser ?? throw new ArgumentNullException(nameof(parser));
		}
		#endregion

		#region IVendorFilter implementation
		public IEnumerable<VendorInfo> FilterVendors(RandomFoodPropositionsRequest request, IEnumerable<VendorInfo> fullVendorsList)
		{
			var filtered = FilterByCuisineAndPaymentTypes(request, fullVendorsList);
			filtered = FilterByDeliveryTime(request, filtered);
			filtered = FilterByFreeDelivery(request, filtered);
			filtered = FilterByRating(request, filtered);
			return filtered;
		}
		#endregion

		#region [Private]
		#region [Private methods]
		private IEnumerable<VendorInfo> FilterByDeliveryTime(RandomFoodPropositionsRequest request, IEnumerable<VendorInfo> filtered)
		{
			if (request.AcceptableDeliveryTimeTil.HasValue)
			{
				filtered = filtered.Where(x => {
					if (string.IsNullOrEmpty(x.DeliveryTime))
					{
						return true;
					}

					// accepts last number in time string of vendor as proposed delivery time (always consider worst outcome).
					var possibleTime = _parser.GetSpan(x.DeliveryTime);
					return request.AcceptableDeliveryTimeTil >= possibleTime;
				});
			}

			return filtered;
		}
		#endregion
		#endregion
	}
}