using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.Maps.Requests;
using GetTheFoodAlready.Api.Maps.Responses;

namespace GetTheFoodAlready.Api.Maps
{
	/// <summary> Service for map, address and location interactions. </summary>
	public interface IMapService
	{
		/// <summary> Get list of suggestions for address autocompletion. </summary>
		/// <param name="request">Request with subscting of address.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>Response with address propositions.</returns>
		Task<SuggestAddressResponse> SuggestAddress(SuggestAddressRequest request, CancellationToken cancellationToken);
	}
}
