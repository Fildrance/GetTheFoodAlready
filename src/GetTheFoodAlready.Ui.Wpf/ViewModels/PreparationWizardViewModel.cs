﻿using System;
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
			SetupVendorPointPreferencesViewModel setupVendorPointPreferences,
			SetupFoodPreferencesViewModel setupFoodPreferencesViewModel
		)
		{
			SetupLocationViewModel = setupLocation ?? throw new ArgumentNullException(nameof(setupLocation));
			SetupVendorPointPreferencesViewModel = setupVendorPointPreferences ?? throw new ArgumentNullException(nameof(setupVendorPointPreferences));
			SetupFoodPreferencesViewModel = setupFoodPreferencesViewModel ?? throw new ArgumentNullException(nameof(setupFoodPreferencesViewModel));

			FinishCommand = ReactiveCommand.Create(() => {

				SetupLocationViewModel.SaveDefault();
				SetupVendorPointPreferencesViewModel.SaveDefault();
				SetupFoodPreferencesViewModel.SaveDefault();

				TimeSpan? acceptableDeliveryTil = null;
				if (SetupVendorPointPreferencesViewModel.SelectedAcceptableDeliveryTimeTil != null)
				{
					acceptableDeliveryTil = TimeSpan.FromMinutes(SetupVendorPointPreferencesViewModel.SelectedAcceptableDeliveryTimeTil.Value);
				}

				RatingInfo ratingInfo = null;
				if (SetupVendorPointPreferencesViewModel.IsRatingImportant)
				{
					ratingInfo = new RatingInfo
					(
						SetupVendorPointPreferencesViewModel.MinimumRaiting ?? 0,
						SetupVendorPointPreferencesViewModel.MinimumRateVoteCount ?? 0
					);
				}

				var request = new RandomFoodPropositionsRequest
				(
					SetupLocationViewModel.SelectedLocation,
					acceptableDeliveryTil,
					SetupVendorPointPreferencesViewModel.GetSelectedCuisines(),
					SetupVendorPointPreferencesViewModel.GetSelectedPaymentTypes(),
					SetupVendorPointPreferencesViewModel.IsOnlyFreeDelivery,
					SetupVendorPointPreferencesViewModel.MinimumOrderAmount,
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
		public SetupVendorPointPreferencesViewModel SetupVendorPointPreferencesViewModel { get; }
		#endregion
		#endregion
	}
}