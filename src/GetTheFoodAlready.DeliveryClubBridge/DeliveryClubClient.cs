using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.DeliveryClubBridge.DataTypes;

using Newtonsoft.Json;

using NLog;

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

		#region [Static fields]
		private static readonly ILogger Logger = LogManager.GetLogger(typeof(DeliveryClubClient).FullName);
		#endregion

		#region [Fields]
		private readonly HttpClient _httpClient;
		#endregion

		#region [c-tor]
		public DeliveryClubClient(HttpClientHandlerProvider provider)
		{
			var nestedHandler = new HttpClientHandler
			{
				CookieContainer = new CookieContainer(),
				UseCookies = true
			};
			Logger.Trace("Asking for HttpMessageHandler from provider factory method.");
			var handlerToBeUsed = provider(nestedHandler);
			Logger.Trace($"Got '{handlerToBeUsed.GetType().Name}' as top level HttpMessageHandler from provider.");
			_httpClient = new HttpClient(handlerToBeUsed);
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
			Logger.Debug("Getting cookies to work with delivery-club api.");
			// get session to operate further
			var sessionGetResult = await _httpClient.PostAsync($"{DeliveryApiBaseUrl}/{DeliveryApiVersion}/user/login", new StringContent(""), cancellationToken);
			Logger.Trace(() => $"Api login result is :\r\n {sessionGetResult}");
			sessionGetResult.EnsureSuccessStatusCode();

			Logger.Debug("Getting list of closest delivery club vendors from delivery-club api.");
			var url = $"{DeliveryApiBaseUrl}/{DeliveryApiVersion}/vendors?limit={take}&offset={skip}"
			          + $"&latitude={latitude.ToString(CultureInfo.InvariantCulture)}&longitude={longitude.ToString(CultureInfo.InvariantCulture)}";
			Logger.Trace($"Url for request is '{url}'. \r\n Attempting to get list of vendor points.");
			var requestResult = await _httpClient.GetAsync(url, cancellationToken);
			Logger.Trace(() => $"Get closest vendors result :\r\n {requestResult}");
			requestResult.EnsureSuccessStatusCode();
			Logger.Trace("Reading response content.");
			var requestContent = await requestResult.Content.ReadAsStringAsync();

			var vendorsResp = JsonConvert.DeserializeObject<RootDeliveryClubVendorsResponse>(requestContent);
			return vendorsResp;
		}
		#endregion

	}
}