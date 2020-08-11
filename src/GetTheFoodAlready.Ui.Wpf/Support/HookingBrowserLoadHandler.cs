using System;
using System.Windows;

using CefSharp;
using CefSharp.Handler;

namespace GetTheFoodAlready.Ui.Wpf.Support
{
	public class HookingRequestHandler : DefaultRequestHandler
	{
		public override bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
		{
			// todo try catch redirect on browser to get coordinates of pinpointed google map location.
			return base.OnBeforeBrowse(chromiumWebBrowser, browser, frame, request, userGesture, isRedirect);
		}
	}
}