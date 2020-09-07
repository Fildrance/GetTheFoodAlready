using System.Diagnostics.CodeAnalysis;

using FluentValidation;

using GetTheFoodAlready.Api.Maps.Requests;
using GetTheFoodAlready.Api.Maps.Requests.Validation;

using NUnit.Framework;

namespace GetTheFoodAlready.Api.Tests.Unit.Maps.Requests.Validation
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Unit")]
	public class SuggestAddressRequestValidatorTests
	{
		private IValidator<SuggestAddressRequest> _cut;
		[SetUp]
		public void Setup()
		{
			_cut = new SuggestAddressRequestValidator();
		}

		[Test]
		public void Validate()
		{
			//arrange
			var request = new SuggestAddressRequest
			{
				Substring = "some address"
			};
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.True(result.IsValid);
		}
		[Test]
		public void Validate_SmallSbstr_Invalid()
		{
			//arrange
			var request = new SuggestAddressRequest
			{
				Substring = "sa"
			};
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.False(result.IsValid);
		}
		[Test]
		public void Validate_SbstrIsNull_Invalid()
		{
			//arrange
			var request = new SuggestAddressRequest
			{
				Substring = null
			};
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.False(result.IsValid);
		}

	}
}