using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using GetTheFoodAlready.Api.FoodAgregators;
using GetTheFoodAlready.Api.FoodAgregators.Requests;
using GetTheFoodAlready.Ui.Wpf.Localization;
using GetTheFoodAlready.Ui.Wpf.Resources;
using GetTheFoodAlready.Ui.Wpf.Support;

using ReactiveUI;

namespace GetTheFoodAlready.Ui.Wpf.ViewModels
{
	public class SetupVendorPointPreferencesViewModel : ReactiveObject
	{
		#region [Fields]
		private readonly IDefaultManager<VendorPointPreferences> _defaultManager;
		private readonly IMapper _mapper;
		private readonly IDeliveryClubService _deliveryClubService;

		private IList<SelectBoxItem<string>> _availableCuisines;
		private bool _isMinimumOrderAmountUsed;
		private bool _isOnlyFreeDelivery;
		private bool _isRatingImportant;
		private int? _minimumOrderAmount;
		private decimal? _minimumRaiting;
		private int? _minimumRateVoteCount;
		private int? _selectedAcceptableDeliveryTimeTil;
		private bool _isReady;
		#endregion

		#region [c-tor]
		public SetupVendorPointPreferencesViewModel(
			IDefaultManager<VendorPointPreferences> defaultManager,
			IMapper mapper,
			IDeliveryClubService deliveryClubService
		)
		{
			_defaultManager = defaultManager ?? throw new ArgumentNullException(nameof(defaultManager));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
			_deliveryClubService = deliveryClubService ?? throw new ArgumentNullException(nameof(deliveryClubService));

			TranslationSource.Instance.DistinctUntilChanged()
				.Subscribe(x => this.RaisePropertyChanged(string.Empty));
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public IList<SelectBoxItem<int>> AcceptableDeliveryTimes { get; set; } = new List<SelectBoxItem<int>>
		{
			new SelectBoxItem<int>(15, () => string.Format(PreparationWizardLabels.AccaptableDeliveryTimeOptionLabel, 15), TranslationSource.Instance),
			new SelectBoxItem<int>(30, () => string.Format(PreparationWizardLabels.AccaptableDeliveryTimeOptionLabel, 30), TranslationSource.Instance),
			new SelectBoxItem<int>(45, () => string.Format(PreparationWizardLabels.AccaptableDeliveryTimeOptionLabel, 45), TranslationSource.Instance),
			new SelectBoxItem<int>(60, () => string.Format(PreparationWizardLabels.AccaptableDeliveryTimeOptionLabel, 60), TranslationSource.Instance),
			new SelectBoxItem<int>(90, () => string.Format(PreparationWizardLabels.AccaptableDeliveryTimeOptionLabel, 90), TranslationSource.Instance)
		};
		public IList<SelectBoxItem<string>> AvailableCuisines
		{
			get => _availableCuisines;
			set => this.RaiseAndSetIfChanged(ref _availableCuisines, value);
		}
		public IList<SelectBoxItem<string>> AvailablePaymentTypes { get; set; } = new List<SelectBoxItem<string>>
		{
			new SelectBoxItem<string>("card_offline", () => PreparationWizardLabels.PaymentTypeLabelCourierCard, TranslationSource.Instance),
			new SelectBoxItem<string>("card_online", () => PreparationWizardLabels.PaymentTypeLabelCardOnline, TranslationSource.Instance),
			new SelectBoxItem<string>("androidpay", () => PreparationWizardLabels.PaymentTypeLabelAndroidPay, TranslationSource.Instance),
			new SelectBoxItem<string>("samsungpay", () => PreparationWizardLabels.PaymentTypeLabelSamsungPay, TranslationSource.Instance),
			new SelectBoxItem<string>("applepay", () => PreparationWizardLabels.PaymentTypeLabelApplePay, TranslationSource.Instance),
			new SelectBoxItem<string>("sberspasibo", () => PreparationWizardLabels.PaymentTypeLabelSberbankSpasibo, TranslationSource.Instance),
		};
		public bool IsMinimumOrderAmountUsed
		{
			get => _isMinimumOrderAmountUsed;
			set => this.RaiseAndSetIfChanged(ref _isMinimumOrderAmountUsed, value);
		}
		public bool IsOnlyFreeDelivery
		{
			get => _isOnlyFreeDelivery;
			set => this.RaiseAndSetIfChanged(ref _isOnlyFreeDelivery, value);
		}
		public bool IsRatingImportant
		{
			get => _isRatingImportant;
			set
			{
				this.RaiseAndSetIfChanged(ref _isRatingImportant, value);
			}
		}
		public int? MinimumOrderAmount
		{
			get => _minimumOrderAmount;
			set
			{
				this.RaiseAndSetIfChanged(ref _minimumOrderAmount, value);
				this.RaisePropertyChanged(nameof(MinimunOrderAmountLabel));
			}
		}
		public decimal? MinimumRaiting
		{
			get => _minimumRaiting;
			set
			{
				this.RaiseAndSetIfChanged(ref _minimumRaiting, value);
				this.RaisePropertyChanged(nameof(MinimunRatingLabel));
			}
		}
		public int? MinimumRateVoteCount
		{
			get => _minimumRateVoteCount;
			set
			{
				this.RaiseAndSetIfChanged(ref _minimumRateVoteCount, value);
				this.RaisePropertyChanged(nameof(MinimunRateVoteCountLabel));
			}
		}
		public bool IsBusy
		{
			get => _isReady;
			set
			{
				this.RaiseAndSetIfChanged(ref _isReady, value);
				this.RaisePropertyChanged(nameof(IsReady));
			}
		}
		public bool IsReady { get => !_isReady; }

		public string MinimunOrderAmountLabel => string.Format(PreparationWizardLabels.IsRatingImportant, MinimumOrderAmount);
		public string MinimunRateVoteCountLabel => string.Format(PreparationWizardLabels.MinimunRateVoteCountLabel, MinimumRateVoteCount);
		public string MinimunRatingLabel => string.Format(PreparationWizardLabels.MinimunRatingLabel, MinimumRaiting?.ToString("F1"));
		public int? SelectedAcceptableDeliveryTimeTil
		{
			get => _selectedAcceptableDeliveryTimeTil;
			set => this.RaiseAndSetIfChanged(ref _selectedAcceptableDeliveryTimeTil, value);
		}
		#endregion

		#region [Public methods]
		public IReadOnlyCollection<string> GetSelectedCuisines()
		{
			return Utilities.GetSelectedValues(AvailableCuisines);
		}

		public IReadOnlyCollection<string> GetSelectedPaymentTypes()
		{
			return Utilities.GetSelectedValues(AvailablePaymentTypes);
		}

		public async Task<SetupVendorPointPreferencesViewModel> Setup(SetupLocationViewModel previousPageViewModel)
		{
			IsBusy = true;
			var request = new ClosestVendorPointsGetRequest
			{
				Latitude = previousPageViewModel.SelectedLocation.Latitude,
				Longitude = previousPageViewModel.SelectedLocation.Longitude
			};
			var requestResult = await _deliveryClubService.GetClosestVendorPoints(request, CancellationToken.None);

			var foundCuisines = requestResult.Vendors.SelectMany(x => x.Cuisines)
				.Distinct();
			AvailableCuisines = foundCuisines.Select(x => new SelectBoxItem<string>(x, x))
				.ToArray();

			var defaults = await _defaultManager.GetDefault();
			_mapper.Map(defaults, this);
			IsBusy = false;
			return this;
		}

		public void SaveDefault()
		{
			var mapped = _mapper.Map<VendorPointPreferences>(this);
			_defaultManager.SaveDefault(mapped);
		}
		#endregion
		#endregion
	}
}