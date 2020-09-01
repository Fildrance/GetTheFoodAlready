using System.Collections.Generic;

using Newtonsoft.Json;

namespace GetTheFoodAlready.DeliveryClubBridge.DataTypes
{
	public class Id
	{
		public string Primary { get; set; }
	}

	public class Chain
	{
		/// <summary>
		/// Name of vendor company.
		/// </summary>
		public string Alias { get; set; }
		public Id Id { get; set; }
	}

	public class MinOrderPrice
	{
		public int Value { get; set; }
	}

	public class Price
	{
		public decimal Value { get; set; }
	}

	public class PriceInterval
	{
		public int? DeliveryPrice { get; set; }
		public int? MinOrderTotal { get; set; }
	}

	public class Delivery
	{
		public bool Available { get; set; }
		public MinOrderPrice MinOrderPrice { get; set; }
		public Price Price { get; set; }
		public List<PriceInterval> PriceIntervals { get; set; }
		public string Provider { get; set; }
		public List<string> Services { get; set; }
		public string Time { get; set; }
	}

	public class Payment
	{
		public string Type { get; set; }
	}

	public class Reviews
	{
		public string RatingScore { get; set; }
		public int? ReviewCount { get; set; }
		public string Score { get; set; }
		public int? ScoreCount { get; set; }
	}

	public class DeliveryClubVendor
	{
		/// <summary>
		/// Vendor point alias.
		/// </summary>
		public string Alias { get; set; }
		public bool AllowPreorder { get; set; }
		public int? CategoryId { get; set; }
		/// <summary>
		/// Vendor company data. 
		/// </summary>
		public Chain Chain { get; set; }
		public List<string> Cuisines { get; set; }
		public Delivery Delivery { get; set; }
		public Id Id { get; set; }
		public List<string> Labels { get; set; }
		public string Logo { get; set; }
		public string Name { get; set; }
		public string OpenDays { get; set; }
		public string OpenFrom { get; set; }
		public string OpenTo { get; set; }
		public List<Payment> Payments { get; set; }
		public Reviews Reviews { get; set; }
	}

	public class DeliveryClubVendorsList
	{
		public int Count { get; set; }
		public bool HasMore { get; set; }
		[JsonProperty("items")]
		public List<DeliveryClubVendor> Vendors { get; set; }

		public override string ToString()
		{
			return $"Vendor list with {Vendors.Count} items, addtional info - Count:{Count}, HasMore:{HasMore}";
		}
	}
	public class RootDeliveryClubVendorsResponse
	{
		[JsonProperty("vendors")]
		public DeliveryClubVendorsList PagedList { get; set; }
	}
}