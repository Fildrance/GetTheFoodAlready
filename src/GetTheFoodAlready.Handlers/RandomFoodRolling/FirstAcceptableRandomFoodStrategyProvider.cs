using System;
using System.Collections.Generic;
using System.Linq;

using GetTheFoodAlready.Api.Orchestration.Requests;

namespace GetTheFoodAlready.Handlers.RandomFoodRolling
{
	/// <summary> Random food items strategy that returns first strategy that can accept provided request. </summary>
	public class FirstAcceptableRandomFoodStrategyProvider : IRandomFoodStrategyProvider
	{
		#region [Fields]
		private readonly IReadOnlyCollection<IRandomFoodStrategy> _strategies;
		#endregion

		#region [c-tor]
		public FirstAcceptableRandomFoodStrategyProvider(IReadOnlyCollection<IRandomFoodStrategy> strategies)
		{
			if (null == strategies
			    || !strategies.Any())
			{
				throw new ArgumentNullException(nameof(strategies));
			}
			_strategies = strategies;
		}
		#endregion

		#region [Public]
		#region [Public methods]
		public IRandomFoodStrategy GetStrategy(RandomFoodPropositionsRequest request)
		{
			foreach (var strategy in _strategies)
			{
				if (strategy.CanAccept(request))
				{
					return strategy;
				}
			}

			throw new InvalidOperationException("No acceptable random food rolling strategy found!");
		}
		#endregion
		#endregion
	}
}
