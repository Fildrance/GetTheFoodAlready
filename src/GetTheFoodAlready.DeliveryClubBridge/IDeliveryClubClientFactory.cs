namespace GetTheFoodAlready.DeliveryClubBridge
{
	/// <summary> Factory for deliver club api clients. </summary>
	public interface IDeliveryClubClientFactory
	{
		/// <summary> Creates new instance of delivery club api client. </summary>
		IDeliveryClubClient Create();
	}
}
