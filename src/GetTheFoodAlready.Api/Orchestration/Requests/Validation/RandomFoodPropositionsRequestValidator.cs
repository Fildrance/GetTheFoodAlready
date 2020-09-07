using System;

using FluentValidation;

using GetTheFoodAlready.Api.Support;

namespace GetTheFoodAlready.Api.Orchestration.Requests.Validation
{
	public class RandomFoodPropositionsRequestValidator : AbstractValidator<RandomFoodPropositionsRequest>
	{
		public RandomFoodPropositionsRequestValidator(IValidator<AddressInfo> addressValidator)
		{
			RuleFor(x => x.AddressInfo)
				.NotNull()
				.SetValidator(addressValidator);
			RuleFor(x => x.AcceptableCuisineTypes)
				.NotNull();
			RuleFor(x => x.AcceptablePaymentTypes)
				.NotNull();
			RuleFor(x => x.FoodCategoryExceptions)
				.NotNull();
			RuleFor(x => x.AcceptableDeliveryTimeTil)
				.GreaterThan(TimeSpan.FromMinutes(10))
				.When(x => x.AcceptableDeliveryTimeTil.HasValue);
		}
	}
}
