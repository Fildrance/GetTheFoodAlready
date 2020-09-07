using System.Diagnostics.CodeAnalysis;

using FluentValidation;

using GetTheFoodAlready.Api.Support;
using GetTheFoodAlready.Api.Support.Validation;

using NUnit.Framework;

namespace GetTheFoodAlready.Api.Tests.Unit.Support.Validation
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Unit")]
	public class AddressInfoValidatorTests
	{
		private IValidator<AddressInfo> _cut;

		[SetUp]
		public void Setup()
		{
			_cut = new AddressInfoValidator();
		}
		private static object[][] _testSourceData = {
			new object[] { "32.4322", "15.3241", true},
			new object[] { "100.4322", "15.3241", false},
			new object[] { "10.4322", "100.3241", false},
			new object[] { "100.4322", "100.3241", false},
			new object[] { "32", "15", false},
			new object[] {"-31.543", "-45.1241" , true},
			new object[] {"-31.543", "" , false},
			new object[] {"", "-31.543", false},
			new object[] {"lonbg", "lat", false},
			new object[] {"", "", false},
			new object[] {null, null, false}
		};

		[TestCaseSource(nameof(_testSourceData))]
		public void Validate(string lng, string lat, bool isValid)
		{
			//arrange
			var request = new AddressInfo("some name", lat, lng);
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.AreEqual(isValid, result.IsValid);
		}
	}
}