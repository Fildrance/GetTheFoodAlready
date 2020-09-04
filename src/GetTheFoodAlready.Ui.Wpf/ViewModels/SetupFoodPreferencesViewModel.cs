using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using GetTheFoodAlready.Ui.Wpf.Support;

using ReactiveUI;

namespace GetTheFoodAlready.Ui.Wpf.ViewModels
{
	public class SetupFoodPreferencesViewModel : ReactiveObject
	{
		#region [Fields]
		private readonly IDefaultManager<FoodPreference> _defaultManager;
		private readonly IMapper _mapper;
		#endregion

		#region [c-tor]
		public SetupFoodPreferencesViewModel(IDefaultManager<FoodPreference> defaultManager, IMapper mapper)
		{
			_defaultManager = defaultManager ?? throw new ArgumentNullException(nameof(defaultManager));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public IList<SelectBoxItem<string>> AvailableToExcludeMenuParts { get; set; } = new List<SelectBoxItem<string>>
		{
			new SelectBoxItem<string>("Напитки", "Напитки"),
			new SelectBoxItem<string>("Дессерты", "Дессерты"),
			new SelectBoxItem<string>("Закуски", "Закуски")
		};
		#endregion

		#region [Public methods]
		public IReadOnlyCollection<string> GetExcludedMenuParts()
		{
			var items = AvailableToExcludeMenuParts.Where(x => x.IsSelected)
				.ToArray();
			return items.Any()
				? items.Select(x => x.Value)
					.ToArray()
				: Array.Empty<string>();
		}

		public void SaveDefault()
		{
			var mapped = _mapper.Map<FoodPreference>(this);
			_defaultManager.SaveDefault(mapped);
		}

		public async Task<SetupFoodPreferencesViewModel> SetupDefaults()
		{
			var defaults = await _defaultManager.GetDefault();
			_mapper.Map(defaults, this);
			return this;
		}
		#endregion
		#endregion
	}
}