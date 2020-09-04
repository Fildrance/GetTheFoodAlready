using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using CrashReporterDotNET;

using GetTheFoodAlready.Api.Registration;
using GetTheFoodAlready.Api.Support;
using GetTheFoodAlready.Handlers;
using GetTheFoodAlready.Handlers.Registration;
using GetTheFoodAlready.Ui.Wpf.MappingProfiles;
using GetTheFoodAlready.Ui.Wpf.Support;
using GetTheFoodAlready.Ui.Wpf.ViewModels;

using MediatR;

using NLog;

namespace GetTheFoodAlready.Ui.Wpf.Registration
{
	public class AppInstaller : IWindsorInstaller
	{
		private static readonly ILogger Logger = LogManager.GetLogger(typeof(App).FullName);

		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			var reportCrashInstance = CreateReportCrashInstance();

			var googleApiKey = ConfigurationManager.AppSettings["googleApiKey"];

			container.Register(
				Component.For<MainWindow>().ImplementedBy<MainWindow>().LifestyleSingleton(),

				Component.For<MainViewModel>().ImplementedBy<MainViewModel>().LifestyleSingleton(),

				Component.For<Profile>().ImplementedBy<ViewModelToSafeDefaultsMappingProfile>(),
				Component.For<ReportCrash>().Instance(reportCrashInstance).LifestyleSingleton(),
				Component.For<SetupLocationViewModel>().ImplementedBy<SetupLocationViewModel>().LifestyleSingleton()
					.DependsOn(Dependency.OnValue("googleApiKey", googleApiKey))
					.OnCreate(async x => await x.SetupObservables().SetupDefaultLocation()),
				Component.For<PreparationWizardViewModel>().ImplementedBy<PreparationWizardViewModel>().LifestyleSingleton(),
				Component.For<FoodsListViewModel>().ImplementedBy<FoodsListViewModel>().LifestyleSingleton(),
				Component.For<SetupVendorPointPreferencesViewModel> ().ImplementedBy<SetupVendorPointPreferencesViewModel>().LifestyleSingleton()
					.OnCreate(async x => await x.SetupDefault()),
				Component.For<SetupFoodPreferencesViewModel> ().ImplementedBy<SetupFoodPreferencesViewModel>().LifestyleSingleton()
					.OnCreate(async x => await x.SetupDefaults()),

				Component.For<IMediator>().ImplementedBy<Mediator>(),
				Component.For<IDefaultManager<AddressInfo>>().ImplementedBy<DefaultLocationManager>().LifestyleSingleton(),
				Component.For(typeof(IDefaultManager<>)).ImplementedBy(typeof(DefaultManager<>)).LifestyleSingleton(),

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
			//todo: possibly add some class that will wrap reportCrash calls.
			AppDomain.CurrentDomain.UnhandledException += (sender, unhandledExceptionEventArgs) => { HandleException(unhandledExceptionEventArgs.ExceptionObject as Exception, reportCrashInstance); };
			TaskScheduler.UnobservedTaskException += (sender, unobservedTaskExceptionEventArgs) => { HandleException(unobservedTaskExceptionEventArgs.Exception, reportCrashInstance); };

			return reportCrashInstance;
		}
		private static void HandleException(Exception exception, ReportCrash reportCrashInstance)
		{
			var loggerName = exception?.Data[Constants.LoggerToBeUsed] as string;
			if (!string.IsNullOrEmpty(loggerName))
			{
				var logger = LogManager.GetLogger(loggerName);
				logger.Error(exception);
			}
			else
			{
				Logger.Error(exception);
			}

			reportCrashInstance.Send(exception);
		}
	}
}
