using System;
using System.Collections.ObjectModel;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;

using GetTheFoodAlready.Api.FoodAgregators.Responses;
using GetTheFoodAlready.Api.Orchestration;
using GetTheFoodAlready.Api.Orchestration.Requests;

using ReactiveUI;

namespace GetTheFoodAlready.Ui.Wpf.ViewModels
{
	public class FoodsListViewModel : ReactiveObject
	{
		#region  [Constants]
		private const string DeliveryVendorPageBaseUrl = "https://www.delivery-club.ru/srv/";
		#endregion

		#region [Fields]
		private ObservableCollection<FoodInfo> _allFoodItems;
		private readonly ISubject<bool> _canExecuteActionsWithProducs = new ReplaySubject<bool>(1);
		private ObservableCollection<FoodInfo> _foodItems;

		private readonly IOrchestrationService _orchestrationService;
		private ObservableCollection<FoodInfo> _proposedFoodItems;
		private RandomFoodPropositionsRequest _randomFoodPropositionsRequest;
		private VendorInfo _vendor;
		#endregion

		#region [c-tor]
		public FoodsListViewModel(IOrchestrationService orchestrationService)
		{
			_orchestrationService = orchestrationService ?? throw new ArgumentNullException(nameof(orchestrationService));

			GetRandomFoodCommand = ReactiveCommand.Create(async () =>
			{
				if (_randomFoodPropositionsRequest == null)
				{
					throw new InvalidOperationException("No request info, cannot make request for random food.");
				}
				await DoRollRandomFood(_randomFoodPropositionsRequest);
			}, _canExecuteActionsWithProducs);
			SwitchFoodList = ReactiveCommand.Create(() => {
				_foodItems = _foodItems == _allFoodItems 
					? _proposedFoodItems 
					: _allFoodItems;
				this.RaisePropertyChanged(nameof(FoodItems));
			}, _canExecuteActionsWithProducs);
			GoToVendorPage = ReactiveCommand.Create(() => {
				System.Diagnostics.Process.Start(DeliveryVendorPageBaseUrl + Vendor.VendorAlias);
			}, _canExecuteActionsWithProducs);
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public RandomFoodPropositionsRequest FilterInfo
		{
			get => _randomFoodPropositionsRequest;
			set
			{
				_randomFoodPropositionsRequest = value;
				GetRandomFoodCommand.Execute(null);
			}
		}
		public ObservableCollection<FoodInfo> FoodItems
		{
			get => _foodItems ?? (_foodItems = _proposedFoodItems);
			set => this.RaiseAndSetIfChanged(ref _foodItems, value);
		}
		public VendorInfo Vendor
		{
			get => _vendor;
			set => this.RaiseAndSetIfChanged(ref _vendor, value);
		}
		public ICommand GetRandomFoodCommand { get; }
		public ICommand GoToVendorPage { get; }
		public ICommand SwitchFoodList { get; }
		#endregion

		#region [Public methods]
		public async Task RollRandomFood(RandomFoodPropositionsRequest request)
		{
			_randomFoodPropositionsRequest = request;
			await DoRollRandomFood(request);
		}
		#endregion
		#endregion

		#region [Private]
		#region [Private methods]
		private async Task DoRollRandomFood(RandomFoodPropositionsRequest request)
		{
			_canExecuteActionsWithProducs.OnNext(false);

			var result = await _orchestrationService.GetRandomFoodPropositions(request);
			_allFoodItems = new ObservableCollection<FoodInfo>(result.FullListOfFoodInfos);
			_proposedFoodItems = new ObservableCollection<FoodInfo>(result.ProposedFoods);
			_foodItems = _proposedFoodItems;
			this.RaisePropertyChanged(nameof(FoodItems));

			_canExecuteActionsWithProducs.OnNext(true);

			Vendor = result.VendorInfo;
		}
		#endregion
		#endregion
	}
}
