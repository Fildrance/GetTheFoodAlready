using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

using GetTheFoodAlready.Api.Maps;
using GetTheFoodAlready.Api.Maps.Requests;
using GetTheFoodAlready.Api.Maps.Responses;

using ReactiveUI;

namespace GetTheFoodAlready.Ui.Wpf.ViewModels
{
	/// <summary> View-model for setting up user location. </summary>
	public class SetupLocationViewModel : ReactiveObject
	{
		#region [Fields]
		private bool _isPropositionsListOpen;
		private string _location;
		private AddressInfo _selectedLocation;
		private string _showUrl;
		private IReadOnlyCollection<AddressInfo> _locationPropositions;

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
				.Where(x => x.Value.Length > 3 && LocationPropositions?.Any(a => a.AddressName == x.Value) != true )
				.DistinctUntilChanged()
				.Throttle(TimeSpan.FromMilliseconds(1500))
				.Subscribe(async x => {
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
				.Subscribe(x => {
					var encoded = Uri.EscapeUriString(x.Value.AddressName);
					var googleMapUrl = $"https://www.google.ru/maps/place/{encoded}/";
					ShowUrl = googleMapUrl;
				});
		}
		#endregion

		#region [Public]
		#region [Public properties]
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
		public IReadOnlyCollection<AddressInfo> LocationPropositions
		{
			get => _locationPropositions;
			set => this.RaiseAndSetIfChanged(ref _locationPropositions, value);
		}
		public AddressInfo SelectedLocation
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