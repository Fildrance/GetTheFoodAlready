using System.Threading.Tasks;

using GetTheFoodAlready.Api.Support;

namespace GetTheFoodAlready.Ui.Wpf.Support
{
	/// <summary> Manager for default user location. </summary>
	public interface IDefaultLocationManager
	{
		/// <summary> Gets default location for user (from settings, by user ip or in other way. </summary>
		Task<AddressInfo> GetDefaultLocation();
	}
}