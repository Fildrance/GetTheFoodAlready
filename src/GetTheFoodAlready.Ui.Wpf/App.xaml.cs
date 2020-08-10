using System;
using System.Windows;

using Castle.Windsor;

using GetTheFoodAlready.Ui.Wpf.Registration;
using GetTheFoodAlready.Ui.Wpf.Resources;
using GetTheFoodAlready.Ui.Wpf.Support;

namespace GetTheFoodAlready.Ui.Wpf
{
	public partial class App : Application
	{
		#region [Fields]
		private SingleAppInstanceHelper _singleAppHelper;
		#endregion

		#region [Application overrides]
		protected override void OnExit(ExitEventArgs e)
		{
			_singleAppHelper?.Dispose();
			base.OnExit(e);
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			_singleAppHelper = new SingleAppInstanceHelper();
			if (!_singleAppHelper.IsOriginalInstance())
			{
				MessageBox.Show(MessageResources.ApplicationAlreadyRunning);
				Shutdown();
				return;
			}

			try
			{
				var container = new WindsorContainer()
					.Install(new AppInstaller());
				
				var mainWindow = container.Resolve<MainWindow>();
				MainWindow = mainWindow;
				mainWindow.Show();

				_singleAppHelper.HookupActivateWindowOnOtherInstanceInitialization(
					() => Dispatcher.BeginInvoke((Action)(() => ((MainWindow)MainWindow)?.Activate()))
				);

				base.OnStartup(e);
			}
			catch
			{
				_singleAppHelper.Dispose();
				throw;
			}
		}
		#endregion
	}
}