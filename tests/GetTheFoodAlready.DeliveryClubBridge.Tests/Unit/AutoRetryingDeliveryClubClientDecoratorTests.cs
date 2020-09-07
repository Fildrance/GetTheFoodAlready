using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
	public class AutoRetryingDeliveryClubClientDecoratorTests
	{
		private AutoRetryingDeliveryClubClientDecorator _cut;

		private IDeliveryClubClient _stubClient;

		private const string Longitude = "12.1212421";
		private const string Latitude = "34.4345134";
		private const int VendorPointId = 543513;

		[SetUp]
		public void Setup()
		{
			_stubClient = Mock.Of<IDeliveryClubClient>();

			_cut =new AutoRetryingDeliveryClubClientDecorator(_stubClient);
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_NestedSucceeds_OnlyCalledOnce()
		{
			//arrange
			var resp = new RootDeliveryClubVendorsResponse();

			_stubClient.Setup(x => x.GetDeliveryClubVendorsNearby(Longitude, Latitude, It.IsAny<CancellationToken>(), 0, 200))
				.Returns(Task.FromResult(resp));
			//act
			await _cut.GetDeliveryClubVendorsNearby(Longitude, Latitude);
			//assert
			_stubClient.Verify(x=>x.GetDeliveryClubVendorsNearby(Longitude, Latitude, It.IsAny<CancellationToken>(), 0, 200), Times.Once());
		}

		// this decorator is made to check if method have failed to acquire data due to some implicit errors in delivery-club api service.
		[Test]
		public void GetDeliveryClubVendorsNearby_NestedThrows_Throws()
		{
			//arrange

			_stubClient.Setup(x => x.GetDeliveryClubVendorsNearby(Longitude, Latitude, It.IsAny<CancellationToken>(), 0, 200))
				.Throws<InvalidOperationException>();
			//act
			//assert
			Assert.ThrowsAsync<InvalidOperationException>(() => _cut.GetDeliveryClubVendorsNearby(Longitude, Latitude));
		}

		[Test]
		public async Task GetFoodInfo_NestedSucceeds_OnlyCalledOnce()
		{
			//arrange
			var resp = new DeliveryClubFoodInfo
			{
				Products = new List<MenuItem>
				{
					new MenuItem()
				}
			};

			_stubClient.Setup(x => x.GetFoodInfo(VendorPointId, It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(resp));
			//act
			await _cut.GetFoodInfo(VendorPointId, CancellationToken.None);
			//assert
			_stubClient.Verify(x=>x.GetFoodInfo(VendorPointId, It.IsAny<CancellationToken>()), Times.Once());
		}

		// this decorator is made to check if method have failed to acquire data due to some implicit errors in delivery-club api service.
		[Test]
		public void GetFoodInfo_NestedThrows_Throws()
		{
			//arrange
			_stubClient.Setup(x => x.GetFoodInfo(VendorPointId, It.IsAny<CancellationToken>()))
				.Throws<InvalidOperationException>();
			//act
			//assert
			Assert.ThrowsAsync<InvalidOperationException>(() => _cut.GetFoodInfo(VendorPointId, CancellationToken.None));
		}

		[Test]
		public async Task GetFoodInfo_NestedReturnsEmptyOnceThenSucceeds_ReturnsSuccess()
		{
			//arrange
			var empty = new DeliveryClubFoodInfo();
			var success = new DeliveryClubFoodInfo
			{
				Products = new List<MenuItem>
				{
					new MenuItem()
				}
			};
			_stubClient.SetupSequence(x => x.GetFoodInfo(VendorPointId, It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(empty))
				.Returns(Task.FromResult(success));
			//act
			var result = await _cut.GetFoodInfo(VendorPointId, CancellationToken.None);
			//assert
			Assert.AreEqual(success, result);
		}


		[Test]
		public async Task GetFoodInfo_NestedReturnsEmptyMaxRetryTimes_ReturnsLastOne()
		{
			//arrange
			_cut =new AutoRetryingDeliveryClubClientDecorator(_stubClient, 7);
			var empty = new DeliveryClubFoodInfo();
			var last = new DeliveryClubFoodInfo();
			_stubClient.SetupSequence(x => x.GetFoodInfo(VendorPointId, It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(empty))
				.Returns(Task.FromResult(empty))
				.Returns(Task.FromResult(empty))
				.Returns(Task.FromResult(empty))
				.Returns(Task.FromResult(empty))
				.Returns(Task.FromResult(empty))
				.Returns(Task.FromResult(last));
			//act
			var result = await _cut.GetFoodInfo(VendorPointId, CancellationToken.None);
			//assert
			Assert.AreEqual(last, result);
		}

		[Test]
		public async Task Login_NestedSucceeds_OnlyCalledOnce()
		{
			//arrange
			_stubClient.Setup(x => x.Login(CancellationToken.None, false))
				.Returns(Task.FromResult("someToken"));
			//act
			await _cut.Login(CancellationToken.None);
			//assert
			_stubClient.Verify(x => x.Login(CancellationToken.None, false), Times.Once());
		}

		// this decorator is made to check if method have failed to acquire data due to some implicit errors in delivery-club api service.
		[Test]
		public void Login_NestedThrows_Throws()
		{
			//arrange
			_stubClient.Setup(x => x.Login(CancellationToken.None, false))
				.Throws<InvalidOperationException>();
			//act
			//assert
			Assert.ThrowsAsync<InvalidOperationException>(() => _cut.Login(CancellationToken.None));
		}

	}
}
