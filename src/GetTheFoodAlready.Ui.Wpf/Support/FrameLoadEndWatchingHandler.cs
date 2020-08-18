using System.Reactive.Subjects;

using CefSharp;

namespace GetTheFoodAlready.Ui.Wpf.Support
{
	/// <summary> Browser page load handler, invokes subject when main frame load eneded. </summary>
	public class FrameLoadEndWatchingHandler : ILoadHandler
	{
		#region [Fields]
		private readonly Subject<IFrame> _subj;
		#endregion

		#region [c-tor]
		public FrameLoadEndWatchingHandler(Subject<IFrame> subj)
		{
			_subj = subj;
		}
		#endregion

		#region ILoadHandler implementation
		public void OnLoadingStateChange(IWebBrowser chromiumWebBrowser, LoadingStateChangedEventArgs loadingStateChangedArgs) { }
		public void OnFrameLoadStart(IWebBrowser chromiumWebBrowser, FrameLoadStartEventArgs frameLoadStartArgs) { }
		public void OnFrameLoadEnd(IWebBrowser chromiumWebBrowser, FrameLoadEndEventArgs frameLoadEndArgs)
		{
			var frame = frameLoadEndArgs.Frame;
			_subj.OnNext(frame);
		}
		public void OnLoadError(IWebBrowser chromiumWebBrowser, LoadErrorEventArgs loadErrorArgs) { }
		#endregion
	}
}