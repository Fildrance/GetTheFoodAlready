namespace GetTheFoodAlready.DeliveryClubBridge
{
	public static class DeliveryClubConstants
	{
		public static readonly string NotEnoughRatingType = "not_enough";
		public static readonly string FreeDeliveryLabel = "delivery_free";

		internal const string UserAuthorizationCookieName = "x_user_authorization";
		internal const string UserAuthorizationHeaderName = "x-user-authorization";
		internal const string CookiePackageHeaderName = "cookie";
		internal const string SessionIdCookieName = "PHPSESSID";
		internal const string CookieDomain = "https://www.delivery-club.ru/";
	}
}
