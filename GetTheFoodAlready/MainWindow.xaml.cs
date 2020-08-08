using System;
using System.Windows;

namespace GetTheFoodAlready
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void MainWindow_OnClosed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}
