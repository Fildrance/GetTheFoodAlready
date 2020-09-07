using FluentValidation;

namespace GetTheFoodAlready.Api.Support.Validation
{
	public class AddressInfoValidator : AbstractValidator<AddressInfo>
	{
		public AddressInfoValidator()
		{
			RuleFor(x => x.Longitude)
				.NotNull()
				.Matches(@"^-?([1-8]?[0-9]\.{1}\d{1,6}$|90\.{1}0{1,6}$)");
			RuleFor(x => x.Latitude)
				.NotNull()
				.Matches(@"^-?([1-8]?[0-9]\.{1}\d{1,6}$|90\.{1}0{1,6}$)");
		}
	}
}
