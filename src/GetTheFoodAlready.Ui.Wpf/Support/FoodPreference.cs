using System.Collections.Generic;

namespace GetTheFoodAlready.Ui.Wpf.Support
{
	/// <summary> Settings for select food preferences wizard page. </summary>
	public class FoodPreference
	{
		#region [Public]
		#region [Public methods]
		public IReadOnlyCollection<string> SelectedMenuPartsToExclude { get; set; }
		#endregion
		#endregion
	}
}