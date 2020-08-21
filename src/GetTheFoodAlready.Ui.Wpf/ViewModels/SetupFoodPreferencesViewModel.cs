using System;
using System.Collections.Generic;
using System.Linq;

using GetTheFoodAlready.Ui.Wpf.Support;

using ReactiveUI;

namespace GetTheFoodAlready.Ui.Wpf.ViewModels
{
	public class SetupFoodPreferencesViewModel : ReactiveObject
	{
		#region [Fields]
		private IList<SelectBoxItem<string>> _selectedMenuPartsToExclude;
		#endregion

		#region [Public]
		#region [Public properties]
		public IList<SelectBoxItem<string>> AvailableToExcludeMenuParts => new List<SelectBoxItem<string>>
		{
			new SelectBoxItem<string>("Напитки", "Напитки"),
			new SelectBoxItem<string>("Дессерты", "Дессерты"),
			new SelectBoxItem<string>("Закуски", "Закуски")
		};
		public IList<SelectBoxItem<string>> SelectedMenuPartsToExclude
		{
			get => _selectedMenuPartsToExclude;
			set => this.RaiseAndSetIfChanged(ref _selectedMenuPartsToExclude, value);
		}
		#endregion
		#endregion

		public IReadOnlyCollection<string> GetExcludedMenuParts()
		{
			return SelectedMenuPartsToExclude != null
				? SelectedMenuPartsToExclude.Select(x => x.Value)
					.ToArray()
				: Array.Empty<string>();
		}
	}
}
