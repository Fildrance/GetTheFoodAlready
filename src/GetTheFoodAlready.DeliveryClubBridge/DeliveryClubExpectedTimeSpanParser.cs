using System;
using System.Collections.Generic;

using GetTheFoodAlready.Api.Support;

namespace GetTheFoodAlready.DeliveryClubBridge
{
	public class DeliveryClubExpectedTimeSpanParser : ITimeSpanParser
	{
		#region [Fields]
		private readonly Dictionary<string, int> _expectedTimeStringToMinutes = new Dictionary<string, int>
		{
			{ "25-35 мин", 35},
			{ "15-25 мин", 25},
			{ "30 мин", 30},
			{ "30-40 мин", 40},
			{ "20-30 мин", 30},
			{ "30-45 мин", 45},
			{ "35-45 мин", 45},
			{ "15-30 мин", 30},
			{ "50-60 мин", 60},
			{ "45-60 мин", 60},
			{ "45-55 мин", 60},
			{ "55-65 мин", 65},
			{ "55 мин", 55},
			{ "50 мин", 50},
			{ "40-50 мин", 50},
			{ "60 мин", 60},
			{ "1 час 40 мин", 100},
			{ "70 мин", 70},
			{ "90 мин", 90},
			{ "65 мин", 65},
			{ "45 мин", 45},
			{ "75 мин", 75},
			{ "50-70 мин", 70},
			{ "80 мин", 80},
			{ "85 мин", 85},
			{ "3 часа", 180},
			{ "в течение дня", 250},
			{ "2 часа", 120},
			{ "на следующий день", 24*60},
			{ "2 часа 25 мин", 155},
			{ "70-80 мин", 80},
			{ "1 час 50 мин", 110},
			{ "2 часа 20 мин", 140},
			{ "1 час 30 мин", 90},
			{ "1 час 35 мин", 95},
			{ "2 часа 30 мин", 150},
			{ "3 часа 20 мин", 300},
			{ "24 часа", 24*60 },
			{ "2 часа 15 мин", 135 }
		};
		#endregion

		#region [Public]
		#region [Public methods]
		public virtual TimeSpan GetSpan(string timeAsString)
		{
			// todo : replace with actual parsing.
			if (_expectedTimeStringToMinutes.TryGetValue(timeAsString, out var result))
			{
				return TimeSpan.FromMinutes(result);
			}
			throw new InvalidOperationException($"Failed to parse delivery time - unexpected symbol '{timeAsString}'!");
		}
		#endregion
		#endregion
	}
}