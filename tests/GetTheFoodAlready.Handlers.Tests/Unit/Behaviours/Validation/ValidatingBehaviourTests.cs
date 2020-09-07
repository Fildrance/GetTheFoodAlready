using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;
using FluentValidation.Results;

using GetTheFoodAlready.Handlers.Behaviours.Validation;
using GetTheFoodAlready.TestsCommon;

using MediatR;

using Moq;

using NUnit.Framework;

namespace GetTheFoodAlready.Handlers.Tests.Unit.Behaviours.Validation
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Unit")]
	public class ValidatingBehaviourTests
	{
		private ValidatingBehaviour<string, object> _cut;

		private IValidatorProvider _validatorProviderStub;
		private IValidator _validator1;
		private IValidator _validator2;
		private IValidator _validator3;
		private string _request = "someRequest";

		[SetUp]
		public void Setup()
		{
			_validatorProviderStub = Mock.Of<IValidatorProvider>();
			_validatorProviderStub.Setup(x => x.GetValidators<string>())
				.Returns(Array.Empty<IValidator>());

			_validator1 = Mock.Of<IValidator>();
			_validator2 = Mock.Of<IValidator>();
			_validator3 = Mock.Of<IValidator>();

			_validator1.Setup(x => x.Validate(It.Is<ValidationContext<string>>(c => c.InstanceToValidate == _request)))
				.Returns(new ValidationResult());
			_validator2.Setup(x => x.Validate(It.Is<ValidationContext<string>>(c => c.InstanceToValidate == _request)))
				.Returns(new ValidationResult());
			_validator3.Setup(x => x.Validate(It.Is<ValidationContext<string>>(c => c.InstanceToValidate == _request)))
				.Returns(new ValidationResult());

			_cut = new ValidatingBehaviour<string, object>(_validatorProviderStub);
		}

		[Test]
		public async Task Handle_ReturnsNestedEmpty_ReturnsResultFromNested()
		{
			//arrange
			var expected = new object();
			RequestHandlerDelegate<object> requestHandlerDelegate = () => Task.FromResult(expected);
			//act
			var actual = await _cut.Handle(_request, CancellationToken.None, requestHandlerDelegate);
			//assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public async Task Handle_ReturnsMutlipleValidatorsThatReturnsValid_ReturnsResultFromNested()
		{
			//arrange
			var expected = new object();
			RequestHandlerDelegate<object> requestHandlerDelegate = () => Task.FromResult(expected);
			_validatorProviderStub.DropSetup()
				.Setup(x=>x.GetValidators<string>())
				.Returns(new []{_validator1,_validator2,_validator3});

			//act
			var actual = await _cut.Handle(_request, CancellationToken.None, requestHandlerDelegate);
			//assert
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Handle_ReturnsMutlipleValidatorsThatReturnsInvalid_Throws()
		{
			//arrange
			var expected = new object();
			RequestHandlerDelegate<object> requestHandlerDelegate = () => Task.FromResult(expected);
			_validatorProviderStub.DropSetup()
				.Setup(x => x.GetValidators<string>())
				.Returns(new[] { _validator1, _validator2, _validator3 });

			_validator1.DropSetup()
				.Setup(x => x.Validate(It.Is<ValidationContext<string>>(c => c.InstanceToValidate == _request)))
				.Returns(new ValidationResult( new [] { new ValidationFailure(nameof(string.Length), "too long") }));

			//act
			var ex = Assert.ThrowsAsync<ValidationException>(()=> _cut.Handle(_request, CancellationToken.None, requestHandlerDelegate));
			//assert
			Assert.AreEqual(1, ex.Errors.Count());
			Assert.True(ex.Errors.Any(x=>x.ErrorMessage.Contains("too long")));
		}

		[Test]
		public void Handle_ReturnsMutlipleValidatorsEveryReturnsInvalid_Throws()
		{
			//arrange
			var expected = new object();
			RequestHandlerDelegate<object> requestHandlerDelegate = () => Task.FromResult(expected);
			_validatorProviderStub.DropSetup()
				.Setup(x => x.GetValidators<string>())
				.Returns(new[] { _validator1, _validator2, _validator3 });

			_validator1.DropSetup()
				.Setup(x => x.Validate(It.Is<ValidationContext<string>>(c => c.InstanceToValidate == _request)))
				.Returns(new ValidationResult(new[] { new ValidationFailure(nameof(string.Length), "too long2") }));
			_validator2.DropSetup()
				.Setup(x => x.Validate(It.Is<ValidationContext<string>>(c => c.InstanceToValidate == _request)))
				.Returns(new ValidationResult(new[] { new ValidationFailure(nameof(string.Length), "too long3") }));
			_validator3.DropSetup()
				.Setup(x => x.Validate(It.Is<ValidationContext<string>>(c => c.InstanceToValidate == _request)))
				.Returns(new ValidationResult(new[] { new ValidationFailure(nameof(string.Length), "too long4") }));

			//act
			var ex = Assert.ThrowsAsync<ValidationException>(() => _cut.Handle(_request, CancellationToken.None, requestHandlerDelegate));
			//assert
			Assert.True(ex.Errors.Any(x => x.ErrorMessage.Contains("too long2")));
			Assert.True(ex.Errors.Any(x => x.ErrorMessage.Contains("too long3")));
			Assert.True(ex.Errors.Any(x => x.ErrorMessage.Contains("too long4")));
		}
	}
}