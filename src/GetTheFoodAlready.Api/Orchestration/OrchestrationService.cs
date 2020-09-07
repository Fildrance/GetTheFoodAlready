using System;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.Orchestration.Requests;
using GetTheFoodAlready.Api.Orchestration.Responses;

using MediatR;

namespace GetTheFoodAlready.Api.Orchestration
{
	public class OrchestrationService : IOrchestrationService
	{
		#region [Fields]
		private readonly IMediator _mediator;
		#endregion

		#region [c-tor]
		public OrchestrationService(IMediator mediator)
		{
			_mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
		}
		#endregion

		#region IOrchestrationService implementation
		public Task<RandomFoodPropositionsResponse> GetRandomFoodPropositions(RandomFoodPropositionsRequest request)
		{
			return _mediator.Send(request);
		}
		#endregion
	}
}