using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.DeliveryClubBridge.Client;
using GetTheFoodAlready.DeliveryClubBridge.DataTypes;
using GetTheFoodAlready.DeliveryClubBridge.Tests.Support;

using NUnit.Framework;

namespace GetTheFoodAlready.DeliveryClubBridge.Tests.Unit.Client
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Unit")]
	public class DeliveryClubClientTests
	{
		private DeliveryClubClient _cut;

		private HttpMessageHandler _stubHandler;
		private Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _func;

		private const string Longitude = "12.312412";
		private const string Latitude = "54.23123";
		private const int VendorPointId = 5145134;
		private static readonly string Token = "8345u793347513784yf134f7134ffg33g9u81h345g";
		private static readonly string Secret = "54A1B35C13";
		private static readonly string SecretToken = $"{{token : \"{Token}\", secret : \"{Secret}\" }}";
		private static readonly string UserAuth = $"{Token}.{Secret}";
		private readonly HttpResponseMessage _okAuthResponse = new HttpResponseMessage(HttpStatusCode.OK)
		{
			Content = new StringContent(SecretToken)
		};

		[SetUp]
		public void Setup()
		{
			_stubHandler = new DelegatingHandlerStub((message, token) => _func(message, token));
			var deliveryClubClientSettings = new DeliveryClubClientSettings("https://api.delivery-club.ru", "api1.2");

			_cut = new DeliveryClubClient(nested => _stubHandler, deliveryClubClientSettings);
		}

		[Test]
		public async Task Login_HttpRequestReturnsResult_ReturnsToken()
		{
			//arrange
			_func = (message, token) => Task.FromResult(_okAuthResponse);
			//act
			var result = await _cut.Login(CancellationToken.None);
			//assert
			StringAssert.AreEqualIgnoringCase("8345u793347513784yf134f7134ffg33g9u81h345g.54A1B35C13", result);
		}

		[Test]
		public async Task Login_HttpRequestReturnsResult_RequestingProperUrl()
		{
			//arrange
			var actualUrl = "";
			_func = (message, token) =>
			{
				actualUrl = message.RequestUri.ToString();
				return Task.FromResult(_okAuthResponse);
			};
			//act
			await _cut.Login(CancellationToken.None);
			//assert
			StringAssert.AreEqualIgnoringCase("https://api.delivery-club.ru/api1.2/user/login", actualUrl);
		}

		[Test]
		public async Task Login_HttpRequestReturnsResult_SecondCallNotHappening()
		{
			var callCount = 0;
			//arrange
			_func = (message, token) =>
			{
				callCount++;
				return Task.FromResult(_okAuthResponse);
			};
			//act
			await _cut.Login(CancellationToken.None);
			await _cut.Login(CancellationToken.None);
			//assert
			Assert.AreEqual(1, callCount);
		}

		[Test]
		public async Task Login_HttpRequestReturnsResult_HeadersContainTokenAndSecret()
		{
			//arrange
			HttpRequestHeaders headers = null;
			bool isSecondCall = false;
			_func = (message, token) =>
			{
				if (isSecondCall)
				{
					headers = message.Headers;
				}

				isSecondCall = true;
				return Task.FromResult(_okAuthResponse);
			};
			//act
			await _cut.Login(CancellationToken.None);
			await _cut.GetFoodInfo(123, CancellationToken.None);
			//assert
			var cookkieHeader = headers?.GetValues("cookie")
				.SingleOrDefault();
			Assert.NotNull(cookkieHeader);
			Assert.AreEqual($"x_user_authorization={UserAuth}", cookkieHeader);
			var userAuth = headers?.GetValues("x-user-authorization")
				.SingleOrDefault();
			Assert.NotNull(userAuth);
			Assert.AreEqual(UserAuth, userAuth);
		}

		[Test]
		public void Login_HttpRequestReturns401_Throws()
		{
			//arrange
			_func = (message, token) =>
			{
				var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
				return Task.FromResult(httpResponseMessage);
			};
			//act
			//assert
			Assert.ThrowsAsync<HttpRequestException>(() => _cut.Login(CancellationToken.None));
		}

		[Test]
		public void GetFoodInfo_HttpRequestReturns401_Throws()
		{
			//arrange
			_func = (message, token) =>
			{
				var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
				return Task.FromResult(httpResponseMessage);
			};
			//act
			//assert
			Assert.ThrowsAsync<HttpRequestException>(() => _cut.GetFoodInfo(1231, CancellationToken.None));
		}

		[Test]
		public async Task GetFoodInfo_PassParameters_CallsProperUrl()
		{
			//arrange
			var url = "";
			_func = (message, token) =>
			{
				url = message.RequestUri.ToString();
				var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StringContent("{}")
				};
				return Task.FromResult(httpResponseMessage);
			};
			//act
			await _cut.GetFoodInfo(VendorPointId, CancellationToken.None);
			//assert
			Assert.AreEqual($"https://api.delivery-club.ru/api1.2/vendor/{VendorPointId}/menu?data=menu,products,actions", url);
		}

		[Test]
		public async Task GetFoodInfo_ResponseContainInfo_ReturnsDeserializedData()
		{
			//arrange
			var primaryId = "34651436";
			var productItem = new MenuItem
			{
				Name = "someName",
				Description = "item description",
				Id = new InventoryId {Primary = primaryId},
				Price = new ItemPrice {Value = 3432},
				ItemProperties = new ItemProperties {Volume = 435314}
			};
			var menuCat = new MenuCategory
			{
				Name = "menuItem",
				ProductIds = new List<string>
				{
					primaryId
				}
			};
			_func = (message, token) =>
			{
				var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StringContent(TestResources.FoodInfoRespForTest)
				};
				return Task.FromResult(httpResponseMessage);
			};
			//act
			var result = await _cut.GetFoodInfo(VendorPointId, CancellationToken.None);
			//assert
			Assert.AreEqual(productItem.Name, result.Products.Single().Name);
			Assert.AreEqual(productItem.Description, result.Products.Single().Description);
			Assert.AreEqual(productItem.Id.Primary, result.Products.Single().Id.Primary);
			Assert.AreEqual(productItem.Price.Value, result.Products.Single().Price.Value);
			Assert.AreEqual(productItem.ItemProperties.Volume, result.Products.Single().ItemProperties?.Volume);

			Assert.AreEqual(menuCat.Name, result.Menu.Single().Name);
			Assert.AreEqual(menuCat.ProductIds.Single(), result.Menu.Single().ProductIds.Single());
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_PassParameters_CallsProperUrl()
		{
			//arrange
			var url = "";
			_func = (message, token) =>
			{
				url = message.RequestUri.ToString();
				var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StringContent("{}")
				};
				return Task.FromResult(httpResponseMessage);
			};
			//act
			await _cut.GetDeliveryClubVendorsNearby(Longitude, Latitude, CancellationToken.None);
			//assert
			Assert.AreEqual($"https://api.delivery-club.ru/api1.2/vendors?limit=200&offset=0&latitude={Latitude}&longitude={Longitude}", url);
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_PassTakeSkip_CallsProperUrl()
		{
			//arrange
			var url = "";
			_func = (message, token) =>
			{
				url = message.RequestUri.ToString();
				var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StringContent("{}")
				};
				return Task.FromResult(httpResponseMessage);
			};
			//act
			await _cut.GetDeliveryClubVendorsNearby(Longitude, Latitude, CancellationToken.None, 100, 90);
			//assert
			Assert.AreEqual($"https://api.delivery-club.ru/api1.2/vendors?limit=90&offset=100&latitude={Latitude}&longitude={Longitude}", url);
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_ResponseContainInfo_ReturnsDeserializedData()
		{
			//arrange
			_func = (message, token) =>
			{
				var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StringContent(TestResources.GetDeliveryClubVendorsNearbyRespForTest)
				};
				return Task.FromResult(httpResponseMessage);
			};
			//act
			var result = await _cut.GetDeliveryClubVendorsNearby(Longitude, Latitude, CancellationToken.None);
			//assert
			Assert.AreEqual(2, result.PagedList.Count);
			Assert.AreEqual(true, result.PagedList.HasMore);
			var vendorInfo = result.PagedList.Vendors.Single();
			Assert.AreEqual("TEST-TEST", vendorInfo.Name);
			Assert.AreEqual(true, vendorInfo.Delivery.Available);
			Assert.AreEqual(500, vendorInfo.Delivery.MinOrderPrice.Value);
			Assert.AreEqual(0, vendorInfo.Delivery.Price.Value);
			Assert.AreEqual("4.5", vendorInfo.Reviews.Score);
			CollectionAssert.AreEqual(new []{ "Суши", "Завтраки", "Супы", "Пицца" }, vendorInfo.Cuisines);
			Assert.True(vendorInfo.Labels.Any(x=>x == "delivery_free"));

		}

		[Test]
		public void GetDeliveryClubVendorsNearby_HttpRequestReturns401_Throws()
		{
			//arrange
			_func = (message, token) =>
			{
				var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
				return Task.FromResult(httpResponseMessage);
			};
			//act
			//assert
			Assert.ThrowsAsync<HttpRequestException>(() => _cut.GetDeliveryClubVendorsNearby(Longitude, Latitude, CancellationToken.None));
		}

		private class DelegatingHandlerStub : DelegatingHandler
		{
			private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;

			public DelegatingHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc)
			{
				_handlerFunc = handlerFunc ?? throw new ArgumentNullException(nameof(handlerFunc));
			}

			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				return _handlerFunc(request, cancellationToken);
			}
		}
	}
}