using System.Collections.Generic;
using System.Threading.Tasks;

namespace GetTheFoodAlready.DaDataBridge
{
	/// <summary>
	/// Client for DaData api. Can interact with addresses database ect.
	/// </summary>
	public interface IDaDataClient
	{
		/// <summary>
		/// Gets suggestions for addresses that look like presented substring.
		/// </summary>
		/// <param name="subString">String that contains part of requested address.</param>
		/// <returns>List of suggested addresses.</returns>
		Task<IReadOnlyCollection<string>> SuggestAddresses(string subString);
	}
}