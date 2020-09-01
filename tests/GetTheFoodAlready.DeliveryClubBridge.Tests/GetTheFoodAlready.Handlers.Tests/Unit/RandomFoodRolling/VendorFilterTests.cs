using System;
using System.Diagnostics.CodeAnalysis;

using GetTheFoodAlready.Api.FoodAgregators.Responses;
using GetTheFoodAlready.Api.Orchestration.Requests;
using GetTheFoodAlready.Api.Support;
using GetTheFoodAlready.Handlers.RandomFoodRolling;
using GetTheFoodAlready.Handlers.Tests.Support;
using GetTheFoodAlready.TestsCommon;

using Moq;

using NUnit.Framework;

namespace GetTheFoodAlready.Handlers.Tests.Unit.RandomFoodRolling
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Unit")]
	public class VendorFilterTests
	{
		private IVendorFilter _cut;

		private ITimeSpanParser _parser;

		[SetUp]
		public void Setup()
		{
			_parser = Mock.Of<ITimeSpanParser>();

			_cut = new VendorFilter(_parser);
		}

		[Test]
		public void Filter_EmptyFilterDataInRequest_ReturnAll()
		{
			//arrange
			var request = TestDataFactory.CreateRequest();
			var fullVendorsList = new[]
			{
				CreateVendor(),
				CreateVendor(),
				CreateVendor()
			};
			//act
			var result = _cut.FilterVendors(request, fullVendorsList);
			//assert
			CollectionAssert.AreEquivalent(fullVendorsList, result);
		}

		[Test]
		public void Filter_MentionRating_ReturnFilteredByRating()
		{
			//arrange
			var ratingInfo = new RatingInfo(3, 400);
			var request = TestDataFactory.CreateRequest(rating: ratingInfo);
			var vendor1 = CreateVendor(100, 2.1m);
			var vendor2 = CreateVendor(300, 2.1m);
			var vendor3 = CreateVendor(400, 4.1m);
			//act
			var result = _cut.FilterVendors(request, new []{ vendor3, vendor1, vendor2 });
			//assert
			CollectionAssert.AreEquivalent(new []{ vendor3 }, result);
		}

		[Test]
		public void Filter_MentionDeliveryTime_ReturnFilteredByDeliveryTime()
		{
			//arrange
			var request = TestDataFactory.CreateRequest(deliveryTime: TimeSpan.FromMinutes(51));

			var vendor1 = CreateVendor(deliveryTime:"1");
			var vendor2 = CreateVendor(deliveryTime:"50");
			var vendor3 = CreateVendor(deliveryTime:"100");
			_parser.Setup(x => x.GetSpan(It.IsAny<string>()))
				.Returns<string>(str => TimeSpan.FromMinutes(int.Parse(str)));
			//act
			var result = _cut.FilterVendors(request, new[] { vendor1, vendor2, vendor3 });
			//assert
			CollectionAssert.AreEquivalent(new[] { vendor1, vendor2 }, result);
		}

		[Test]
		public void Filter_MentionFreeDelivery_ReturnFilteredByFreeDelivery()
		{
			//arrange
			var request = TestDataFactory.CreateRequest(freeDelivery: true);
			var vendor1 = CreateVendor(freeDelivery: true);
			var vendor2 = CreateVendor();
			var vendor3 = CreateVendor(freeDelivery: true);
			//act
			var result = _cut.FilterVendors(request, new[] { vendor1, vendor2, vendor3 });
			//assert
			CollectionAssert.AreEquivalent(new[] { vendor1, vendor3 }, result);
		}

		[Test]
		public void Filter_MentionCuisine_ReturnOnlyWithMentionedCuisine()
		{
			//arrange
			var cuisine4 = "cus4";
			var cuisine3 = "cus3";
			var cuisine2 = "cus2";
			var cuisine1 = "cus1";

			var request = TestDataFactory.CreateRequest(cuisines: new[] { cuisine3, cuisine4 });

			var vendor1 = CreateVendor(cuisine: new [] { cuisine1, cuisine2, cuisine3 });
			var vendor2 = CreateVendor(cuisine: new[] { cuisine1, cuisine3 });
			var vendor3 = CreateVendor(cuisine: new[] { cuisine1, cuisine2 });
			//act
			var result = _cut.FilterVendors(request, new[] { vendor1, vendor2, vendor3 });
			//assert
			CollectionAssert.AreEquivalent(new[] { vendor1, vendor2 }, result);
		}

		[Test]
		public void Filter_MentionPaymentType_ReturnOnlyWithMentionedPaymentType()
		{
			//arrange
			var paymentT4 = "pt4";
			var paymentT3 = "pt3";
			var paymentT2 = "pt2";
			var paymentT1 = "pt1";

			var request = TestDataFactory.CreateRequest(paymentTypes: new[] { paymentT1, paymentT3 });

			var vendor1 = CreateVendor(paymentTypes: new[] { paymentT1, paymentT3 });
			var vendor2 = CreateVendor(paymentTypes: new[] { paymentT2, paymentT4 });
			var vendor3 = CreateVendor(paymentTypes: new[] { paymentT2, paymentT3 });
			//act
			var result = _cut.FilterVendors(request, new[] { vendor1, vendor2, vendor3 });
			//assert
			CollectionAssert.AreEquivalent(new[] { vendor1, vendor3 }, result);
		}

		private static VendorInfo CreateVendor(int? scoreCount = null, decimal? score = null, bool freeDelivery = false, string deliveryTime = "", string[] cuisine= null, string[] paymentTypes = null)
		{
			return new VendorInfo
			{
				Cuisines = cuisine ?? new string[0],
				AvailablePaymentTypes = paymentTypes ?? new string[0],
				Score = score,
				ScoreCount = scoreCount,
				IsDeliveringForFree = freeDelivery,
				DeliveryTime = deliveryTime
			};
		}
	}
}