using System.Threading.Tasks;

using GetTheFoodAlready.Api.Orchestration.Requests;
using GetTheFoodAlready.Api.Orchestration.Responses;

namespace GetTheFoodAlready.Api.Orchestration
{
	/// <summary> Orchestrating service for major application scenarios. </summary>
	public interface IOrchestrationService
	{
		/// <summary> Get random food propositions for your dinner! </summary>
		/// <param name="request">Request data with filters.</param>
		/// <returns>List of proposed foods and vendor points data.</returns>
		Task<RandomFoodPropositionsResponse> GetRandomFoodPropositions(RandomFoodPropositionsRequest request);
	}
}
