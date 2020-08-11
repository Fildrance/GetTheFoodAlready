using System.Threading;
using System.Threading.Tasks;

using MediatR;

namespace GetTheFoodAlready.Handlers.Behaviours
{
	/// <summary>
	/// Adds nestable uinque session identifier to make reading async and parallel handler calls easier.
	/// Behaviours wrap mediatr calls.
	/// </summary>
	public class SessionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
	{
		public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			return Session.ExecuteInSession(next());
		}
	}
}