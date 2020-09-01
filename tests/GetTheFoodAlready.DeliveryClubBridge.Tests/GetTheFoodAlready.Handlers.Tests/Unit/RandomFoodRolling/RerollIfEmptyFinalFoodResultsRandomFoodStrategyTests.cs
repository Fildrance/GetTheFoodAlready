using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.FoodAgregators;
using GetTheFoodAlready.Api.FoodAgregators.Requests;
using GetTheFoodAlready.Api.FoodAgregators.Responses;
using GetTheFoodAlready.Handlers.RandomFoodRolling;
using GetTheFoodAlready.Handlers.Tests.Support;
using GetTheFoodAlready.TestsCommon;

using Moq;

using NUnit.Framework;

namespace GetTheFoodAlready.Handlers.Tests.Unit.RandomFoodRolling
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Unit")]
	public class RerollIfEmptyFinalFoodResultsRandomFoodStrategyTests
	{
		private RerollIfEmptyFinalFoodResultsRandomFoodStrategy _cut;

		private IDeliveryClubService _deliveryClubService;

		#region vendor-infos
		private readonly VendorInfo _vendorInfo = new VendorInfo
		{
			Id = 1,
			DisplayName = "v1",
			Cuisines = new []{ "c1", "c2"}
		};

		private readonly VendorInfo _vendorInfo2 = new VendorInfo
		{
			Id = 2,
			DisplayName = "v2",
			Cuisines = new []{ "c1", "c2"}
		};

		private readonly VendorInfo _vendorInfo3 = new VendorInfo
		{
			Id = 3,
			DisplayName = "v3",
			Cuisines = new []{ "c1", "c2"}
		};
		#endregion

		[SetUp]
		public void Setup()
		{
			_deliveryClubService = new Mock<IDeliveryClubService>(MockBehavior.Strict).Object;

			_cut = new RerollIfEmptyFinalFoodResultsRandomFoodStrategy(_deliveryClubService);
		}

		#region RollVendor
		[Test]
		public void RollVendor_EmptyVendortList_ReturnsEmpty()
		{
			//arrange
			var totalList = new List<VendorInfo>();
			var request = TestDataFactory.CreateRequest();
			//act
			//assert
			Assert.ThrowsAsync<InvalidOperationException>(()=> _cut.RollVendor(request, totalList, CancellationToken.None));
			_deliveryClubService.VerifyAll();
		}

		[Test]
		public void RollVendor_NoVendorPointReturnsHaveFood_Throws()
		{
			//arrange
			var request = TestDataFactory.CreateRequest();
			var totalList = new List<VendorInfo> { _vendorInfo, _vendorInfo2, _vendorInfo3 };
			var foodInfoGetResponse = new FoodInfoGetResponse(new List<FoodInfo>());

			_deliveryClubService.SetupSequence(x => x.GetFoodInfo(It.IsAny<FoodInfoGetRequest>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(foodInfoGetResponse)
				.ReturnsAsync(foodInfoGetResponse)
				.ReturnsAsync(foodInfoGetResponse);
			//act
			//assert
			Assert.ThrowsAsync<InvalidOperationException>(() => _cut.RollVendor(request, totalList, CancellationToken.None));
		}

		[Test]
		public void RollVendor_RetryLimitReached_Throws()
		{
			//arrange
			_cut = new RerollIfEmptyFinalFoodResultsRandomFoodStrategy(_deliveryClubService, maxRerollAttempts: 2);
			var request = TestDataFactory.CreateRequest();
			var totalList = new List<VendorInfo> { _vendorInfo,_vendorInfo2,_vendorInfo3 };
			var foodInfoGetResponse = new FoodInfoGetResponse(new List<FoodInfo>());
			var realResp = new FoodInfoGetResponse(new List<FoodInfo>
			{
				new FoodInfo(),
				new FoodInfo()
			});

			_deliveryClubService.SetupSequence(x => x.GetFoodInfo(It.IsAny<FoodInfoGetRequest>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(foodInfoGetResponse)
				.ReturnsAsync(foodInfoGetResponse)
				.ReturnsAsync(realResp);
			//act
			//assert
			Assert.ThrowsAsync<InvalidOperationException>(() => _cut.RollVendor(request, totalList, CancellationToken.None));
		}

		[Test]
		public async Task RollVendor_SecondReturnsFoodItems_ReturnsResult()
		{
			//arrange
			_cut = new RerollIfEmptyFinalFoodResultsRandomFoodStrategy(_deliveryClubService, maxRerollAttempts: 2);
			var request = TestDataFactory.CreateRequest();
			var totalList = new List<VendorInfo> { _vendorInfo,_vendorInfo2,_vendorInfo3 };
			var foodInfoGetResponse = new FoodInfoGetResponse(new List<FoodInfo>());
			var realResp = new FoodInfoGetResponse(new List<FoodInfo>
			{
				new FoodInfo(),
				new FoodInfo()
			});

			_deliveryClubService.SetupSequence(x => x.GetFoodInfo(It.IsAny<FoodInfoGetRequest>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(foodInfoGetResponse)
				.ReturnsAsync(realResp);
			//act
			var result = await _cut.RollVendor(request, totalList, CancellationToken.None);
			//assert
			CollectionAssert.AreEqual(realResp.FoodInfos, result.Values.Single());
		}
		#endregion

		#region RollFood
		[Test]
		public void RollFood_HaveMoreThenOneVendorPointInfo_Throws()
		{
			//arrange
			var readOnlyDictionary = new Dictionary<VendorInfo, IReadOnlyCollection<FoodInfo>>
			{
				{ _vendorInfo, new List<FoodInfo>() },
				{ _vendorInfo2, new List<FoodInfo>() }
			};

			//act
			//assert
			Assert.Throws<ArgumentException>(() => _cut.RollFood(readOnlyDictionary));
		}

		[Test]
		public void RollFood_HaveFoodInfos_Return4Items()
		{
			//arrange
			var readOnlyDictionary = new Dictionary<VendorInfo, IReadOnlyCollection<FoodInfo>>
			{
				{ _vendorInfo, new List<FoodInfo>
					{
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo()
					}
				}
			};
			//act
			var result =  _cut.RollFood(readOnlyDictionary);
			//assert
			Assert.AreEqual(4, result.Values.Single().proposedList.Count());
		}

		[Test]
		public void RollFood_HaveMoreThen4MaxFoodItems_ReturnSetCount()
		{
			//arrange
			var maxFoodItems = 7;
			_cut =new RerollIfEmptyFinalFoodResultsRandomFoodStrategy(_deliveryClubService, maxFoodItems);
			var readOnlyDictionary = new Dictionary<VendorInfo, IReadOnlyCollection<FoodInfo>>
			{
				{ _vendorInfo, new List<FoodInfo>
					{
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo(),
						new FoodInfo()
					}
				}
			};
			//act
			var result =  _cut.RollFood(readOnlyDictionary);
			//assert
			Assert.AreEqual(maxFoodItems, result.Values.Single().proposedList.Count());
		}


		[Test]
		public void RollFood_HaveNoVendorPointInfo_Throws()
		{
			//arrange
			var readOnlyDictionary = new Dictionary<VendorInfo, IReadOnlyCollection<FoodInfo>>();

			//act
			//assert
			Assert.Throws<ArgumentNullException>(() => _cut.RollFood(readOnlyDictionary));
		}

		#endregion
	}
}