using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.FoodAgregators;
using GetTheFoodAlready.Api.FoodAgregators.Requests;
using GetTheFoodAlready.Api.FoodAgregators.Responses;
using GetTheFoodAlready.Handlers.Orchestration;
using GetTheFoodAlready.Handlers.RandomFoodRolling;
using GetTheFoodAlready.Handlers.Tests.Support;
using GetTheFoodAlready.TestsCommon;

using Moq;

using NUnit.Framework;

namespace GetTheFoodAlready.Handlers.Tests.Unit.Orchestration
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Unit")]
	public class GetRandomFoodPropositionsHandlerTests
	{
		private GetRandomFoodPropositionsHandler _cut;

		private IDeliveryClubService _deliveryClubService;
		private IRandomFoodStrategyProvider _strategyProvider;
		private IVendorFilter _vendorFilter;

		private IRandomFoodStrategy _strategy;

		[SetUp]
		public void Setup()
		{
			_deliveryClubService = Mock.Of<IDeliveryClubService>();
			_strategyProvider = Mock.Of<IRandomFoodStrategyProvider>();
			_vendorFilter = Mock.Of<IVendorFilter>();
			_strategy = Mock.Of<IRandomFoodStrategy>();

			_cut = new GetRandomFoodPropositionsHandler(_deliveryClubService, _strategyProvider, _vendorFilter);
		}

		[Test]
		public async Task Handle_ParametersUnderTest_ExpectedResult()
		{
			//arrange
			var request = TestDataFactory.CreateRequest("11", "22");
			var resp = new ClosestVendorPointsGetResponse(new [] { new VendorInfo(), new VendorInfo() });
			var vendorInfos = new List<VendorInfo>();
			var rollVendorResult = new Dictionary<VendorInfo, IReadOnlyCollection<FoodInfo>>();
			var rollFoodResult = new Dictionary<VendorInfo, (IEnumerable<FoodInfo> totalList, IEnumerable<FoodInfo> proposedList)>();

			_deliveryClubService.Setup(x => x.GetClosestVendorPoints(It.Is<ClosestVendorPointsGetRequest>(r => r.Longitude == "22" && r.Latitude == "11"), It.IsAny<CancellationToken>()))
				.ReturnsAsync(resp);
			_vendorFilter.Setup(x => x.FilterVendors(request, resp.Vendors))
				.Returns(vendorInfos);
			_strategyProvider.Setup(x => x.GetStrategy(request))
				.Returns(_strategy);
			_strategy.Setup(x => x.RollVendor(request, vendorInfos, It.IsAny<CancellationToken>()))
				.ReturnsAsync(rollVendorResult);
			_strategy.Setup(x => x.RollFood(rollVendorResult))
				.Returns(rollFoodResult);
			//act
			var result = await _cut.Handle(request, CancellationToken.None);
			//assert
			Assert.AreEqual(rollFoodResult, result.VendorsToRollResults);
		}
	}
}