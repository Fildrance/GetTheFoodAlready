using GetTheFoodAlready.Ui.Wpf.Registration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Resources;

namespace GetTheFoodAlready.Ui.Wpf.Localization
{
	public class TranslationSource : INotifyPropertyChanged, IObservable<CultureInfo>
	{
		// todo:replace with alias collections and reflection.
		private const string ApplicationResourcesNamespace = "GetTheFoodAlready.Ui.Wpf.Resources.";

		private readonly Dictionary<string, ResourceManager> resManagers = new Dictionary<string, ResourceManager>();
		private readonly IReadOnlyCollection<CultureInfo> _allCultures;
		private CultureInfo currentCulture = null;
		private ReplaySubject<CultureInfo> _cultureSubject = new ReplaySubject<CultureInfo>(1);
		private IObservable<CultureInfo> _cultureObservable;

		public TranslationSource(IReadOnlyCollection<CultureInfo> allCultures, CultureInfo defaultCulture)
		{
			_cultureObservable = _cultureSubject.Where(x => x != null);

			_allCultures = allCultures ?? throw new ArgumentNullException(nameof(allCultures));
			CurrentCulture = defaultCulture ?? throw new ArgumentNullException(nameof(defaultCulture));
		}
		private static TranslationSource _instance;

		public static TranslationSource Instance 
		{ 
			get 
			{
				if (_instance == null) 
				{
					throw new InvalidOperationException();
				}
				return _instance;
			} 
			set 
			{
				if(_instance != null)
				{
					throw new InvalidOperationException();
				}
				_instance = value;
			}
		}

		public string this[string key]
		{
			get
			{
				var value = key.Split('/');
				if (value.Length < 2)
				{
					var message = $"Passed localization argument ({key}) doesn't contain separator ('/') or have empty first/second parts of localization reference. " +
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

				return resManager.GetString(resourceName, currentCulture);
			}
		}

		public CultureInfo CurrentCulture
		{
			get { return currentCulture; }
			set
			{
				if (currentCulture != value)
				{
					Validate(value);
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

		public IEnumerable<CultureInfo> AllCultures => _allCultures;

		public event PropertyChangedEventHandler PropertyChanged;

		public IDisposable Subscribe(IObserver<CultureInfo> observer)
		{
			return _cultureObservable.Subscribe(observer);
		}

		private void Validate(CultureInfo value)
		{
			if (!_allCultures.Contains(value))
			{
				var message = $"Cannot use culture '{value}', culture is not supported. Applicaiton currently supports only following cultures: '{string.Join(",", _allCultures)}'. " +
					$"To refine list of spported cultures add more in {nameof(AppInstaller)}.{nameof(AppInstaller.SupportedCultures)}.";
				throw new ArgumentException(message);
			}
		}
	}
}
