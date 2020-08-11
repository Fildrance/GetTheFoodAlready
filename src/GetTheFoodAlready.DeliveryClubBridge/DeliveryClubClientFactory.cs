using System;

using Newtonsoft.Json;

namespace GetTheFoodAlready.DeliveryClubBridge
{
	/// <summary> Default implementation. </summary>
	public class DeliveryClubClientFactory : IDeliveryClubClientFactory
	{
		#region [Fields]
		private readonly HttpClientHandlerProvider _provider;
		private readonly JsonSerializer _serializer;
		#endregion

		#region [c-tor]
		public DeliveryClubClientFactory(JsonSerializer serializer, HttpClientHandlerProvider provider)
		{
			_serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
			_provider = provider ?? throw new ArgumentNullException(nameof(provider));
		}
		#endregion

		#region IDeliveryClubClientFactory implementation
		public IDeliveryClubClient Create()
		{
			return new DeliveryClubClient(_serializer, _provider);
		}
		#endregion
	}
}