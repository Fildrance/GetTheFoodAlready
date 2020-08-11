using ReactiveUI;

namespace GetTheFoodAlready.Ui.Wpf.ViewModels
{
	/// <summary> View-model for preparation of app usage. </summary>
	public class PreparationWizardViewModel : ReactiveObject
	{
		#region [Fields]
		#endregion

		#region [c-tor]
		public PreparationWizardViewModel(SetupLocationViewModel setupLocation)
		{
			SetupLocationViewModel = setupLocation;
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public SetupLocationViewModel SetupLocationViewModel { get; }
		#endregion
		#endregion
	}
}