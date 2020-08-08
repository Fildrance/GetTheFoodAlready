using System;
using System.Threading.Tasks;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using CrashReporterDotNET;

namespace GetTheFoodAlready.Registration
{
	public class AppInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			var reportCrashInstance = CreateReportCrashInstance();

			container.Register(
				Component.For<ReportCrash>().Instance(reportCrashInstance).LifestyleSingleton(),
				Component.For<MainWindow>().ImplementedBy<MainWindow>()
			);
		}
		private static ReportCrash CreateReportCrashInstance()
		{
			var reportCrashInstance = new ReportCrash("fildrance@gmail.com")
			{
				Silent = false,
				CaptureScreen = false
			};

			AppDomain.CurrentDomain.UnhandledException += (sender, unhandledExceptionEventArgs) => { reportCrashInstance.Send((Exception) unhandledExceptionEventArgs.ExceptionObject); };
			TaskScheduler.UnobservedTaskException += (sender, unobservedTaskExceptionEventArgs) => { reportCrashInstance.Send(unobservedTaskExceptionEventArgs.Exception); };

			return reportCrashInstance;
		}
	}
}
