using System;
using System.Configuration;

using AutoMapper;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using GetTheFoodAlready.Api.Support;
using GetTheFoodAlready.DaDataBridge;
using GetTheFoodAlready.DeliveryClubBridge;
using GetTheFoodAlready.DeliveryClubBridge.Client;
using GetTheFoodAlready.Handlers.Behaviours;
using GetTheFoodAlready.Handlers.MappingProfiles;
using GetTheFoodAlready.Handlers.RandomFoodRolling;
using GetTheFoodAlready.Handlers.Support;

using MediatR;

using NLog;

namespace GetTheFoodAlready.Handlers.Registration
{
	public class GetTheFoodAlreadyHandlersInstaller : IWindsorInstaller
	{
		#region [Fields]
		private readonly bool _useLogging;
		private readonly bool _useProfiling;
		private readonly bool _useSession;
		#endregion

		#region [c-tor]
		public GetTheFoodAlreadyHandlersInstaller(bool useLogging = true, bool useProfiling = true, bool useSession = true)
		{
			_useLogging = useLogging;
			_useProfiling = useProfiling;
			_useSession = useSession;
		}
		#endregion

		#region IWindsorInstaller implementation
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			var dadataApiKey = ConfigurationManager.AppSettings["dadataApiKey"];
			if (string.IsNullOrEmpty(dadataApiKey))
			{
				throw new InvalidOperationException("Attempting to launch application without property 'dadataApiKey' in appSettings! Cannot proceed withou this api key.");
			}

			var deliveryClubClientSettings = new DeliveryClubClientSettings("https://api.delivery-club.ru", "api1.2");
			var retryCount = Dependency.OnValue("maxRetryCount", 5);
			var dadataApiKeyDependency = Dependency.OnValue("token", dadataApiKey);

			container.Register(
				// auto-mapper
				Component.For<Profile>().ImplementedBy<MappingProfile>().LifestyleSingleton(),
				Component.For<IMapper>().ImplementedBy<Mapper>().LifestyleSingleton(),
				Component.For<IConfigurationProvider>().UsingFactoryMethod(x => {
					var profiles = x.ResolveAll<Profile>();
					return new MapperConfiguration(c => c.AddProfiles(profiles));
				}),
				// mediatr handlers
				Classes.FromThisAssembly().BasedOn(typeof(IRequestHandler<,>)).WithServiceAllInterfaces().LifestyleSingleton(),
				Classes.FromThisAssembly().BasedOn(typeof(INotificationHandler<>)).WithServiceAllInterfaces().LifestyleSingleton(),

				Component.For<HttpClientHandlerProvider>().Instance(nested => new LoggingHttpHandler(nested)),
				
				Component.For<IDeliveryClubClient>().ImplementedBy<AutoRetryingDeliveryClubClientDecorator>().DependsOn(retryCount).LifestyleSingleton(),
				Component.For<IDeliveryClubClient>().ImplementedBy<AutoLoginningDeliveryClubClientDecorator>().LifestyleSingleton(),
				Component.For<IDeliveryClubClient>().ImplementedBy<DeliveryClubClient>().LifestyleSingleton(),

				Component.For<IDaDataClient>().ImplementedBy<DaDataClient>().LifestyleSingleton()
					.DependsOn(dadataApiKeyDependency),
				Component.For<HandlerTypeToImplementationCache>().ImplementedBy<HandlerTypeToImplementationCache>().LifestyleSingleton(),
				Component.For<ITimeSpanParser>().ImplementedBy<DeliveryClubExpectedTimeSpanParser>().LifestyleSingleton(),
				Component.For<DeliveryClubClientSettings>().Instance(deliveryClubClientSettings).LifestyleSingleton(),
				Component.For<IVendorFilter>().ImplementedBy<VendorFilter>(),

				Component.For<IRandomFoodStrategy>().ImplementedBy<RerollIfEmptyFinalFoodResultsRandomFoodStrategy>()
					.DependsOn(Dependency.OnValue("maxFoodItems", 4), Dependency.OnValue("maxRerollAttempts", 5)),
				Component.For<IRandomFoodStrategyProvider>().ImplementedBy<FirstAcceptableRandomFoodStrategyProvider>()
			);																						

			if (_useSession)
			{
				container.Register
				(
					Component.For(typeof(IPipelineBehavior<,>)).ImplementedBy(typeof(SessionBehaviour<,>))
				);
			}
			if (_useLogging)
			{
				container.Register
				(
					Component.For(typeof(IPipelineBehavior<,>)).ImplementedBy(typeof(LoggingBehavior<,>))
				);
			}
			if (_useProfiling)
			{
				container.Register
				(
					Component.For(typeof(IPipelineBehavior<,>)).ImplementedBy(typeof(ProfilingBehaviour<,>))
						.DependsOn(Dependency.OnValue<ILogger>(LogManager.GetLogger("ProfilingBehaviour")))
				);
			}
		}
		#endregion
	}
}
