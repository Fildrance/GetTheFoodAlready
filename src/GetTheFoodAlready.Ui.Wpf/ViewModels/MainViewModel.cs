using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using GetTheFoodAlready.Ui.Wpf.Localization;
using GetTheFoodAlready.Ui.Wpf.Support;
using ReactiveUI;

namespace GetTheFoodAlready.Ui.Wpf.ViewModels
{
	public class MainViewModel : ReactiveObject
	{
		private static readonly string AssemblyName = typeof(MainViewModel).Assembly.FullName;
		private const string ImagePathPattern = "/{0};component/Resources/Flags/{1}.png";

		private static SelectBoxItem<CultureInfo> CreateFlagItem(CultureInfo cultureInfo)
		{
			return new SelectBoxItem<CultureInfo>(cultureInfo, string.Format(ImagePathPattern, AssemblyName, cultureInfo.Name));
		}

		private readonly TranslationSource _translationSource;
		private ReactiveObject _currentViewModel;
		private IReadOnlyCollection<SelectBoxItem<CultureInfo>> _availableCultures;

		#region [c-tor]
		public MainViewModel(
			PreparationWizardViewModel preparationWizardViewModel,
			FoodsListViewModel foodsListViewModel,
			TranslationSource translationSource
		)
		{
			_translationSource = translationSource ?? throw new ArgumentNullException(nameof(translationSource));

			CurrentViewModel = preparationWizardViewModel;

			preparationWizardViewModel.FinishObservable.Subscribe(x => {
				CurrentViewModel = foodsListViewModel;
				foodsListViewModel.FilterInfo = x;
			});
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public ReactiveObject CurrentViewModel
		{
			get => _currentViewModel;
			set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
		}

		public IReadOnlyCollection<SelectBoxItem<CultureInfo>> AvailableCultures
		{
			get
			{
				if(_availableCultures == null)
				{
					_availableCultures = _translationSource.AllCultures.Select(CreateFlagItem)
						.ToArray();
				}
				return _availableCultures;
			}
		}

		public SelectBoxItem<CultureInfo> SelectedCulture
		{
			get 
			{
				return AvailableCultures.First(x => x.Value == _translationSource.CurrentCulture);
			}
			set 
			{ 
				if(value.Value != _translationSource.CurrentCulture)
				{
					_translationSource.CurrentCulture = value.Value;
				}
			}
		}
		#endregion
		#endregion
	}
}