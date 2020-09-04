using System.Linq;

using AutoMapper;

using GetTheFoodAlready.Ui.Wpf.Support;
using GetTheFoodAlready.Ui.Wpf.ViewModels;

namespace GetTheFoodAlready.Ui.Wpf.MappingProfiles
{
	/// <summary> Mapping profile for resetting default values of viewModels. </summary>
	public class ViewModelToSafeDefaultsMappingProfile : Profile
	{
		public ViewModelToSafeDefaultsMappingProfile()
		{
			CreateMap<FoodPreference, SetupFoodPreferencesViewModel>()
				.ForMember(x => x.AvailableToExcludeMenuParts, opts => opts.Ignore())
				.AfterMap((defaults, viewModel) =>
				{
					ViewModelExtensions.RegenerateListFromOriginalElements(viewModel.AvailableToExcludeMenuParts, defaults.SelectedMenuPartsToExclude);
				})
				.ReverseMap()
				.ForMember(x => x.SelectedMenuPartsToExclude, opts => opts.MapFrom(src => src.AvailableToExcludeMenuParts.Where(c => c.IsSelected).Select(x => x.Value).ToArray()));

			CreateMap<VendorPointPreferences, SetupVendorPointPreferencesViewModel>()
				.ForMember(x => x.AvailablePaymentTypes, opts => opts.Ignore())
				.ForMember(x => x.AvailableCuisines, opts => opts.Ignore())
				.AfterMap((defaults, viewModel) =>
				{
					ViewModelExtensions.RegenerateListFromOriginalElements(viewModel.AvailablePaymentTypes, defaults.SelectedPaymentTypes);
					ViewModelExtensions.RegenerateListFromOriginalElements(viewModel.AvailableCuisines, defaults.SelectedCuisines);
				})
				.ReverseMap()
				.ForMember(x => x.SelectedCuisines, opts => opts.MapFrom(src => src.AvailableCuisines.Where(c => c.IsSelected).Select(x => x.Value).ToArray()))
				.ForMember(x => x.SelectedPaymentTypes, opts => opts.MapFrom(src => src.AvailablePaymentTypes.Where(c => c.IsSelected).Select(x => x.Value).ToArray()));
		}
	}
}