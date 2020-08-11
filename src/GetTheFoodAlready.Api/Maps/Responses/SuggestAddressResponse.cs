using System;
using System.Collections.Generic;

namespace GetTheFoodAlready.Api.Maps.Responses
{
	/// <summary> Response with address autocomplete suggestions </summary>
	public class SuggestAddressResponse
	{
		#region [c-tor]
		public SuggestAddressResponse(IReadOnlyCollection<string> resolutionCandidates)
		{
			ResolutionCandidates = resolutionCandidates??throw new ArgumentNullException(nameof(resolutionCandidates));
		}
		#endregion

		#region [Private]
		#region [Private properties]
		/// <summary> List with suggested addresses. </summary>
		public IReadOnlyCollection<string> ResolutionCandidates { get; }
		#endregion
		#endregion
	}
}
