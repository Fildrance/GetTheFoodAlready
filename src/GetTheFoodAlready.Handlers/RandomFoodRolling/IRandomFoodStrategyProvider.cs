using System;

using GetTheFoodAlready.Api.Orchestration.Requests;

namespace GetTheFoodAlready.Handlers.RandomFoodRolling
{
	/// <summary> Provides startegy for rolling random food. </summary>
	public interface IRandomFoodStrategyProvider
	{
		/// <summary> Gets acceptable strategy for rolling random food. </summary>
		/// <param name="request"> Request, containing filtration and selection data.</param>
		/// <exception cref="InvalidOperationException">Throws if fails to find supported strategy.</exception>
		/// <returns>Strategy for rolling random food.</returns>
		IRandomFoodStrategy GetStrategy(RandomFoodPropositionsRequest request);
	}
}