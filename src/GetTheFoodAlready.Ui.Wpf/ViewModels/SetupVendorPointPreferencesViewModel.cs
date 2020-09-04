using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

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
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public IList<SelectBoxItem<int>> AcceptableDeliveryTimes { get; set; } = new List<SelectBoxItem<int>>
		{
			new SelectBoxItem<int>(15, "до 15 минут"),
			new SelectBoxItem<int>(30, "до 30 минут"),
			new SelectBoxItem<int>(45, "до 45 минут"),
			new SelectBoxItem<int>(60, "до 60 минут"),
			new SelectBoxItem<int>(90, "до 90 минут")
		};
		public IList<SelectBoxItem<string>> AvailableCuisines { get; set; } = new List<SelectBoxItem<string>>
		{
			new SelectBoxItem<string>("burger", "Бургеры"),
			new SelectBoxItem<string>("indiyskaya", "Индийская"),
		};
		public IList<SelectBoxItem<string>> AvailablePaymentTypes { get; set; } = new List<SelectBoxItem<string>>
		{
			new SelectBoxItem<string>("card_offline", "Картой курьеру"),
			new SelectBoxItem<string>("card_online", "Картой предварительно"),
			new SelectBoxItem<string>("androidpay", "Android pay"),
			new SelectBoxItem<string>("samsungpay", "Samsung pay"),
			new SelectBoxItem<string>("applepay", "Apple pay"),
			new SelectBoxItem<string>("sberspasibo", "Sberbank spasibo"),
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
			set => this.RaiseAndSetIfChanged(ref _isRatingImportant, value);
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
		public string MinimunOrderAmountLabel => $"Select minimum order amount ({MinimumOrderAmount})";
		public string MinimunRateVoteCountLabel => $"Select minimum amount of votes for rating ({MinimumRateVoteCount})";
		public string MinimunRatingLabel => $"Select minimum allowed rating ({MinimumRaiting:F1})";
		public int? SelectedAcceptableDeliveryTimeTil
		{
			get => _selectedAcceptableDeliveryTimeTil;
			set => this.RaiseAndSetIfChanged(ref _selectedAcceptableDeliveryTimeTil, value);
		}
		#endregion

		#region [Public methods]
		public IReadOnlyCollection<string> GetSelectedCuisines()
		{
			var items = AvailableCuisines.Where(x => x.IsSelected)
				.ToArray();
			return items.Any()
				? items.Select(x => x.Text)
					.ToArray()
				: Array.Empty<string>();
		}

		public IReadOnlyCollection<string> GetSelectedPaymentTypes()
		{
			var items = AvailablePaymentTypes.Where(x => x.IsSelected)
				.ToArray();
			return items.Any()
				? items.Select(x => x.Value)
					.ToArray()
				: Array.Empty<string>();
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