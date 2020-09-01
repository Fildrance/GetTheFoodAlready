using System;

namespace GetTheFoodAlready.Api.Support
{
	/// <summary> Specific to delivery time strings timespan parser. </summary>
	public interface ITimeSpanParser
	{
		/// <summary> Gets TimeSpan from present string. </summary>
		// todo: add format exceptions
		TimeSpan GetSpan(string timeAsString);
	}
}