using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using GetTheFoodAlready.Api.FoodAgregators.Requests;
using GetTheFoodAlready.Api.FoodAgregators.Responses;
using GetTheFoodAlready.DeliveryClubBridge;

using MediatR;

namespace GetTheFoodAlready.Handlers.FoodAgregators
{
	public class GetClosestRestorauntsHandler : IRequestHandler<GetClosestVendorPointsRequest, GetClosestVendorPointsResponse>
	{
		#region [Fields]
		private readonly IDeliveryClubClient _deliveryClubClient;
		private readonly IMapper _mapper;
		#endregion

		#region [c-tor]
		public GetClosestRestorauntsHandler(
			IDeliveryClubClient deliveryClubClient,
			IMapper mapper
		)
		{
			_deliveryClubClient = deliveryClubClient ?? throw new ArgumentNullException(nameof(deliveryClubClient));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}
		#endregion

		#region IRequestHandler<GetClosestRestorauntsRequest,GetClosestRestorauntsResponse> implementation
		public async Task<GetClosestVendorPointsResponse> Handle(GetClosestVendorPointsRequest request, CancellationToken cancellationToken)
		{
			var coordinatesLatitude = 55.867051m;
			var coordinatesLongitude = 37.594261m;

			var vendorsResp = await _deliveryClubClient.GetDeliveryClubVendorsNearby(coordinatesLongitude, coordinatesLatitude, cancellationToken);

			var vendors = vendorsResp.PagedList.Vendors;
			var mapped = vendors.Select(_mapper.Map<VendorInfo>)
				.ToArray();
			return new GetClosestVendorPointsResponse(mapped);
		}
		
		#endregion
	}
}