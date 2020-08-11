using System;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Handlers.Support;

using MediatR;

using NLog;

namespace GetTheFoodAlready.Handlers.Behaviours
{
	/// <summary>
	/// Logs all incoming and outgoing calls.
	/// Behaviours wrap mediatr calls.
	/// </summary>
	public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
	{
		#region [Fields]
		private readonly HandlerTypeToImplementationCache _handlerInterfaceToImplementationTypeCache;
		#endregion

		#region [c-tor]
		public LoggingBehavior(HandlerTypeToImplementationCache cache)
		{
			_handlerInterfaceToImplementationTypeCache = cache ?? throw new ArgumentNullException(nameof(cache));
		}
		#endregion

		#region IPipelineBehavior<TRequest,TResponse> implementation
		public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			var key = typeof(IRequestHandler<TRequest, TResponse>);
			var handlerType = _handlerInterfaceToImplementationTypeCache.GetCachedOrFromConfiguration(key);

			if (Session.Instance != null)
			{
				MappedDiagnosticsLogicalContext.Set("RootSessionId", Session.Instance.RootId);
				MappedDiagnosticsLogicalContext.Set("SessionId", Session.Instance.Id);
			}

			var logger = LogManager.GetLogger(handlerType.FullName);

			//todo: add logging of request and response data.
			logger.Debug($"START {typeof(TRequest).Name}");
			var response = await next();
			logger.Debug($"END {typeof(TResponse).Name}");

			return response;
		}
		#endregion
	}
}