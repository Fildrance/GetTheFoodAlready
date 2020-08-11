using GetTheFoodAlready.Api.Maps.Responses;

using MediatR;

namespace GetTheFoodAlready.Api.Maps.Requests
{
	/// <summary> Request for address autocomplete suggestions. </summary>
	public class SuggestAddressRequest : IRequest<SuggestAddressResponse>
	{
		public string Substring { get; set; }
	}
}
