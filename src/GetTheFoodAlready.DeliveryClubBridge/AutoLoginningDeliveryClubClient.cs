using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.DeliveryClubBridge.DataTypes;

using Newtonsoft.Json;

using NLog;

namespace GetTheFoodAlready.DeliveryClubBridge
{
	public class AutoLoginningDeliveryClubClient : DeliveryClubClient
	{
		#region [Static fields]
		private static readonly ILogger Logger = LogManager.GetLogger(typeof(AutoLoginningDeliveryClubClient).FullName);
		#endregion

		#region [c-tor]
		public AutoLoginningDeliveryClubClient(HttpClientHandlerProvider provider) : base(provider)
		{
		}
		#endregion

		#region [Public]
		#region [Public methods]
		public override async Task<RootDeliveryClubVendorsResponse> GetDeliveryClubVendorsNearby(string longitude, string latitude, CancellationToken cancellationToken = default(CancellationToken), int skip = 0, int take = 200)
		{
			await EnsureClientLoggedOn(false, cancellationToken);

			try
			{
				return await base.GetDeliveryClubVendorsNearby(longitude, latitude, cancellationToken, skip, take);
			}
			catch (Exception e)
			{
				if (e.Message.Contains("401"))
				{
					await EnsureClientLoggedOn(true, cancellationToken);
					return await base.GetDeliveryClubVendorsNearby(longitude, latitude, cancellationToken, skip, take);
				}
				throw;
			}
			
		}
		public override async Task<DeliveryClubFoodInfo> GetFoodInfo(int vendorPointId, CancellationToken cancellationToken)
		{
			await EnsureClientLoggedOn(false, cancellationToken);
			try
			{
				return await base.GetFoodInfo(vendorPointId, cancellationToken);
			}
			catch (HttpRequestException e)
			{
				if (e.Message.Contains("401"))
				{
					await EnsureClientLoggedOn(true, cancellationToken);
					return await base.GetFoodInfo(vendorPointId, cancellationToken);
				}
				throw;
			}
			
		}
		#endregion
		#endregion

		#region [Private]
		#region [Private methods]
		private async Task<string> EnsureClientLoggedOn(bool forceLogin, CancellationToken cancellationToken)
		{
			var deliveryClubCookies = CookieContainer.GetCookies(new Uri("https://www.delivery-club.ru/"));
			var sessionIdCookie = deliveryClubCookies["x_user_authorization"];
			if (forceLogin == false && sessionIdCookie?.Value != null)
			{
				return sessionIdCookie.Value;
			}
			Logger.Debug("Getting cookies to work with delivery-club api.");
			// get session to operate further
			var sessionGetResult = await HttpClient.PostAsync($"{DeliveryApiBaseUrl}/{DeliveryApiVersion}/user/login", new StringContent(""), cancellationToken);
			Logger.Trace(() => $"Api login result is :\r\n {sessionGetResult}");
			sessionGetResult.EnsureSuccessStatusCode();
			var loginResponseContent = await sessionGetResult.Content
				.ReadAsStringAsync();

			var deserialized = JsonConvert.DeserializeAnonymousType(loginResponseContent, new { token = "", secret = "" });
			var token = deserialized.token;
			var secret = deserialized.secret;

			var authKey = $"{token}.{secret}";
			var xUserAuthCookie = new Cookie("x_user_authorization", authKey);
			deliveryClubCookies.Add(xUserAuthCookie);

			var str = deliveryClubCookies.OfType<Cookie>()
				.Where(cookie => cookie.Name == "PHPSESSID" || cookie.Name == "x_user_authorization")
				.ToArray();

			HttpClient.DefaultRequestHeaders.Remove("x-user-authorization");
			HttpClient.DefaultRequestHeaders.Remove("cookie");
			HttpClient.DefaultRequestHeaders.Add("x-user-authorization", authKey);
			HttpClient.DefaultRequestHeaders.Add("cookie", string.Join("; ", str.Select(x => $"{x.Name}={x.Value}")));

			return authKey;
		}
		#endregion
		#endregion
	}
}
