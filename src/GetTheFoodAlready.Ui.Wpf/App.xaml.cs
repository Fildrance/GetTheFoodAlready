using System;
using System.Windows;

using Castle.Windsor;

using GetTheFoodAlready.Ui.Wpf.Registration;
using GetTheFoodAlready.Ui.Wpf.Resources;
using GetTheFoodAlready.Ui.Wpf.Support;

using NLog;

namespace GetTheFoodAlready.Ui.Wpf
{
	public partial class App : Application
	{
		#region [Static fields]
		private static readonly ILogger Logger = LogManager.GetLogger(typeof(App).FullName);
		#endregion

		#region [Fields]
		private SingleAppInstanceHelper _singleAppHelper;
		private IWindsorContainer _container;
		#endregion

		#region [Application overrides]
		protected override void OnExit(ExitEventArgs e)
		{
			Logger.Debug($"Application is shutting down. Exit code is {e.ApplicationExitCode}.");
			_container?.Dispose();
			_singleAppHelper?.Dispose();
			base.OnExit(e);
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			Logger.Info("Starting application.\r\nChecking if there are other instances up.");
			_singleAppHelper = new SingleAppInstanceHelper();
			if (!_singleAppHelper.IsOriginalInstance())
			{
				Logger.Warn(MessageResources.ApplicationAlreadyRunning);
				MessageBox.Show(MessageResources.ApplicationAlreadyRunning);
				Shutdown();
				return;
			}

			try
			{
				Logger.Info("Composing application root - Registration.");
				_container = new WindsorContainer()
					.Install(new AppInstaller());

				Logger.Info("Composing application root - Resolution.");
				var mainWindow = _container.Resolve<MainWindow>();
				MainWindow = mainWindow;
				Logger.Info("Rendering main view.");
				mainWindow.Show();

				_singleAppHelper.HookupActivateWindowOnOtherInstanceInitialization
				(
					() => Dispatcher.BeginInvoke
					(
						(Action) (() =>
						{
							Logger.Debug("Other instance of application asked to activate main window.");
							((MainWindow) MainWindow)?.Activate();
						})
					)
				);

				base.OnStartup(e);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				_singleAppHelper.Dispose();
				throw;
			}
		}
		#endregion
	}
}