using System;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.Maps.Requests;
using GetTheFoodAlready.Api.Maps.Responses;

using MediatR;

namespace GetTheFoodAlready.Api.Maps
{
	/// <summary> Default implementation. </summary>
	public class MapService : IMapService
	{
		#region [Fields]
		private readonly IMediator _mediator;
		#endregion

		#region [c-tor]
		public MapService(IMediator mediator)
		{
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}
		#endregion

		#region IMapService implementation
		public Task<SuggestAddressResponse> SuggestAddress(SuggestAddressRequest request, CancellationToken cancellationToken)
		{
			return _mediator.Send(request, cancellationToken);
		}
		#endregion
	}
}