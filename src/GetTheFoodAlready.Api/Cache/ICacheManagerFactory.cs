namespace GetTheFoodAlready.Api.Cache
{
	/// <summary> Factory for getting cache managers. </summary>
	public interface ICacheManagerFactory
	{
		/// <summary>
		/// Gets cache manager object.
		/// </summary>
		/// <typeparam name="T">Type of object that manager should cache.</typeparam>
		/// <returns>New or existing cache manager.</returns>
		ICacheManager<T> GetManager<T>() where T : class;
	}
}
