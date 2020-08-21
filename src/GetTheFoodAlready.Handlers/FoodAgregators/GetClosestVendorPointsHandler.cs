using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using GetTheFoodAlready.Api.FoodAgregators.Requests;
using GetTheFoodAlready.Api.FoodAgregators.Responses;
using GetTheFoodAlready.DeliveryClubBridge;
using GetTheFoodAlready.DeliveryClubBridge.DataTypes;

using MediatR;

using NLog;

namespace GetTheFoodAlready.Handlers.FoodAgregators
{
	/// <summary> Handles request for closest vendor points. </summary>
	public class GetClosestVendorPointsHandler : IRequestHandler<ClosestVendorPointsGetRequest, ClosestVendorPointsGetResponse>
	{
		#region [Static fields]
		private static readonly ILogger Logger = LogManager.GetLogger(typeof(GetClosestVendorPointsHandler).FullName);
		#endregion

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

		#region IRequestHandler<GetClosestVendorPointsRequest,GetClosestVendorPointsResponse> implementation
		public async Task<ClosestVendorPointsGetResponse> Handle(ClosestVendorPointsGetRequest request, CancellationToken cancellationToken)
		{
			var skip = 0;
			var take = 100;
			bool hasMore;
			var vendors = new List<DeliveryClubVendor>();
			do
			{
				Logger.Debug($"Attempting to acquire closest vendor points info, lng:{request.Longitude}, lat: {request.Latitude}, taking {take}, skipping {skip}");
				var vendorsResp = await _client.GetDeliveryClubVendorsNearby(request.Longitude, request.Latitude, cancellationToken, skip, take);
				Logger.Trace($"Acquired data {vendorsResp.PagedList}.");
				vendors.AddRange(vendorsResp.PagedList.Vendors);
				skip = skip + take;
				hasMore = vendorsResp.PagedList.HasMore;
			}
			while (hasMore);

			var mapped = vendors.Select(_mapper.Map<VendorInfo>)
				.ToArray();
			return new ClosestVendorPointsGetResponse(mapped);
		}
		#endregion
	}
}