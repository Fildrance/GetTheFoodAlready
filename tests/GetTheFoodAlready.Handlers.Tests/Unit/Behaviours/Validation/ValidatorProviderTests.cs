using System.Diagnostics.CodeAnalysis;

using FluentValidation;

using GetTheFoodAlready.Handlers.Behaviours.Validation;
using GetTheFoodAlready.TestsCommon;

using Moq;

using NUnit.Framework;

namespace GetTheFoodAlready.Handlers.Tests.Unit.Behaviours.Validation
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Unit")]
	public class ValidatorProviderTests
	{
		private ValidatorProvider _cut;

		private IValidator _validator1;
		private IValidator _validator2;
		private IValidator _validator3;

		[SetUp]
		public void Setup()
		{
			_validator1 = Mock.Of<IValidator>();
			_validator2 = Mock.Of<IValidator>();
			_validator3 = Mock.Of<IValidator>();

			_cut= new ValidatorProvider(new []{ _validator1, _validator2, _validator3 });
		}

		[Test]
		public void Validate()
		{
			//arrange
			_validator1.Setup(x => x.CanValidateInstancesOfType(typeof(string)))
				.Returns(false);
			_validator2.Setup(x => x.CanValidateInstancesOfType(typeof(string)))
				.Returns(true);
			_validator3.Setup(x => x.CanValidateInstancesOfType(typeof(string)))
				.Returns(true);
			//act
			var actual = _cut.GetValidators<string>();
			//assert
			CollectionAssert.AreEqual(new []{ _validator2, _validator3}, actual);
		}

		[Test]
		public void Validate_NotAcceptableType_ReturnEmpty()
		{
			//arrange
			_validator1.Setup(x => x.CanValidateInstancesOfType(typeof(string)))
				.Returns(false);
			_validator2.Setup(x => x.CanValidateInstancesOfType(typeof(string)))
				.Returns(false);
			_validator3.Setup(x => x.CanValidateInstancesOfType(typeof(string)))
				.Returns(false);
			//act
			var actual = _cut.GetValidators<string>();
			//assert
			CollectionAssert.IsEmpty(actual);
		}
	}
}
