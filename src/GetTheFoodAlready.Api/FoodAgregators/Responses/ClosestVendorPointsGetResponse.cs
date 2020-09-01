using System.Collections.Generic;

namespace GetTheFoodAlready.Api.FoodAgregators.Responses
{
	public class ClosestVendorPointsGetResponse
	{
		#region [c-tor]
		public ClosestVendorPointsGetResponse(IReadOnlyCollection<VendorInfo> vendors)
		{
			Vendors = vendors;
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public IReadOnlyCollection<VendorInfo> Vendors { get; }
		#endregion
		#endregion
	}
	public class VendorInfo
	{
		public int Id { get; set; }
		public string VendorAlias { get; set; }
		public string VendorPointAlias { get; set; }
		public string DisplayName { get; set; }
		public IReadOnlyCollection<string> Cuisines { get; set; }
		public IReadOnlyCollection<string> AvailablePaymentTypes { get; set; }
		public int? MinOrderTotal { get; set; }
		public int? DeliveryPrice { get; set; }

		public string RatingScore { get; set; }
		public int? ReviewCount { get; set; }
		public decimal? Score { get; set; }
		public int? ScoreCount { get; set; }
		public string DeliveryTime { get; set; }
		public bool IsDeliveringForFree { get; set; }
		/// <summary>Возвращает строку, представляющую текущий объект.</summary>
		/// <returns>Строка, представляющая текущий объект.</returns>
		public override string ToString()
		{
			return $"Name: '{DisplayName}', Id:'{Id}'";
		}
	}
}