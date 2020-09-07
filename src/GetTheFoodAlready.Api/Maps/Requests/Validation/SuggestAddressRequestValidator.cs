using FluentValidation;

namespace GetTheFoodAlready.Api.Maps.Requests.Validation
{
	public class SuggestAddressRequestValidator : AbstractValidator<SuggestAddressRequest>
	{
		public SuggestAddressRequestValidator()
		{
			RuleFor(x => x.Substring)
				.NotNull()
				.Length(3, 1000);
		}
	}
}
