using System.Threading.Tasks;

namespace GetTheFoodAlready.Api.Cache
{
	/// <summary> Manages cache objects of certain type.</summary>
	/// <typeparam name="T">Type of objects that should be cached.</typeparam>
	public interface ICacheManager<T> where T : class
	{
		/// <summary>
		/// Gets cached value by string key. If there is no element with such key - returns null.
		/// </summary>
		/// <param name="key">Stringified cache key.</param>
		/// <returns>Found value or null.</returns>
		Task<T> Get(string key);
		/// <summary>
		/// Saves value to cache with passed key.
		/// </summary>
		/// <param name="key">Stringified cache key.</param>
		/// <param name="value">Value to be saved.</param>
		Task Set(string key, T value);
		/// <summary>
		/// Try to get value, if there is such key.
		/// </summary>
		/// <param name="key">Stringified cache key.</param>
		/// <param name="value">Found cached value or null.</param>
		/// <returns>True if found some value, false otherwise.</returns>
		Task<bool> TryGet(string key, out T value);
		/// <summary>
		/// Invalidates cache for passed key.
		/// </summary>
		/// <param name="key">Stringified cache key.</param>
		Task Evict(string key);
	}
}
