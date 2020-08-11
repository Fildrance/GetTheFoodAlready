using System;
using System.Configuration;

using AutoMapper;

using Castle.Core.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using GetTheFoodAlready.Api.Support;
using GetTheFoodAlready.DaDataBridge;
using GetTheFoodAlready.DeliveryClubBridge;
using GetTheFoodAlready.Handlers.Behaviours;
using GetTheFoodAlready.Handlers.MappingProfiles;
using GetTheFoodAlready.Handlers.Support;

using MediatR;

using Newtonsoft.Json;

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

			container.Register(
				// auto-mapper
				Component.For<Profile>().ImplementedBy<MappingProfile>().LifestyleSingleton(),
				Component.For<IMapper>().ImplementedBy<Mapper>().LifestyleSingleton(),
				Component.For<IConfigurationProvider>().UsingFactoryMethod(x =>
				{
					var profiles = x.ResolveAll<Profile>();
					return new MapperConfiguration(c => c.AddProfiles(profiles));
				}),
				// mediatr handlers
				Classes.FromThisAssembly().BasedOn(typeof(IRequestHandler<,>)).WithServiceAllInterfaces().LifestyleSingleton(),
				Classes.FromThisAssembly().BasedOn(typeof(INotificationHandler<>)).WithServiceAllInterfaces().LifestyleSingleton(),

				Component.For<JsonSerializer>().Instance(JsonSerializer.CreateDefault()),
				Component.For<HttpClientHandlerProvider>().Instance(nested => new LoggingHttpHandler(nested)),
				
				Component.For<IDeliveryClubClient>().ImplementedBy<DeliveryClubClient>().LifestyleSingleton(),
				Component.For<IDaDataClient>().ImplementedBy<DaDataClient>().LifestyleSingleton()
					.DependsOn(Dependency.OnValue("token", dadataApiKey)),
				Component.For<HandlerTypeToImplementationCache>().ImplementedBy<HandlerTypeToImplementationCache>().LifestyleSingleton()
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
