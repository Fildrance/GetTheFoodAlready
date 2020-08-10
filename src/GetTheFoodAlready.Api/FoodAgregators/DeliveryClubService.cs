using System;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.FoodAgregators.Requests;
using GetTheFoodAlready.Api.FoodAgregators.Responses;

using MediatR;

namespace GetTheFoodAlready.Api.FoodAgregators
{
	public class DeliveryClubService : IDeliveryClubService
	{
		#region [Fields]
		private readonly IMediator _mediator;
		#endregion

		#region [c-tor]
		public DeliveryClubService(IMediator mediator)
		{
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}
		#endregion

		#region [Public]
		#region [Public methods]
		public Task<GetClosestVendorPointsResponse> GetClosestVendorPoints(GetClosestVendorPointsRequest request, CancellationToken cancellationToken)
		{
			return _mediator.Send(request, cancellationToken);
		}
		#endregion
		#endregion
	}
}
