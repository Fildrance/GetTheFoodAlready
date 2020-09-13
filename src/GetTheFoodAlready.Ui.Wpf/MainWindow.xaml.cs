using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GetTheFoodAlready.Ui.Wpf.Localization;
using GetTheFoodAlready.Ui.Wpf.ViewModels;

namespace GetTheFoodAlready.Ui.Wpf
{
	public partial class MainWindow : Window
	{
		private readonly MainViewModel _viewModel;

		public MainWindow(MainViewModel viewModel)
		{
			_viewModel = viewModel;

			InitializeComponent();
			DataContext = viewModel;

			//tmp solution until proper custom window implemented.
			LangComboBox.SelectedIndex = 0;
		}

		private void MainWindow_OnClosed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var value = (string)e.AddedItems.OfType<ComboBoxItem>().First().Content;
			TranslationSource.Instance.CurrentCulture = new CultureInfo(value);
		}
	}
}