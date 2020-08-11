using System;
using System.Threading;
using System.Threading.Tasks;

namespace GetTheFoodAlready.Handlers.Behaviours
{
	/// <summary> Mediatr call session. Contains root, parent session and current session (if one was declared). </summary>
	public class Session
	{
		private static AsyncLocal<Session> _instance { get; } = new AsyncLocal<Session>();
		public static Session Instance => _instance.Value;
		private Session(Session parent, Guid? rootId = null)
		{
			ParentSession = parent;
			Id = Guid.NewGuid();
			RootId = rootId ?? Id;
		}
		/// <summary>
		/// Parent session.
		/// </summary>
		public Session ParentSession { get; }
		/// <summary>
		/// Root session Id.
		/// </summary>
		public Guid? RootId { get; }
		/// <summary>
		/// This session Id.
		/// </summary>
		public Guid Id { get; }

		/// <summary>
		/// Executes action within session context.
		/// </summary>
		/// <param name="action">Action to be executed.</param>
		public static void ExecuteInSession(Action action)
		{
			var previous = _instance.Value;
			_instance.Value = new Session(previous, previous?.RootId);
			action();
			_instance.Value = previous;
		}

		/// <summary>
		/// Executes task within session context.
		/// </summary>
		/// <param name="task">Task to be executed.</param>
		/// <returns>Result of requested task.</returns>
		public static async Task<T> ExecuteInSession<T>(Task<T> task)
		{
			var previous = _instance.Value;
			_instance.Value = new Session(previous, previous?.RootId);
			var result = await task;
			_instance.Value = previous;
			return result;
		}
	}
}