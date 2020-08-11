using System;
using System.Collections.Generic;

using Castle.MicroKernel;
using Castle.Windsor;

namespace GetTheFoodAlready.Handlers.Support
{
	/// <summary> Handler interface types mapped to registered implementation types. </summary>
	public class HandlerTypeToImplementationCache : Dictionary<Type, Type>
	{
		#region [Fields]
		private readonly IKernel _container;
		#endregion

		#region [c-tor]
		public HandlerTypeToImplementationCache(IKernel container)
		{
			_container = container ?? throw new ArgumentNullException(nameof(container));
		}
		#endregion

		#region [Public]
		#region [Public methods]
		/// <summary>
		/// Gets implementation type from cache or from DI container configuration.
		/// </summary>
		/// <param name="handlerInterfaceType">Handler interface type.</param>
		/// <returns>Type that implements requested handler interface type in app DI configuration.</returns>
		public Type GetCachedOrFromConfiguration(Type handlerInterfaceType)
		{
			if (!TryGetValue(handlerInterfaceType, out var handlerType))
			{
				var handler = _container.GetHandler(handlerInterfaceType);
				if (handler == null)
				{
					this[handlerInterfaceType] = null;
					return null;
				}
				var handlerImplType = handler.ComponentModel.Implementation;

				handlerType = handlerImplType;
				this[handlerImplType] = handlerType;
			}
			return handlerType;
		}
		#endregion
		#endregion
	}
}