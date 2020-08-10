using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.DeliveryClubBridge.DataTypes;

using Newtonsoft.Json;

namespace GetTheFoodAlready.DeliveryClubBridge
{
	public class DeliveryClubClient : IDeliveryClubClient
	{
		#region  [Constants]
		private const string DeliveryApiBaseUrl = "https://api.delivery-club.ru";
		private const string DeliveryApiVersion = "api1.2";
		#endregion

		#region [Fields]
		private readonly JsonSerializer _serializer;
		#endregion

		#region [c-tor]
		public DeliveryClubClient(JsonSerializer serializer)
		{
			_serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
		}
		#endregion

		#region IDeliveryClubClient implementation
		public async Task<RootDeliveryClubVendorsResponse> GetDeliveryClubVendorsNearby(decimal longitude, decimal latitude, CancellationToken cancellationToken, int skip = 0, int take = 100)
		{
			var cookieContainer = new CookieContainer();
			var newHandler = new HttpClientHandler {CookieContainer = cookieContainer, UseCookies = true};
			using (var client = new HttpClient(newHandler))
			{
				
				var url = $"{DeliveryApiBaseUrl}/{DeliveryApiVersion}/vendors?limit={take}&offset={skip}&latitude={latitude.ToString(CultureInfo.InvariantCulture)}&longitude={longitude.ToString(CultureInfo.InvariantCulture)}";

				// get session to operate further
				var r = await client.PostAsync("https://api.delivery-club.ru/api1.2/user/login", new StringContent(""), cancellationToken);
				var requestResult = await client.GetAsync(url, cancellationToken);
				requestResult.EnsureSuccessStatusCode();
				var requestContent = await requestResult.Content.ReadAsStringAsync();

				var vendorsResp = Deserialize<RootDeliveryClubVendorsResponse>(requestContent);
				return vendorsResp;
			}
		}
		#endregion

		#region [Private]
		#region [Private methods]
		private T Deserialize<T>(string content)
		{
			var stringReader = new StringReader(content);
			var jsonTextReader = new JsonTextReader(stringReader);
			return _serializer.Deserialize<T>(jsonTextReader);
		}
		#endregion
		#endregion
	}
}