using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Handlers.Support;

using MediatR;

using NLog;

namespace GetTheFoodAlready.Handlers.Behaviours
{
	/// <summary>
	/// Profiles incoming calls, getting execution time.
	/// Behaviours wrap mediatr calls.
	/// </summary>
	public class ProfilingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
	{
		#region [Fields]
		private readonly HandlerTypeToImplementationCache _handlerInterfaceToImplementationTypeCache;
		private readonly ILogger _profilerLogger;
		#endregion

		#region [c-tor]
		public ProfilingBehaviour(HandlerTypeToImplementationCache cache, ILogger logger)
		{
			_handlerInterfaceToImplementationTypeCache = cache ?? throw new ArgumentNullException(nameof(cache));
			_profilerLogger = logger ?? throw new ArgumentNullException(nameof(logger));
		}
		#endregion

		#region IPipelineBehavior<TRequest,TResponse> implementation
		public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			var key = typeof(IRequestHandler<TRequest, TResponse>);
			var found = _handlerInterfaceToImplementationTypeCache.GetCachedOrFromConfiguration(key);

			var stopwatch = Stopwatch.StartNew();
			var response = await next();

			var traceMessage = BuildTraceMessage(stopwatch, found.FullName);
			_profilerLogger.Trace(traceMessage);

			return response;
		}
		#endregion

		#region [Protected]
		#region [Protected methods]
		/// <summary> Builds message to be written for perf analysis</summary>
		/// <param name="stopwatch">Stopwatch with recorded call duration.</param>
		/// <param name="handlerTypeFullName">Name of called handler.</param>
		/// <returns></returns>
		protected virtual string BuildTraceMessage(Stopwatch stopwatch, string handlerTypeFullName)
		{
			return $"{handlerTypeFullName};{stopwatch.ElapsedMilliseconds}";
		}
		#endregion
		#endregion
	}
}