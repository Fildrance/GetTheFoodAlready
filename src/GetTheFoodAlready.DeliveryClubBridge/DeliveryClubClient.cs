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
	/// <summary> Wraps http client handler into decorators. </summary>
	/// <param name="nested">Original http-client.</param>
	/// <remarks>Can be used to add additional behaviour, like logging, or just replace original http-client-handler by mock.</remarks>
	/// <returns>Message handler to be used in HttpClient.</returns>
	public delegate HttpMessageHandler HttpClientHandlerProvider(HttpClientHandler nested);

	/// <summary> Default implementation of client. </summary>
	public class DeliveryClubClient : IDeliveryClubClient
	{
		#region  [Constants]
		private const string DeliveryApiBaseUrl = "https://api.delivery-club.ru";
		private const string DeliveryApiVersion = "api1.2";
		#endregion

		#region [Fields]
		private readonly HttpClient _httpClient;
		private readonly JsonSerializer _serializer;
		#endregion

		#region [c-tor]
		public DeliveryClubClient(JsonSerializer serializer, HttpClientHandlerProvider provider)
		{
			_serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

			var nestedHandler = new HttpClientHandler {CookieContainer = new CookieContainer(), UseCookies = true};
			var handlerToBeUsed = provider(nestedHandler);
			_httpClient = new HttpClient(handlerToBeUsed);
		}

		public void Dispose()
		{
			_httpClient?.Dispose();
		}
		#endregion

		#region IDeliveryClubClient implementation
		public async Task<RootDeliveryClubVendorsResponse> GetDeliveryClubVendorsNearby(
			decimal longitude,
			decimal latitude,
			CancellationToken cancellationToken = default(CancellationToken),
			int skip = 0,
			int take = 200
		)
		{
			var url = $"{DeliveryApiBaseUrl}/{DeliveryApiVersion}/vendors?limit={take}&offset={skip}"
			          + $"&latitude={latitude.ToString(CultureInfo.InvariantCulture)}&longitude={longitude.ToString(CultureInfo.InvariantCulture)}";

			// get session to operate further
			var sessionGetResult = await _httpClient.PostAsync("https://api.delivery-club.ru/api1.2/user/login", new StringContent(""), cancellationToken);
			sessionGetResult.EnsureSuccessStatusCode();

			var requestResult = await _httpClient.GetAsync(url, cancellationToken);
			requestResult.EnsureSuccessStatusCode();
			var requestContent = await requestResult.Content.ReadAsStringAsync();

			var vendorsResp = Deserialize<RootDeliveryClubVendorsResponse>(requestContent);
			return vendorsResp;
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