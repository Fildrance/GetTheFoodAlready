using System;
using System.Reactive;
using System.Reactive.Linq;

using GetTheFoodAlready.Api.Orchestration.Requests;

using ReactiveUI;

namespace GetTheFoodAlready.Ui.Wpf.ViewModels
{
	/// <summary> View-model for preparation of app usage. </summary>
	public class PreparationWizardViewModel : ReactiveObject
	{
		#region [c-tor]
		public PreparationWizardViewModel(
			SetupLocationViewModel setupLocation,
			SetupVendorPointBasicPreferencesViewModel setupVendorPointBasicPreferences,
			SetupFoodPreferencesViewModel setupFoodPreferencesViewModel
		)
		{
			SetupLocationViewModel = setupLocation ?? throw new ArgumentNullException(nameof(setupLocation));
			SetupVendorPointBasicPreferencesViewModel = setupVendorPointBasicPreferences ?? throw new ArgumentNullException(nameof(setupVendorPointBasicPreferences));
			SetupFoodPreferencesViewModel = setupFoodPreferencesViewModel ?? throw new ArgumentNullException(nameof(setupFoodPreferencesViewModel));

			FinishCommand = ReactiveCommand.Create(() => {

				SetupLocationViewModel.SaveSelected();

				var acceptableDeliveryTil = SetupVendorPointBasicPreferencesViewModel.SelectedAcceptableDeliveryTimeTil == null
					? null
					: (int?) SetupVendorPointBasicPreferencesViewModel.SelectedAcceptableDeliveryTimeTil.Value;

				RatingInfo ratingInfo = null;
				if (SetupVendorPointBasicPreferencesViewModel.IsRatingImportant)
				{
					ratingInfo = new RatingInfo
					(
						SetupVendorPointBasicPreferencesViewModel.MinimumRaiting.Value,
						SetupVendorPointBasicPreferencesViewModel.MinimumRateVoteCount.Value
					);
				}

				var request = new RandomFoodPropositionsRequest
				(
					SetupLocationViewModel.SelectedLocation,
					acceptableDeliveryTil,
					SetupVendorPointBasicPreferencesViewModel.GetSelectedCuisines(),
					SetupVendorPointBasicPreferencesViewModel.GetSelectedPaymentTypes(),
					SetupVendorPointBasicPreferencesViewModel.IsOnlyFreeDelivery,
					SetupVendorPointBasicPreferencesViewModel.MinimumOrderAmount,
					ratingInfo,
					SetupFoodPreferencesViewModel.GetExcludedMenuParts()
				);
				return request;
			});

			// todo remember not to fuck up and forget about 
			//"openDays": "0,1,2,3,4,5,6",
			//"openFrom": "11:00",
			//"openTo": "23:59",
			// and such stuff
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public ReactiveCommand<Unit, RandomFoodPropositionsRequest> FinishCommand { get; }
		public IObservable<RandomFoodPropositionsRequest> FinishObservable => FinishCommand.LoggedCatch(this, Observable.Return((RandomFoodPropositionsRequest) null));
		public SetupFoodPreferencesViewModel SetupFoodPreferencesViewModel { get; }
		public SetupLocationViewModel SetupLocationViewModel { get; }
		public SetupVendorPointBasicPreferencesViewModel SetupVendorPointBasicPreferencesViewModel { get; }
		#endregion
		#endregion
	}
}