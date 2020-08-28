using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.DeliveryClubBridge.DataTypes;

using Newtonsoft.Json;

using NLog;

namespace GetTheFoodAlready.DeliveryClubBridge.Client
{
	/// <summary> Wraps http client handler into decorators. </summary>
	/// <param name="nested">Original http-client.</param>
	/// <remarks>Can be used to add additional behaviour, like logging, or just replace original http-client-handler by mock.</remarks>
	/// <returns>Message handler to be used in HttpClient.</returns>
	public delegate HttpMessageHandler HttpClientHandlerProvider(HttpClientHandler nested);

	/// <summary> Default implementation of client. </summary>
	public class DeliveryClubClient : IDeliveryClubClient
	{
		#region [Static fields]
		private static readonly ILogger Logger = LogManager.GetLogger(typeof(DeliveryClubClient).FullName);
		#endregion

		#region [Fields]
		private readonly DeliveryClubClientSettings _settings;
		#endregion

		#region [c-tor]
		public DeliveryClubClient(
			HttpClientHandlerProvider provider,
			DeliveryClubClientSettings deliveryClubClientSettings
		)
		{
			_settings = deliveryClubClientSettings ?? throw new ArgumentNullException(nameof(deliveryClubClientSettings));
			if (provider == null)
			{
				throw new ArgumentNullException(nameof(provider));
			}

			var nestedHandler = new HttpClientHandler
			{
				CookieContainer = CookieContainer,
				UseCookies = true
			};
			Logger.Trace("Asking for HttpMessageHandler from provider factory method.");
			var handlerToBeUsed = provider(nestedHandler);
			Logger.Trace($"Got '{handlerToBeUsed.GetType().Name}' as top level HttpMessageHandler from provider.");
			Client = new HttpClient(handlerToBeUsed);
		}
		#endregion

		#region IDeliveryClubClient implementation
		public virtual async Task<RootDeliveryClubVendorsResponse> GetDeliveryClubVendorsNearby(
			string longitude,
			string latitude,
			CancellationToken cancellationToken = default(CancellationToken),
			int skip = 0,
			int take = 200
		)
		{
			Logger.Debug("Getting list of closest delivery club vendors from delivery-club api.");
			var url = $"{_settings.ApiBaseUrl}/{_settings.ApiVersion}/vendors?limit={take}&offset={skip}"
			          + $"&latitude={latitude}&longitude={longitude}";
			Logger.Trace($"Url for request is '{url}'. \r\n Attempting to get list of vendor points.");
			var requestResult = await Client.GetAsync(url, cancellationToken);
			Logger.Trace(() => $"Get closest vendors result :\r\n {requestResult}");
			requestResult.EnsureSuccessStatusCode();

			Logger.Trace("Reading response content.");
			var requestContent = await requestResult.Content.ReadAsStringAsync();

			var vendorsResp = JsonConvert.DeserializeObject<RootDeliveryClubVendorsResponse>(requestContent);
			return vendorsResp;
		}

		public virtual async Task<DeliveryClubFoodInfo> GetFoodInfo(int vendorPointId, CancellationToken cancellationToken)
		{
			Logger.Debug("Getting list of food items from delivery-club api.");
			var url = $"{_settings.ApiBaseUrl}/{_settings.ApiVersion}/vendor/{vendorPointId}/menu?data=menu,products,actions";

			var requestResult = await Client.GetAsync(url, cancellationToken);
			Logger.Trace(() => $"Get menu for vendor with id '{vendorPointId}', got '{requestResult}'");
			requestResult.EnsureSuccessStatusCode();

			Logger.Trace("Reading response content.");
			var requestContent = await requestResult.Content.ReadAsStringAsync();

			var records = JsonConvert.DeserializeObject<DeliveryClubFoodInfo>(requestContent);
			return records;
		}
		public async Task<string> Login(CancellationToken cancellationToken, bool forceReLogin = false)
		{
			var deliveryClubCookies = CookieContainer.GetCookies(new Uri("https://www.delivery-club.ru/"));
			var sessionIdCookie = deliveryClubCookies["x_user_authorization"];
			if (forceReLogin == false
			    && sessionIdCookie?.Value != null)
			{
				return sessionIdCookie.Value;
			}

			Logger.Debug("Getting cookies to work with delivery-club api.");
			// get session to operate further
			var sessionGetResult = await Client.PostAsync($"{_settings.ApiBaseUrl}/{_settings.ApiVersion}/user/login", new StringContent(""), cancellationToken);
			Logger.Trace(() => $"Api login result is :\r\n {sessionGetResult}");
			sessionGetResult.EnsureSuccessStatusCode();
			var loginResponseContent = await sessionGetResult.Content
				.ReadAsStringAsync();

			var deserialized = JsonConvert.DeserializeAnonymousType(loginResponseContent, new {token = "", secret = ""});
			var token = deserialized.token;
			var secret = deserialized.secret;

			var authKey = $"{token}.{secret}";
			var xUserAuthCookie = new Cookie("x_user_authorization", authKey);
			deliveryClubCookies.Add(xUserAuthCookie);

			var str = deliveryClubCookies.OfType<Cookie>()
				.Where(cookie => cookie.Name == "PHPSESSID" || cookie.Name == "x_user_authorization")
				.ToArray();

			Client.DefaultRequestHeaders.Remove("x-user-authorization");
			Client.DefaultRequestHeaders.Remove("cookie");
			Client.DefaultRequestHeaders.Add("x-user-authorization", authKey);
			Client.DefaultRequestHeaders.Add("cookie", string.Join("; ", str.Select(x => $"{x.Name}={x.Value}")));

			return authKey;
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public HttpClient Client { get; }

		public CookieContainer CookieContainer { get; } = new CookieContainer();
		#endregion
		#endregion
	}
}