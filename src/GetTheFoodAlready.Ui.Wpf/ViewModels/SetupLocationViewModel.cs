using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using CefSharp;

using GetTheFoodAlready.Api.Maps;
using GetTheFoodAlready.Api.Maps.Requests;
using GetTheFoodAlready.Api.Support;
using GetTheFoodAlready.Ui.Wpf.Resources;
using GetTheFoodAlready.Ui.Wpf.Support;

using NLog;

using ReactiveUI;

namespace GetTheFoodAlready.Ui.Wpf.ViewModels
{
	/// <summary> View-model for setting up user location. </summary>
	public class SetupLocationViewModel : ReactiveObject
	{
		#region [Static fields]
		private static readonly ILogger BrowserConsoleLogger = LogManager.GetLogger("SetupLocation.BrowserEvents");
		#endregion

		#region [Fields]
		private readonly Subject<AddressInfo> _defaultLocation = new Subject<AddressInfo>();
		private readonly Subject<IFrame> _mainFrameLoadEnd = new Subject<IFrame>();

		private IWebBrowser _browser;
		private string _location;
		private IList<AddressInfo> _locationPropositions = new List<AddressInfo>();
		private AddressInfo _selectedLocation;
		private bool _isPropositionsListOpen;
		private bool _locationSetCalledFromMapClick;

		private readonly IDefaultLocationManager _defaultLocationManager;
		private readonly IMapService _mapService;
		#endregion

		#region [c-tor]
		public SetupLocationViewModel(IMapService mapService, IDefaultLocationManager defaultLocationManager)
		{
			_mapService = mapService ?? throw new ArgumentNullException(nameof(mapService));
			_defaultLocationManager = defaultLocationManager ?? throw new ArgumentNullException(nameof(defaultLocationManager));
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public IWebBrowser Browser
		{
			get => _browser;
			set
			{
				if (value != null)
				{
					PrepareBrowser(value);
				}

				this.RaiseAndSetIfChanged(ref _browser, value);
			}
		}
		public bool IsPropositionsListOpen
		{
			get => _isPropositionsListOpen;
			set => this.RaiseAndSetIfChanged(ref _isPropositionsListOpen, value);
		}
		public string Location
		{
			get => _location;
			set => this.RaiseAndSetIfChanged(ref _location, value);
		}
		public IList<AddressInfo> LocationPropositions
		{
			get => _locationPropositions;
			set => this.RaiseAndSetIfChanged(ref _locationPropositions, value);
		}
		public AddressInfo SelectedLocation
		{
			get => _selectedLocation;
			set => this.RaiseAndSetIfChanged(ref _selectedLocation, value);
		}
		#endregion

		#region [Public methods]
		/// <summary> Prepares default location of user. </summary>
		public async Task SetupDefaultLocation()
		{
			var defaultLocation = await _defaultLocationManager.GetDefaultLocation();
			_defaultLocation.OnNext(defaultLocation);
		}

		/// <summary> Prepares observables of view-model. </summary>
		public SetupLocationViewModel SetupObservables()
		{
			this.ObservableForProperty(x => x.Location)
				.Where(x => x.Value.Length >= 3 && LocationPropositions?.Any(a => a.AddressName == x.Value) != true)
				.DistinctUntilChanged()
				.Throttle(TimeSpan.FromMilliseconds(1500))
				.Subscribe(async x => {
					var suggestAddressRequest = new SuggestAddressRequest
					{
						Substring = x.Value
					};
					var found = await _mapService.SuggestAddress(suggestAddressRequest, CancellationToken.None);
					LocationPropositions = found.ResolutionCandidates.ToList();
					IsPropositionsListOpen = true;
				});

			this.ObservableForProperty(x => x.SelectedLocation)
				.Where(x => x.Value != null)
				.Distinct()
				.Subscribe(x => {
					if (_locationSetCalledFromMapClick)
					{
						_locationSetCalledFromMapClick = false;
						return;
					}

					var command = CreateGoToMarkCommand(x.Value.Latitude, x.Value.Longitude);
					_browser.GetMainFrame()
						.ExecuteJavaScriptAsync(command);
				});

			_defaultLocation.Zip(
					_mainFrameLoadEnd.Where(x => x.IsMain), 
					(a, f) => (Address: a, Frame: f)
				).Subscribe(x => {
					var command = CreateGoToMarkCommand(x.Address.Latitude, x.Address.Longitude);
					x.Frame.ExecuteJavaScriptAsync(command);
				});

			return this;
		}
		#endregion
		#endregion

		#region [Private]
		#region [Private methods]
		private string CreateGoToMarkCommand(string lat, string lng)
		{
			return $"if(window.goToMark) {{ window.goToMark({lat},{lng}); }} else {{ alert('{MessageResources.FailedToFindGoToMarkMethod}');}}";
		}

		private void HandleOnMapClick(string lat, string lng, string name)
		{
			var found = LocationPropositions.FirstOrDefault(x => x.AddressName != name);
			if (found != null)
			{
				LocationPropositions.Remove(found);
			}

			// Add new point to list of options.
			var addressInfo = new AddressInfo(name, lat, lng);
			LocationPropositions.Add(addressInfo);

			// set new point to selected address.
			// _locationSetCalledFromMapClick flag is true to not force map call with new point (because it was provided by map, mark is already set correctly)
			_locationSetCalledFromMapClick = true;
			SelectedLocation = addressInfo;
			Location = addressInfo.AddressName;
		}

		private void PrepareBrowser(IWebBrowser browser)
		{
			browser.ConsoleMessage += (sender, args) =>
			{
				// BrowserConsoleLogger.Warn(args.Message);
				// todo remove message boxes, replace with status bar info.
				MessageBox.Show(args.Message);
			};
			// render dummy page with required js code for google maps & google maps javascript api.
			browser.LoadHtml(BrowserHelper.SetupLocationGoogleMapsDummy, "https://localhost/", Encoding.UTF8);
			browser.LoadHandler = new FrameLoadEndWatchingHandler(_mainFrameLoadEnd);

			// add hook in js to call HandleOnMapClick.
			CefSharpSettings.LegacyJavascriptBindingEnabled = true;
			CefSharpSettings.WcfEnabled = true;

			browser.JavascriptObjectRepository.ResolveObject += (sender, e) =>
			{
				var repo = e.ObjectRepository;
				if (e.ObjectName == "cefWatecher")
				{
					repo.Register("cefWatecher", new CefMapWatcher(HandleOnMapClick), true);
				}
			};
		}
		#endregion
		#endregion

		private class CefMapWatcher
		{
			private readonly Action<string, string, string> _onMapClickAction;

			public CefMapWatcher(Action<string, string, string> onMapClickAction)
			{
				_onMapClickAction = onMapClickAction;
			}

			public void OnMapClick(string lat, string lng, string name)
			{
				_onMapClickAction(lat, lng, name);
			}
		}
	}
}