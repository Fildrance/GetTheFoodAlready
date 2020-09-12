using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace GetTheFoodAlready.Ui.Wpf.Localization
{
	public class TranslationSource : INotifyPropertyChanged
    {
		public static TranslationSource Instance { get; } = new TranslationSource();

        private readonly Dictionary<string, ResourceManager> resManagers = new Dictionary<string, ResourceManager>();
        private CultureInfo currentCulture = null;

        public string this[string key]
        {
            get {
                var value = key.Split('/');
				if (value.Length < 2)
				{
                    var message = $"Passed argument ({key}) had no separator ('/') or had empty first/second parts of localization reference. " +
                        "Reference to localization reference should be of 2 parts - name of resource and name of localization string, separated by '/' character.";
                    throw new ArgumentException(message, nameof(value));
				}
                var managerName = "GetTheFoodAlready.Ui.Wpf.Resources." + value[0];
                var resourceName = value[1];

                ResourceManager resManager;
                if (!resManagers.ContainsKey(managerName))
				{
                    resManagers.Add(managerName, new ResourceManager(managerName, typeof(TranslationSource).Assembly));
				}
                resManager = resManagers[managerName];

                return resManager.GetString(resourceName, currentCulture); }
        }

        public CultureInfo CurrentCulture
        {
            get { return currentCulture; }
            set
            {
                if (currentCulture != value)
                {
                    currentCulture = value;
                    var @event = PropertyChanged;
                    if (@event != null)
                    {
                        @event.Invoke(this, new PropertyChangedEventArgs(string.Empty));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
