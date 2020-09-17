using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using FluentValidation;
using GetTheFoodAlready.Api.Cache;
using GetTheFoodAlready.Api.FoodAgregators;
using GetTheFoodAlready.Api.Maps;
using GetTheFoodAlready.Api.Orchestration;
using System;
using System.Runtime.Caching;

namespace GetTheFoodAlready.Api.Registration
{
	public class GetTheFoodAlreadyApiInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(
				// services
				Component.For<IDeliveryClubService>().ImplementedBy<DeliveryClubService>(),
				Component.For<IMapService>().ImplementedBy<MapService>(),
				Component.For<IOrchestrationService>().ImplementedBy<OrchestrationService>(),

				// validators
				Classes.FromThisAssembly().BasedOn(typeof(IValidator)).WithServiceAllInterfaces().LifestyleSingleton(),

				// cache
				Component.For<ICacheManagerFactory>().ImplementedBy<MemoryCacheManagerFactory>().LifestyleSingleton()
					.DependsOn(Dependency.OnValue<MemoryCache>(MemoryCache.Default))
					.DependsOn(Dependency.OnValue<TimeSpan?>(null))
			);
		}
	}
}
