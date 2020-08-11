using ReactiveUI;

namespace GetTheFoodAlready.Ui.Wpf.ViewModels
{
	public class MainViewModel : ReactiveObject
	{
		#region [c-tor]
		public MainViewModel(PreparationWizardViewModel preparationWizardViewModel)
		{
			CurrentViewModel = preparationWizardViewModel;
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public ReactiveObject CurrentViewModel { get; set; }
		#endregion
		#endregion
	}
}