using System.Collections.Generic;
using System.Linq;

namespace GetTheFoodAlready.Ui.Wpf.Support
{
	/// <summary> Support methods for view models.</summary>
	public static class ViewModelExtensions
	{
		/// <summary>
		/// Marks <see cref="original"/> list elements as selected, if <see cref="actual"/> contain it's value.
		/// </summary>
		/// <typeparam name="T">Type of elements for select-box-items.</typeparam>
		/// <param name="original">Original collection, most likely used in combo-box or similar control.</param>
		/// <param name="actual">List of keys to be selected.</param>
		public static void RegenerateListFromOriginalElements<T>(IEnumerable<SelectBoxItem<T>> original, IReadOnlyCollection<T> actual)
		{
			foreach (var selectBoxItem in original)
			{
				if (actual.Contains(selectBoxItem.Value))
				{
					selectBoxItem.IsSelected = true;
				}
			}
		}
	}
}