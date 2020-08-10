using System;
using System.Windows;

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
		}

		private void MainWindow_OnClosed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}