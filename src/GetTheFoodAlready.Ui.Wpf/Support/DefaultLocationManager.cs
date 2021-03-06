﻿using System;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.Support;

using Newtonsoft.Json;

using NLog;

namespace GetTheFoodAlready.Ui.Wpf.Support
{
	/// <summary> Default implementation. </summary>
	public class DefaultLocationManager : DefaultManager<AddressInfo>
	{
		#region [Static fields]
		private static readonly ILogger Logger = LogManager.GetLogger(typeof(DefaultLocationManager).FullName);
		#endregion

		#region IDefaultLocationManager implementation
		public override async Task<AddressInfo> GetDefault()
		{
			var defaulResult = await base.GetDefault();
			if (null != defaulResult)
			{
				return defaulResult;
			}

			var client = new HttpClient();

			var currentStaticIp = await WrapGet(client, "http://checkip.dyndns.org");
			if (null == currentStaticIp)
			{
				return null;
			}

			var ip = ExtractIp(currentStaticIp);
			if (null == ip)
			{
				return null;
			}

			var getIpInfoContent = await WrapGet(client, "http://ipinfo.io/" + ip);
			if (null == getIpInfoContent)
			{
				return null;
			}

			IpInfo ipInfo;
			try
			{
				ipInfo = JsonConvert.DeserializeObject<IpInfo>(getIpInfoContent);
			}
			catch (JsonSerializationException)
			{
				Logger.Error("Failed to find default location - ipinfo.io returned un-de-serializable string as content.");
				return null;
			}

			var loc = ipInfo.Loc.Split(',');
			var lat = loc[0];
			var lng = loc[1];
			Validate(lat, lng);

			var found = new AddressInfo("", lat, lng);

			return found;
		}
		#endregion

		#region [Private]
		#region [Private methods]
		private string ExtractIp(string currentStaticIpResp)
		{
			var match = Regex.Match(currentStaticIpResp, @"(?'ip'\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b)");
			var group = match.Groups["ip"];
			if (group.Success)
			{
				return group.Value;
			}

			Logger.Error($"Failed to find default location - checkip.dyndns.org returned unexpected content - '{currentStaticIpResp}'.");
			return null;
		}
		private void Validate(string lat, string lng)
		{
			try
			{
				var invariantCulture = CultureInfo.InvariantCulture;
				decimal.Parse(lat, invariantCulture);
				decimal.Parse(lng, invariantCulture);
			}
			catch (FormatException)
			{
				throw new InvalidOperationException("Failed to acquire data on your current IP, ipinfo.io returned not valid longitude and latitude!");
			}
		}

		private async Task<string> WrapGet(HttpClient client, string url)
		{
			try
			{
				var get = await client.GetAsync(url);
				var content = await get.EnsureSuccessStatusCode()
					.Content.ReadAsStringAsync();
				return content;
			}
			catch (HttpRequestException httpEx)
			{
				Logger.Error(httpEx, $"Failed to get correct response from external service '{url}'");
				return null;
			}
		}
		#endregion
		#endregion

		#region [Inner classes]
		public class IpInfo
		{
			#region [Public]
			#region [Public properties]
			[JsonProperty("loc")]
			public string Loc { get; set; }
			#endregion
			#endregion
		}
		#endregion
	}
}