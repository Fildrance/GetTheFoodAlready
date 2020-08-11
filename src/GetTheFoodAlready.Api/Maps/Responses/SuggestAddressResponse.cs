using System;
using System.Collections.Generic;

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

	public class AddressInfo
	{
		#region [c-tor]
		public AddressInfo(string addressName, string latitude, string longitude)
		{
			AddressName = addressName;
			Latitude = latitude;
			Longitude = longitude;
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public string AddressName { get;  }
		public string Latitude { get; }
		public string Longitude { get; }
		#endregion
		#endregion
	}
}