using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dadata;

namespace GetTheFoodAlready.DaDataBridge
{
	/// <summary> Default implementation. </summary>
	public class DaDataClient : IDaDataClient
	{
		#region [Fields]
		private readonly SuggestClientAsync _api;
		#endregion

		#region [c-tor]
		public DaDataClient(string token)
		{
			if (string.IsNullOrEmpty(token))
			{
				throw new ArgumentNullException(nameof(token));
			}
			_api = new SuggestClientAsync(token);
		}
		#endregion

		#region IDaDataClient implementation
		public async Task<IReadOnlyCollection<string>> SuggestAddresses(string subString)
		{
			var response = await _api.SuggestAddress(subString);
			var addresses = response.suggestions.Select(x => x.value);
			return addresses.ToArray();
		}
		#endregion
	}
}