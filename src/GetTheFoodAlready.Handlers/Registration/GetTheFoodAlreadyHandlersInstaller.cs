using AutoMapper;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using GetTheFoodAlready.DeliveryClubBridge;
using GetTheFoodAlready.Handlers.MappingProfiles;

using MediatR;

using Newtonsoft.Json;

namespace GetTheFoodAlready.Handlers.Registration
{
	public class GetTheFoodAlreadyHandlersInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(
				Component.For<Profile>().ImplementedBy<MappingProfile>().LifestyleSingleton(),
				Component.For<IMapper>().ImplementedBy<Mapper>().LifestyleSingleton(),
				Component.For<IConfigurationProvider>().UsingFactoryMethod(x =>
				{
					var profiles = x.ResolveAll<Profile>();
					return new MapperConfiguration(c => c.AddProfiles(profiles));
				}),
				Component.For<IDeliveryClubClient>().ImplementedBy<DeliveryClubClient>(),
				Component.For<JsonSerializer>().Instance(JsonSerializer.CreateDefault()),
				Classes.FromThisAssembly().BasedOn(typeof(IRequestHandler<,>)).WithServiceAllInterfaces(),
				Classes.FromThisAssembly().BasedOn(typeof(INotificationHandler<>)).WithServiceAllInterfaces()
			);
		}
	}
}
