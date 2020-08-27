using System;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.Maps.Requests;
using GetTheFoodAlready.Api.Maps.Responses;
using GetTheFoodAlready.DaDataBridge;

using MediatR;

namespace GetTheFoodAlready.Handlers.Map
{
	public class SuggestAddressHandler : IRequestHandler<SuggestAddressRequest, SuggestAddressResponse>
	{
		#region [Fields]
		private readonly IDaDataClient _client;
		#endregion

		#region [c-tor]
		public SuggestAddressHandler(IDaDataClient client)
		{
			_client = client ?? throw new ArgumentNullException(nameof(client));
		}
		#endregion

		#region IRequestHandler<SuggestAddressRequest,SuggestAddressResponse> implementation
		public async Task<SuggestAddressResponse> Handle(SuggestAddressRequest request, CancellationToken cancellationToken)
		{
			var found = await _client.SuggestAddresses(request.Substring);

			return new SuggestAddressResponse(found);
		}
		#endregion
	}
}