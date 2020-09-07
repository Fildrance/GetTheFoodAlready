using System.Diagnostics.CodeAnalysis;

using FluentValidation;

using GetTheFoodAlready.Api.FoodAgregators.Requests;
using GetTheFoodAlready.Api.FoodAgregators.Requests.Validation;

using NUnit.Framework;

namespace GetTheFoodAlready.Api.Tests.Unit.FoodAgregators.Requests.Validation
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Unit")]
	public class ClosestVendorPointsGetRequestValidatorTests
	{
		private IValidator<ClosestVendorPointsGetRequest> _cut;

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

		[SetUp]
		public void Setup()
		{
			_cut = new ClosestVendorPointsGetRequestValidator();
		}

		[TestCaseSource(nameof(_testSourceData))]
		public void Validate(string lng, string lat, bool isValid)
		{
			//arrange
			var request = new ClosestVendorPointsGetRequest
			{
				Longitude = lng,
				Latitude = lat
			};
			//act
			var result = _cut.Validate(request);
			//assert
			Assert.AreEqual(isValid, result.IsValid);
		}
	}
}