namespace GetTheFoodAlready.Api.Support
{
	public class AddressInfo
	{
		#region [c-tor]
		public AddressInfo(string addressName, string latitude, string longitude)
		{
			AddressName = addressName;
			Latitude = latitude;
			Longitude = longitude;
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public string AddressName { get;  }
		public string Latitude { get; }
		public string Longitude { get; }
		#endregion
		#endregion
	}
}