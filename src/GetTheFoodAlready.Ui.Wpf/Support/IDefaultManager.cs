using System.Threading.Tasks;

namespace GetTheFoodAlready.Ui.Wpf.Support
{
	/// <summary> Manager for default values of settings. </summary>
	/// <typeparam name="T">Setting type to be managed.</typeparam>
	public interface IDefaultManager<T>
	{
		/// <summary> Gets default value for setting. </summary>
		/// <returns>Task that will get defaul value from some storage or create it by logic.</returns>
		Task<T> GetDefault();
		/// <summary> Saves default value in storage. </summary>
		void SaveDefault(T setting);
	}
}
