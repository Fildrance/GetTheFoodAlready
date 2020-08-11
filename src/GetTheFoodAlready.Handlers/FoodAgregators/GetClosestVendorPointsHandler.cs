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
	/// <summary> Handles request for closest vendor points. </summary>
	public class GetClosestVendorPointsHandler : IRequestHandler<GetClosestVendorPointsRequest, GetClosestVendorPointsResponse>
	{
		#region [Fields]
		private readonly IDeliveryClubClient _client;
		private readonly IMapper _mapper;
		#endregion

		#region [c-tor]
		public GetClosestVendorPointsHandler(
			IDeliveryClubClient client,
			IMapper mapper
		)
		{
			_client = client ?? throw new ArgumentNullException(nameof(client));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}
		#endregion

		#region IRequestHandler<GetClosestRestorauntsRequest,GetClosestRestorauntsResponse> implementation
		public async Task<GetClosestVendorPointsResponse> Handle(GetClosestVendorPointsRequest request, CancellationToken cancellationToken)
		{
				var vendorsResp = await _client.GetDeliveryClubVendorsNearby(request.Longitude, request.Latitude, cancellationToken);

				var vendors = vendorsResp.PagedList.Vendors;
				var mapped = vendors.Select(_mapper.Map<VendorInfo>)
					.ToArray();
				return new GetClosestVendorPointsResponse(mapped);
		}
		
		#endregion
	}
}