﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using GetTheFoodAlready.Api.FoodAgregators;
using GetTheFoodAlready.Api.Maps;
using GetTheFoodAlready.Api.Orchestration;

namespace GetTheFoodAlready.Api.Registration
{
	public class GetTheFoodAlreadyApiInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(
				Component.For<IDeliveryClubService>().ImplementedBy<DeliveryClubService>(),
				Component.For<IMapService>().ImplementedBy<MapService>(),
				Component.For<IOrchestrationService>().ImplementedBy<OrchestrationService>()
			);
		}
	}
}
