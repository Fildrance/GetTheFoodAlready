using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.DeliveryClubBridge.Client;
using GetTheFoodAlready.DeliveryClubBridge.DataTypes;
using GetTheFoodAlready.TestsCommon;

using Moq;

using NUnit.Framework;

namespace GetTheFoodAlready.DeliveryClubBridge.Tests.Unit
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Unit")]
	public class AutoLoginningDeliveryClubClientDecoratorTests
	{
		private const string Longitude = "12.1212421";
		private const string Latitude = "34.4345134";
		private const int VendorPointId = 321312;
		private AutoLoginningDeliveryClubClientDecorator _cut;

		private IDeliveryClubClient _stubClient;

		[SetUp]
		public void Setup()
		{
			_stubClient = Mock.Of<IDeliveryClubClient>();

			_cut = new AutoLoginningDeliveryClubClientDecorator(_stubClient);
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_FirstNestedSucceeds_ReturnsNestedResult()
		{
			//arrange
			var cancellationToken = new CancellationToken();
			var resp = new RootDeliveryClubVendorsResponse();

			_stubClient.Setup(x => x.GetDeliveryClubVendorsNearby(Longitude, Latitude, cancellationToken, 0, 200))
				.Returns(Task.FromResult(resp));
			//act
			var result = await _cut.GetDeliveryClubVendorsNearby(Longitude, Latitude, cancellationToken);
			//assert
			Assert.AreEqual(resp, result);
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_FirstNestedSucceeds_LoginCalledBeforeAnyOther()
		{
			//arrange
			bool loginCalled = false;
			var resp = new RootDeliveryClubVendorsResponse();

			_stubClient.Setup(x => x.Login(It.IsAny<CancellationToken>(), false))
				.Callback(new InvocationAction(invocation => loginCalled = true))
				.Returns(Task.FromResult("someToken"));

			_stubClient.Setup(x => x.GetDeliveryClubVendorsNearby(Longitude, Latitude, It.IsAny<CancellationToken>(), 0, 200))
				.Callback(new InvocationAction(invocation => Assert.True(loginCalled)))
				.Returns(Task.FromResult(resp));
			//act
			//assert
			await _cut.GetDeliveryClubVendorsNearby(Longitude, Latitude);
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_FirstNestedThrowsWith401_LoginAndNestedCalledSecondTime()
		{
			//arrange
			var resp = new RootDeliveryClubVendorsResponse();

			_stubClient.SetupSequence(x => x.GetDeliveryClubVendorsNearby(Longitude, Latitude, It.IsAny<CancellationToken>(), 0, 200))
				.Throws(new HttpRequestException("Request failed with 401 status"))
				.Returns(Task.FromResult(resp));
			//act
			//assert
			await _cut.GetDeliveryClubVendorsNearby(Longitude, Latitude);

			_stubClient.Verify(x=>x.Login(It.IsAny<CancellationToken>(), false), Times.Once());
			_stubClient.Verify(x => x.Login(It.IsAny<CancellationToken>(), true), Times.Once());
		}


		[Test]
		public async Task GetFoodInfo_FirstNestedSucceeds_ReturnsNestedResult()
		{
			//arrange
			var cancellationToken = new CancellationToken();
			var resp = new DeliveryClubFoodInfo();

			_stubClient.Setup(x => x.GetFoodInfo(VendorPointId, cancellationToken))
				.Returns(Task.FromResult(resp));
			//act
			var result = await _cut.GetFoodInfo(VendorPointId, cancellationToken);
			//assert
			Assert.AreEqual(resp, result);
		}

		[Test]
		public async Task GetFoodInfo_FirstNestedSucceeds_LoginCalledBeforeAnyOther()
		{
			//arrange
			bool loginCalled = false;
			var resp = new DeliveryClubFoodInfo();

			_stubClient.Setup(x => x.Login(It.IsAny<CancellationToken>(), false))
				.Callback(new InvocationAction(invocation => loginCalled = true))
				.Returns(Task.FromResult("someToken"));

			_stubClient.Setup(x => x.GetFoodInfo(VendorPointId, It.IsAny<CancellationToken>()))
				.Callback(new InvocationAction(invocation => Assert.True(loginCalled)))
				.Returns(Task.FromResult(resp));
			//act
			//assert
			await _cut.GetFoodInfo(VendorPointId, It.IsAny<CancellationToken>());
		}

		[Test]
		public async Task GetFoodInfo_FirstNestedThrowsWith401_LoginAndNestedCalledSecondTime()
		{
			//arrange
			var resp = new DeliveryClubFoodInfo();

			_stubClient.SetupSequence(x => x.GetFoodInfo(VendorPointId, It.IsAny<CancellationToken>()))
				.Throws(new HttpRequestException("Request failed with 401 status"))
				.Returns(Task.FromResult(resp));
			//act
			//assert
			await _cut.GetFoodInfo(VendorPointId, It.IsAny<CancellationToken>());

			_stubClient.Verify(x => x.Login(It.IsAny<CancellationToken>(), false), Times.Once());
			_stubClient.Verify(x => x.Login(It.IsAny<CancellationToken>(), true), Times.Once());
		}
	}
}