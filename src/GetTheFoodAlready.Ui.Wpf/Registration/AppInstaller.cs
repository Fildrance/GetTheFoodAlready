using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using CrashReporterDotNET;

using GetTheFoodAlready.Api.Registration;
using GetTheFoodAlready.Handlers.Registration;
using GetTheFoodAlready.Ui.Wpf.ViewModels;

using MediatR;

namespace GetTheFoodAlready.Ui.Wpf.Registration
{
	public class AppInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			var reportCrashInstance = CreateReportCrashInstance();

			container.Register(
				Component.For<ReportCrash>().Instance(reportCrashInstance).LifestyleSingleton(),
				Component.For<MainWindow>().ImplementedBy<MainWindow>(),
				Component.For<MainViewModel>().ImplementedBy< MainViewModel>(),
				Component.For<SetupLocationViewModel>().ImplementedBy<SetupLocationViewModel>(),
				Component.For<PreparationWizardViewModel>().ImplementedBy<PreparationWizardViewModel>(),
				Component.For<IMediator>().ImplementedBy<Mediator>(),

				//todo prettify (looks awful!)
				Component.For<ServiceFactory>().UsingFactoryMethod<ServiceFactory>(k => (type =>
				{
					var enumerableType = type
						.GetInterfaces()
						.Concat(new[] { type })
						.FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));

					return enumerableType != null ? k.ResolveAll(enumerableType.GetGenericArguments()[0]) : k.Resolve(type);
				}))
			);
			container.Install(
				new GetTheFoodAlreadyApiInstaller(), 
				new GetTheFoodAlreadyHandlersInstaller()
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
