using System;
using System.Collections.Generic;

using GetTheFoodAlready.Api.Support;

namespace GetTheFoodAlready.Api.Maps.Responses
{
	/// <summary> Response with address autocomplete suggestions </summary>
	public class SuggestAddressResponse
	{
		#region [c-tor]
		public SuggestAddressResponse(IReadOnlyCollection<AddressInfo> resolutionCandidates)
		{
			ResolutionCandidates = resolutionCandidates ?? throw new ArgumentNullException(nameof(resolutionCandidates));
		}
		#endregion

		#region [Public]
		#region [Public properties]
		/// <summary> List with suggested addresses. </summary>
		public IReadOnlyCollection<AddressInfo> ResolutionCandidates { get; }
		#endregion
		#endregion
	}
}