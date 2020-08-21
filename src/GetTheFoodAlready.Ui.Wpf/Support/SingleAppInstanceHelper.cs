using System;
using System.Threading;

namespace GetTheFoodAlready.Ui.Wpf.Support
{
	/// <summary>
	/// Keeps wathcing for other instances of same application. Can prevent having multiple instances.
	/// </summary>
	public sealed class SingleAppInstanceHelper : IDisposable
	{
		#region  [Constants]
		private const string MutexForSingletoneAppName = "GetTheFoodAlreadyInitialiOwner";
		#endregion

		#region [Fields]
		private readonly string _evenWaitHandleName = new Guid(51, 3, 45, 143, 6, 13, 4, 86, 7, 40, 61).ToString();
		private Thread _otherInstanceLaunchWatcherThread;
		private EventWaitHandle _sameAppInitializationAttempted;
		private Mutex _singleApplicationMutex;
		#endregion

		#region [Public]
		#region [Public methods]
		/// <summary>
		/// Hooks passed action on event of other instance of same application initialized.
		/// </summary>
		public void HookupActivateWindowOnOtherInstanceInitialization(Action onSameApplicationInitialized)
		{
			_otherInstanceLaunchWatcherThread = new Thread(() =>
			{
				while (_sameAppInitializationAttempted.WaitOne())
				{
					onSameApplicationInitialized();
				}
			});
			_otherInstanceLaunchWatcherThread.Start();
		}

		/// <summary>
		/// Checks if this instance of application is unique.
		/// </summary>
		/// <returns>False if there is already instance of this application, true otherwise.</returns>
		public bool IsOriginalInstance()
		{
			_singleApplicationMutex = new Mutex(true, MutexForSingletoneAppName, out var created);
			_sameAppInitializationAttempted = new EventWaitHandle(false, EventResetMode.AutoReset, _evenWaitHandleName);

			if (created)
			{
				return true;
			}
			// notifies other instance of same application that there was concurrent instance initialization attempt.
			_sameAppInitializationAttempted.Set();
			return false;
		}
		public void Dispose()
		{
			_otherInstanceLaunchWatcherThread?.Abort();
			_sameAppInitializationAttempted?.Dispose();
			_singleApplicationMutex?.Dispose();
		}
		#endregion
		#endregion
	}
}