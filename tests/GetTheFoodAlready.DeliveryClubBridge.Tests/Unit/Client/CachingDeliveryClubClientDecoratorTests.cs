using GetTheFoodAlready.Api.Cache;
using GetTheFoodAlready.DeliveryClubBridge.Client;
using GetTheFoodAlready.DeliveryClubBridge.DataTypes;
using GetTheFoodAlready.TestsCommon;
using Moq;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace GetTheFoodAlready.DeliveryClubBridge.Tests.Unit.Client
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Unit")]
	public class CachingDeliveryClubClientDecoratorTests
	{
		CachingDeliveryClubClientDecorator _cut;

		IDeliveryClubClient _nested;
		ICacheManager<RootDeliveryClubVendorsResponse> _vendorCache;
		ICacheManager<DeliveryClubFoodInfo> _foodCache;

		DeliveryClubFoodInfo expectedFood = new DeliveryClubFoodInfo();
		RootDeliveryClubVendorsResponse expectedVendor = new RootDeliveryClubVendorsResponse();

		private const int VendorId = 54321;
		const string Lng = "12.21";
		const string Lat = "51.12";
		const int skip = 2;
		const int take = 10;
		readonly string VendorSearchKey = $"{Lng}{Lat}{skip}{take}";

		[SetUp]
		public void Setup()
		{
			_nested = Mock.Of<IDeliveryClubClient>();

			_vendorCache = Mock.Of<ICacheManager<RootDeliveryClubVendorsResponse>>();
			_foodCache = Mock.Of<ICacheManager<DeliveryClubFoodInfo>>();

			var factory = Mock.Of<ICacheManagerFactory>();
			factory.Setup(x => x.GetManager<RootDeliveryClubVendorsResponse>())
				.Returns(_vendorCache);
			factory.Setup(x => x.GetManager<DeliveryClubFoodInfo>())
				.Returns(_foodCache);

			_nested.Setup(x => x.GetFoodInfo(VendorId, It.IsAny<CancellationToken>()))
				.Returns(Task.FromResult(expectedFood));
			_nested.Setup(x => x.GetDeliveryClubVendorsNearby(Lng, Lat, CancellationToken.None, 2, 10))
				.Returns(Task.FromResult(expectedVendor));

			_cut = new CachingDeliveryClubClientDecorator(_nested, factory);
		}

		#region GetFoodInfo

		[Test]
		public async Task GetFoodInfo_NoCached_NestedCalledAsync()
		{
			//arrange
			DeliveryClubFoodInfo val = null;
			_foodCache.Setup(x => x.TryGet(VendorId.ToString(), out val))
				.Returns(Task.FromResult(false));
			//act
			var actual = await _cut.GetFoodInfo(VendorId, CancellationToken.None);
			//assert
			Assert.AreEqual(expectedFood, actual);
		}

		[Test]
		public async Task GetFoodInfo_NoCached_SetsCache()
		{
			//arrange
			DeliveryClubFoodInfo val = null;
			_foodCache.Setup(x => x.TryGet(VendorId.ToString(), out val))
				.Returns(Task.FromResult(false));
			//act
			var actual = await _cut.GetFoodInfo(VendorId, CancellationToken.None);
			//assert
			_foodCache.Verify(x => x.Set(VendorId.ToString(), expectedFood), Times.Once());
		}

		[Test]
		public async Task GetFoodInfo_GetCached_NestedNotCalled()
		{
			//arrange
			_foodCache.Setup(x => x.TryGet(VendorId.ToString(), out expectedFood))
				.Returns(Task.FromResult(true));
			//act
			await _cut.GetFoodInfo(VendorId, CancellationToken.None);
			//assert
			_nested.Verify(x => x.GetFoodInfo(VendorId, It.IsAny<CancellationToken>()), Times.Never());
		}

		[Test]
		public async Task GetFoodInfo_GetCached_ReturnCached()
		{
			//arrange
			_foodCache.Setup(x => x.TryGet(VendorId.ToString(), out expectedFood))
				.Returns(Task.FromResult(true));
			//act
			var actual = await _cut.GetFoodInfo(VendorId, CancellationToken.None);
			//assert
			Assert.AreEqual(expectedFood, actual);
		}

		#endregion

		#region
		[Test]
		public async Task GetDeliveryClubVendorsNearby_NoCached_NestedCalledAsync()
		{
			//arrange
			RootDeliveryClubVendorsResponse val = null;
			_vendorCache.Setup(x => x.TryGet(VendorSearchKey, out val))
				.Returns(Task.FromResult(false));
			//act
			var actual = await _cut.GetDeliveryClubVendorsNearby(Lng, Lat, CancellationToken.None, 2, 10);
			//assert
			Assert.AreEqual(expectedVendor, actual);
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_NoCached_SetsCache()
		{
			//arrange
			RootDeliveryClubVendorsResponse val = null;
			_vendorCache.Setup(x => x.TryGet(VendorSearchKey, out val))
				.Returns(Task.FromResult(false));
			//act
			var actual = await _cut.GetDeliveryClubVendorsNearby(Lng, Lat, CancellationToken.None, 2, 10);
			//assert
			_vendorCache.Verify(x => x.Set(VendorSearchKey, expectedVendor), Times.Once());
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_GetCached_NestedNotCalled()
		{
			//arrange
			_vendorCache.Setup(x => x.TryGet(VendorSearchKey, out expectedVendor))
				.Returns(Task.FromResult(true));
			//act
			await _cut.GetDeliveryClubVendorsNearby(Lng, Lat, CancellationToken.None, 2, 10);
			//assert
			_nested.Verify(x => x.GetDeliveryClubVendorsNearby(Lng, Lat, It.IsAny<CancellationToken>(), 2, 10), Times.Never());
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_GetCached_ReturnCached()
		{
			//arrange
			_vendorCache.Setup(x => x.TryGet(VendorSearchKey, out expectedVendor))
				.Returns(Task.FromResult(true));
			//act
			var actual = await _cut.GetDeliveryClubVendorsNearby(Lng, Lat, CancellationToken.None, 2, 10);
			//assert
			Assert.AreEqual(expectedVendor, actual);
		}

		#endregion

		[Test]
		public async Task Login_NoStubPrepped_OnlyNestedCalled()
		{
			//arrange
			_vendorCache = new Mock<ICacheManager<RootDeliveryClubVendorsResponse>>(MockBehavior.Strict).Object;
			_foodCache = new Mock<ICacheManager<DeliveryClubFoodInfo>>(MockBehavior.Strict).Object;

			var factory = Mock.Of<ICacheManagerFactory>();
			factory.Setup(x => x.GetManager<RootDeliveryClubVendorsResponse>())
				.Returns(_vendorCache);
			factory.Setup(x => x.GetManager<DeliveryClubFoodInfo>())
				.Returns(_foodCache);
			_cut = new CachingDeliveryClubClientDecorator(_nested, factory);
			//act
			await _cut.Login(CancellationToken.None);
			//assert
			_vendorCache.VerifyAll();
			_foodCache.VerifyAll();
		}
	}
}
