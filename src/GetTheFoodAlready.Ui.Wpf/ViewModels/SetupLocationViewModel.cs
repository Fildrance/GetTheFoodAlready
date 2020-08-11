using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;

using CefSharp.Wpf;

using GetTheFoodAlready.Api.Maps;
using GetTheFoodAlready.Api.Maps.Requests;
using GetTheFoodAlready.Ui.Wpf.Support;

using ReactiveUI;

namespace GetTheFoodAlready.Ui.Wpf.ViewModels
{
	/// <summary> View-model for setting up user location. </summary>
	public class SetupLocationViewModel : ReactiveObject
	{
		#region [Fields]
		private bool _isPropositionsListOpen;
		private string _location;
		private string _selectedLocation;
		private string _showUrl;
		private IReadOnlyCollection<string> _locationPropositions;
		private IWpfWebBrowser _webBrowser;

		private readonly IMapService _mapService;
		#endregion

		#region [c-tor]
		/// <summary>
		/// Initializes a new instance of the <see cref="T:ReactiveUI.ReactiveObject" /> class.
		/// </summary>
		public SetupLocationViewModel(IMapService mapService)
		{
			_mapService = mapService;

			_mapService = mapService;

			this.ObservableForProperty(x => x.Location)
				.Where(x => x.Value.Length > 3)
				.DistinctUntilChanged()
				.Throttle(TimeSpan.FromMilliseconds(1500))
				.Subscribe
				(async x =>
				{
					var suggestAddressRequest = new SuggestAddressRequest
					{
						Substring = x.Value
					};
					var found = await _mapService.SuggestAddress(suggestAddressRequest, CancellationToken.None);
					LocationPropositions = found.ResolutionCandidates;
					IsPropositionsListOpen = true;
				});

			this.ObservableForProperty(x => x.SelectedLocation)
				.Where(x => x.Value != null)
				.Distinct()
				.Subscribe
				(x =>
				{
					var encoded = Uri.EscapeUriString(x.Value);
					var googleMapUrl = $"https://www.google.ru/maps/place/{encoded}/";
					ShowUrl = googleMapUrl;
				});
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public IWpfWebBrowser WebBrowser
		{
			get => _webBrowser;
			set
			{
				if (value == null)
				{
					return;
				}

				_webBrowser = value;
				value.RequestHandler = new HookingRequestHandler();
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
		public IReadOnlyCollection<string> LocationPropositions
		{
			get => _locationPropositions;
			set => this.RaiseAndSetIfChanged(ref _locationPropositions, value);
		}
		public string SelectedLocation
		{
			get => _selectedLocation;
			set => this.RaiseAndSetIfChanged(ref _selectedLocation, value);
		}
		public string ShowUrl
		{
			get => _showUrl;
			set => this.RaiseAndSetIfChanged(ref _showUrl, value);
		}
		#endregion
		#endregion
	}
}