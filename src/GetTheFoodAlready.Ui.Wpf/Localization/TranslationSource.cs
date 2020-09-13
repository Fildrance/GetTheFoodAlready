using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Resources;

namespace GetTheFoodAlready.Ui.Wpf.Localization
{
	public class TranslationSource : INotifyPropertyChanged, IObservable<CultureInfo>
    {
		
		private const string ApplicationResourcesNamespace = "GetTheFoodAlready.Ui.Wpf.Resources.";

		private readonly Dictionary<string, ResourceManager> resManagers = new Dictionary<string, ResourceManager>();
        private CultureInfo currentCulture = null;
        private ReplaySubject<CultureInfo> _cultureSubject = new ReplaySubject<CultureInfo>(1);
        private IObservable<CultureInfo> _cultureObservable;

		public TranslationSource()
		{
            _cultureObservable = _cultureSubject.Where(x => x != null);
		}
        public static TranslationSource Instance { get; } = new TranslationSource();

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
                var managerName = ApplicationResourcesNamespace + value[0];
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
                    CultureInfo.CurrentCulture = value;
                    CultureInfo.CurrentUICulture = value;
                    var @event = PropertyChanged;
                    if (@event != null)
                    {
                        @event.Invoke(this, new PropertyChangedEventArgs(string.Empty));
                    }
                    _cultureSubject.OnNext(value);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

		public IDisposable Subscribe(IObserver<CultureInfo> observer)
		{
            return _cultureObservable.Subscribe(observer);
		}
	}
}
