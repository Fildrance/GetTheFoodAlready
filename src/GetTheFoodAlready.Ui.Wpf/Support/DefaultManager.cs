using System.Threading.Tasks;

using GetTheFoodAlready.Ui.Wpf.Properties;

using Newtonsoft.Json;

namespace GetTheFoodAlready.Ui.Wpf.Support
{
	/// <summary> Default implementation of default setting manager. Uses application <see cref="Settings"/> to store and load data.</summary>
	/// <typeparam name="T"></typeparam>
	public class DefaultManager<T> : IDefaultManager<T>
	{
		#region [Static fields]
		private static readonly string SettingName = typeof(T).Name;
		#endregion

		#region IDefaultManager<T> implementation
		public virtual Task<T> GetDefault()
		{
			var value = (string) Settings.Default[SettingName];
			if (null == value)
			{
				return Task.FromResult(default(T));
			}
			var result = JsonConvert.DeserializeObject<T>(value);
			return Task.FromResult(result);
		}

		public virtual void SaveDefault(T setting)
		{
			var converted = JsonConvert.SerializeObject(setting);
			var settings = Settings.Default;
			settings[SettingName] = converted;
			settings.Save();
		}
		#endregion
	}
}
