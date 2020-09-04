using System.Collections.Generic;

namespace GetTheFoodAlready.Ui.Wpf.Support
{
	/// <summary> Settings for select vendor point pref page of wizard. </summary>
	public class VendorPointPreferences
	{
		#region [Public]
		#region [Public properties]
		public bool IsMinimumOrderAmountUsed { get; set; }
		public bool IsOnlyFreeDelivery { get; set; }
		public bool IsRatingImportant { get; set; }
		public int? MinimumOrderAmount { get; set; }
		public decimal? MinimumRaiting { get; set; }
		public int? MinimumRateVoteCount { get; set; }
		public int? SelectedAcceptableDeliveryTimeTil { get; set; }
		public IReadOnlyCollection<string> SelectedCuisines { get; set; }
		public IReadOnlyCollection<string> SelectedPaymentTypes { get; set; }
		#endregion
		#endregion
	}
}