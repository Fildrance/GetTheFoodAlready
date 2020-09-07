using System;
using System.Diagnostics.CodeAnalysis;

using FluentValidation;
using FluentValidation.Results;

using GetTheFoodAlready.Api.Orchestration.Requests;
using GetTheFoodAlready.Api.Orchestration.Requests.Validation;
using GetTheFoodAlready.Api.Support;
using GetTheFoodAlready.TestsCommon;

using Moq;

using NUnit.Framework;

namespace GetTheFoodAlready.Api.Tests.Unit.Orchestration.Requests.Validation
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Unit")]
	public class RandomFoodPropositionsRequestValidatorTests
	{
		private IValidator<RandomFoodPropositionsRequest> _cut;

		private IValidator<AddressInfo> _addressInfoValidatorStub;
		private readonly AddressInfo _addressInfo = new AddressInfo("", "", "");

		[SetUp]
		public void Setup()
		{
			_addressInfoValidatorStub = Mock.Of<IValidator<AddressInfo>>();
			_addressInfoValidatorStub.Setup(x => x.Validate(It.Is<IValidationContext>(c => c.InstanceToValidate == _addressInfo)))
				.Returns(new ValidationResult());
			
			_cut = new RandomFoodPropositionsRequestValidator(_addressInfoValidatorStub);
		}

		[Test]
		public void Validate()
		{
			//arrange
			var request = new RandomFoodPropositionsRequest
			(
				_addressInfo,
				TimeSpan.FromMinutes(11),
				new string[0],
				new string[0],
				true,
				null,
				null,
				new string[0]
			);
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.True(result.IsValid);
		}

		[Test]
		public void Validate_DeliveryTimeIsNull_IsValid()
		{
			//arrange
			var request = new RandomFoodPropositionsRequest
			(
				_addressInfo,
				null,
				new string[0],
				new string[0],
				true,
				null,
				null,
				new string[0]
			);
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.True(result.IsValid);
		}

		[Test]
		public void Validate_AddressInfoIsInvalid_IsInvalid()
		{
			//arrange
			var request = new RandomFoodPropositionsRequest
			(
				_addressInfo,
				null,
				new string[0],
				new string[0],
				true,
				null,
				null,
				new string[0]
			);
			_addressInfoValidatorStub.DropSetup();
			_addressInfoValidatorStub.Setup(x => x.Validate(It.Is<IValidationContext>(c => c.InstanceToValidate == _addressInfo)))
				.Returns(new ValidationResult(new[] { new ValidationFailure(nameof(AddressInfo.Longitude), "errorHappened") }));
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.False(result.IsValid);
		}

		[Test]
		public void Validate_PassRatingInfo_IsValid()
		{
			//arrange
			var request = new RandomFoodPropositionsRequest
			(
				_addressInfo,
				TimeSpan.FromMinutes(11),
				new string[0],
				new string[0],
				true,
				null,
				new RatingInfo(1.2m, 11), 
				new string[0]
			);
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.True(result.IsValid);
		}

		[Test]
		public void Validate_FoodCategoriesIsNull_IsInvalid()
		{
			//arrange
			var request = new RandomFoodPropositionsRequest
			(
				_addressInfo,
				TimeSpan.FromMinutes(11),
				new string[0],
				new string[0],
				true,
				null,
				null,
				null
			);
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.False(result.IsValid);
		}

		[Test]
		public void Validate_AcceptableTimeIs10Min_IsInvalid()
		{
			//arrange
			var request = new RandomFoodPropositionsRequest
			(
				_addressInfo,
				TimeSpan.FromMinutes(10),
				new string[0],
				new string[0],
				true,
				null,
				null,
				new string[0]
			);
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.False(result.IsValid);
		}

		[Test]
		public void Validate_AcceptableTimeIsLessThen10Min_IsInvalid()
		{
			//arrange
			var request = new RandomFoodPropositionsRequest
			(
				_addressInfo,
				TimeSpan.FromMinutes(9),
				new string[0],
				new string[0],
				true,
				null,
				null,
				new string[0]
			);
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.False(result.IsValid);
		}

		[Test]
		public void Validate_AcceptableTimeIsLessThen0Min_IsInvalid()
		{
			//arrange
			var request = new RandomFoodPropositionsRequest
			(
				_addressInfo,
				TimeSpan.FromMinutes(-12),
				new string[0],
				new string[0],
				true,
				null,
				null,
				new string[0]
			);
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.False(result.IsValid);
		}

		[Test]
		public void Validate_CuisinesIsNull_IsInvalid()
		{
			//arrange
			var request = new RandomFoodPropositionsRequest
			(
				_addressInfo,
				TimeSpan.FromMinutes(11),
				null,
				new string[0],
				true,
				null,
				null,
				null
			);
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.False(result.IsValid);
		}

		[Test]
		public void Validate_AcceptablePaymentTypesIsNull_IsInvalid()
		{
			//arrange
			var request = new RandomFoodPropositionsRequest
			(
				_addressInfo,
				TimeSpan.FromMinutes(11),
				new string[0],
				null,
				true,
				null,
				null,
				new string[0]
			);
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.False(result.IsValid);
		}
	}
}