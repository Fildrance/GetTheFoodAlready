using System;
using System.Collections.Generic;
using System.Linq;

namespace GetTheFoodAlready.Ui.Wpf.Support
{
	public static class Utilities
	{
		public static IReadOnlyCollection<T> GetSelectedValues<T>(IEnumerable<SelectBoxItem<T>> collection)
		{
			var items = collection.Where(x => x.IsSelected)
				.ToArray();
			return items.Any()
				? items.Select(x => x.Value)
					.ToArray()
				: Array.Empty<T>();
		}
	}
}