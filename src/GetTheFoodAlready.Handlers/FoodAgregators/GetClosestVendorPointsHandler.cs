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
		private readonly IDeliveryClubClientFactory _factory;
		private readonly IMapper _mapper;
		#endregion

		#region [c-tor]
		public GetClosestVendorPointsHandler(
			IDeliveryClubClientFactory factory,
			IMapper mapper
		)
		{
			_factory = factory ?? throw new ArgumentNullException(nameof(factory));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}
		#endregion

		#region IRequestHandler<GetClosestRestorauntsRequest,GetClosestRestorauntsResponse> implementation
		public async Task<GetClosestVendorPointsResponse> Handle(GetClosestVendorPointsRequest request, CancellationToken cancellationToken)
		{
			using (var client = _factory.Create())
			{
				var vendorsResp = await client.GetDeliveryClubVendorsNearby(request.Longitude, request.Latitude, cancellationToken);

				var vendors = vendorsResp.PagedList.Vendors;
				var mapped = vendors.Select(_mapper.Map<VendorInfo>)
					.ToArray();
				return new GetClosestVendorPointsResponse(mapped);
			}
		}
		
		#endregion
	}
}