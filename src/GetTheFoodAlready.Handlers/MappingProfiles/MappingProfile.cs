using System.Globalization;
using System.Linq;

using AutoMapper;

using GetTheFoodAlready.Api.FoodAgregators.Responses;
using GetTheFoodAlready.DeliveryClubBridge;
using GetTheFoodAlready.DeliveryClubBridge.DataTypes;

namespace GetTheFoodAlready.Handlers.MappingProfiles
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<DeliveryClubVendor, VendorInfo>()
				.ForMember(x => x.Id, opts => opts.MapFrom(src => src.Chain.Id.Primary))
				.ForMember(x => x.VendorAlias, opts => opts.MapFrom(src => src.Chain.Alias))
				.ForMember(x => x.VendorPointAlias, opts => opts.MapFrom(src => src.Alias))
				.ForMember(x => x.AvailablePaymentTypes, opts => opts.MapFrom(src => src.Payments.Select(x => x.Type)))
				.ForMember(x => x.DeliveryPrice, opts => opts.MapFrom(src => src.Delivery.Price.Value))
				.ForMember(x => x.MinOrderTotal, opts => opts.MapFrom(src => src.Delivery.MinOrderPrice.Value))
				.ForMember(x => x.RatingScore, opts => opts.MapFrom(src => src.Reviews.RatingScore))
				.ForMember(x => x.ReviewCount, opts => opts.MapFrom(src => src.Reviews.ReviewCount))
				.ForMember(x => x.Score, opts => opts.MapFrom(src => decimal.Parse(src.Reviews.Score, CultureInfo.InvariantCulture)))
				.ForMember(x => x.ScoreCount, opts => opts.MapFrom(src => src.Reviews.ScoreCount))
				.ForMember(x => x.DeliveryTime, opts => opts.MapFrom(src => src.Delivery.Time))
				.ForMember(x => x.IsDeliveringForFree, opts => opts.MapFrom(src => src.Labels.Any(label => label == DeliveryClubConstants.FreeDeliveryLabel)));

			CreateMap<MenuItem, FoodInfo>();
		}
	}
}