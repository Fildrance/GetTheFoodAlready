using System.Windows;
using System.Windows.Input;

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

			NextCommand = ReactiveCommand.Create(() =>
			{
				var stuff = SetupLocationViewModel.SelectedLocation;
				return MessageBox.Show("next called " + stuff?.Latitude + stuff?.Longitude);
			});
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public SetupLocationViewModel SetupLocationViewModel { get; }
		public ICommand NextCommand { get; }
		#endregion
		#endregion
	}
}