using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
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
		#region  [Constants]
		private const string WasInterceptedFlagName = "wasIntercepted";
		#endregion

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

			IDisposable scope = null;
			if (Session.Instance != null)
			{
				var loggingContext = new List<KeyValuePair<string, object>>{
					new KeyValuePair<string, object>("RootSessionId", Session.Instance.RootId),
					new KeyValuePair<string, object>("SessionId", Session.Instance.Id)
				};
				scope = MappedDiagnosticsLogicalContext.SetScoped(loggingContext);
			}

			var logger = LogManager.GetLogger(handlerType.FullName);

			//todo: add logging of request and response data.
			logger.Debug($"START {typeof(TRequest).Name}");
			TResponse response = default(TResponse);
			try
			{
				response = await next();
			}
			catch (Exception ex)
			{
				var o = ex.Data[WasInterceptedFlagName] as bool?;
				if (o != true)
				{
					ex.Data[WasInterceptedFlagName] = true;
					ex.Data[Constants.LoggerToBeUsed] = logger.Name;

					logger.Debug($"END {typeof(TResponse).Name} WITH FAILURE");
					// in async enviroment it is important to get real source of problem - ExceptionDispatchInfo will help getting stack trace with methods called.
					ExceptionDispatchInfo.Capture(ex)
						.Throw();
				}

				throw;
			}

			logger.Debug($"END {typeof(TResponse).Name} WITH SUCCESS");

			scope?.Dispose();
			return response;
		}
		#endregion
	}
}