using System;
using System.Collections.Generic;
using System.Linq;

using GetTheFoodAlready.Ui.Wpf.Support;

using ReactiveUI;

namespace GetTheFoodAlready.Ui.Wpf.ViewModels
{
	public class SetupVendorPointBasicPreferencesViewModel : ReactiveObject
	{
		#region [Static methods]
		#endregion

		#region [Fields]
		private IList<SelectBoxItem<string>> _availablePaymentTypes;
		private bool _isMinimumOrderAmountUsed;
		private bool _isOnlyFreeDelivery;
		private bool _isRatingImportant;
		private int? _minimumOrderAmount;
		private decimal? _minimumRaiting;
		private int? _minimumRateVoteCount;
		private SelectBoxItem<int> _selectedAcceptableDeliveryTimeTil;
		private IList<SelectBoxItem<string>> _selectedCuisines;
		#endregion

		#region [Public]
		#region [Public properties]
		public IList<SelectBoxItem<int>> AcceptableDeliveryTimes { get; } = new List<SelectBoxItem<int>>
		{
			new SelectBoxItem<int>(15, "до 15 минут"),
			new SelectBoxItem<int>(30, "до 30 минут"),
			new SelectBoxItem<int>(45, "до 45 минут"),
			new SelectBoxItem<int>(60, "до 60 минут"),
			new SelectBoxItem<int>(90, "до 90 минут")
		};
		public IList<SelectBoxItem<string>> AvailableCuisines { get; } = new List<SelectBoxItem<string>>
		{
			new SelectBoxItem<string>("burger", "Бургеры"),
			new SelectBoxItem<string>("indiyskaya", "Индийская"),
		};
		public IList<SelectBoxItem<string>> AvailablePaymentTypes { get; } = new List<SelectBoxItem<string>>
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
		public SelectBoxItem<int> SelectedAcceptableDeliveryTimeTil
		{
			get => _selectedAcceptableDeliveryTimeTil;
			set => this.RaiseAndSetIfChanged(ref _selectedAcceptableDeliveryTimeTil, value);
		}
		public IList<SelectBoxItem<string>> SelectedCuisines
		{
			get => _selectedCuisines;
			set => this.RaiseAndSetIfChanged(ref _selectedCuisines, value);
		}
		public IList<SelectBoxItem<string>> SelectedPaymentTypes
		{
			get => _availablePaymentTypes;
			set => this.RaiseAndSetIfChanged(ref _availablePaymentTypes, value);
		}
		#endregion

		#region [Public methods]
		public IReadOnlyCollection<string> GetSelectedCuisines()
		{
			return SelectedCuisines == null
				? Array.Empty<string>()
				: SelectedCuisines.Select(x => x.Text)
					.ToArray();
		}

		public IReadOnlyCollection<string> GetSelectedPaymentTypes()
		{
			return SelectedPaymentTypes == null
				? Array.Empty<string>()
				: SelectedPaymentTypes.Select(x => x.Value)
					.ToArray();
		}
		#endregion
		#endregion
	}
}