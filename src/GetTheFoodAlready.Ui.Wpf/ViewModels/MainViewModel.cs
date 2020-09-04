using System;

using ReactiveUI;

namespace GetTheFoodAlready.Ui.Wpf.ViewModels
{
	public class MainViewModel : ReactiveObject
	{
		private ReactiveObject _currentViewModel;

		#region [c-tor]
		public MainViewModel(
			PreparationWizardViewModel preparationWizardViewModel,
			FoodsListViewModel foodsListViewModel
		)
		{
			var foodsListViewModel1 = foodsListViewModel;

			CurrentViewModel = preparationWizardViewModel;

			preparationWizardViewModel.FinishObservable.Subscribe(x => {
				CurrentViewModel = foodsListViewModel1;
				foodsListViewModel1.FilterInfo = x;
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
		#endregion
		#endregion
	}
}