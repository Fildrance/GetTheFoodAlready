using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using NLog;

namespace GetTheFoodAlready.Api.Support
{
	/// <summary> HttpClient handler that is wrapping calls and logs info if it is relevant. </summary>
	public class LoggingHttpHandler : DelegatingHandler
	{
		#region [Static fields]
		private static readonly ILogger Logger = LogManager.GetLogger(typeof(HttpClient).FullName);
		#endregion

		#region [c-tor]
		public LoggingHttpHandler(HttpMessageHandler innerHandler): base(innerHandler)
		{
		}
		#endregion

		#region [DelegatingHandler overrides]
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (Logger.IsTraceEnabled)
			{
				var requestMessage = $"Request info: \r\n {request}";
				if (request.Content != null)
				{
					var contentMessage = await request.Content.ReadAsStringAsync();
					requestMessage = requestMessage + "\r\n Request data: \r\n " + contentMessage;
				}
				Logger.Trace(requestMessage);
			}

			var response = await base.SendAsync(request, cancellationToken);

			if (Logger.IsTraceEnabled)
			{
				var responseMessage = $"Response info: \r\n {request}";
				if (response.Content != null)
				{
					var contentMessage = await response.Content.ReadAsStringAsync();
					responseMessage = responseMessage + "\r\n Request data: \r\n " + contentMessage;
				}
				Logger.Trace(responseMessage);
			}
			
			return response;
		}
		#endregion
	}
}