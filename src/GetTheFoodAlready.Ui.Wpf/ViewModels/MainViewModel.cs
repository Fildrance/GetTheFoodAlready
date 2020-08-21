using System;
using System.Windows;

using ReactiveUI;

namespace GetTheFoodAlready.Ui.Wpf.ViewModels
{
	public class MainViewModel : ReactiveObject
	{
		private PreparationWizardViewModel _wizardViewModel;
		private FoodsListViewModel _foodsListViewModel;
		private ReactiveObject _currentViewModel;

		#region [c-tor]
		public MainViewModel(
			PreparationWizardViewModel preparationWizardViewModel,
			FoodsListViewModel foodsListViewModel
		)
		{
			_wizardViewModel = preparationWizardViewModel;
			_foodsListViewModel = foodsListViewModel;

			CurrentViewModel = preparationWizardViewModel;

			_wizardViewModel.FinishObservable.Subscribe(x => CurrentViewModel = _foodsListViewModel);
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public ReactiveObject CurrentViewModel
		{
			get => _currentViewModel;
			set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
		}
		#endregion
		#endregion
	}
}