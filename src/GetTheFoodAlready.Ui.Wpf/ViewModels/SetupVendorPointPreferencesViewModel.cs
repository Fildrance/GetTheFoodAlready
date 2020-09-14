using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;

using AutoMapper;
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

		private bool _isMinimumOrderAmountUsed;
		private bool _isOnlyFreeDelivery;
		private bool _isRatingImportant;
		private int? _minimumOrderAmount;
		private decimal? _minimumRaiting;
		private int? _minimumRateVoteCount;
		private int? _selectedAcceptableDeliveryTimeTil;
		#endregion

		#region [c-tor]
		public SetupVendorPointPreferencesViewModel(
			IDefaultManager<VendorPointPreferences> defaultManager,
			IMapper mapper
		)
		{
			_defaultManager = defaultManager ?? throw new ArgumentNullException(nameof(defaultManager));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

			TranslationSource.Instance.DistinctUntilChanged()
				.Subscribe(x => {
					this.RaisePropertyChanged(string.Empty);

				});
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
		public IList<SelectBoxItem<string>> AvailableCuisines { get; set; } = new List<SelectBoxItem<string>>
		{
			new SelectBoxItem<string>("Бургеры", () => PreparationWizardLabels.CuisineTypeLabelBurger, TranslationSource.Instance),
			new SelectBoxItem<string>("Индийская", () => PreparationWizardLabels.CuisineTypeLabelIndian, TranslationSource.Instance),
		};
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

		public async Task<SetupVendorPointPreferencesViewModel> SetupDefault()
		{
			var defaults = await _defaultManager.GetDefault();
			_mapper.Map(defaults, this);
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