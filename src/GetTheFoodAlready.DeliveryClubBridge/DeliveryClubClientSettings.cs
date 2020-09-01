using System;

namespace GetTheFoodAlready.DeliveryClubBridge
{
	public class DeliveryClubClientSettings
	{
		public DeliveryClubClientSettings(string apiBaseUrl, string apiVersion)
		{
			if (string.IsNullOrEmpty(apiBaseUrl))
			{
				throw new ArgumentNullException(nameof(apiBaseUrl));
			}

			if (string.IsNullOrEmpty(apiVersion))
			{
				throw new ArgumentNullException(nameof(apiVersion));
			}
			ApiBaseUrl = apiBaseUrl;
			ApiVersion = apiVersion;
		}

		public string ApiBaseUrl { get; } 
		public string ApiVersion { get; } 
	}
}
